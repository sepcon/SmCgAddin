using System;
using System.Collections.Generic;
using System.Collections;
using ARSourceGeneration;
using System.Diagnostics;

namespace EAParsing
{
     class SMConnectorBaseParser : SMChartParser
    {
        public SMConnectorBaseParser(EA.Repository repository = null, EA.Diagram currentDiagram = null, DSource sourceData = null)
            : base(repository, currentDiagram, sourceData)
        {
            _alreadyParsedConnectors = new List<EA.Connector>();
            _delayParsingNodes = new List<EA.Element>();
        }

        public override bool parse()
        {
            System.Diagnostics.Debug.Assert(false);
            MessageLogger.startLogging();

            var objectsList = CurrentDiagram.DiagramObjects;
            foreach (EA.DiagramObject obj in objectsList)
            {
                EA.Element elem = getEAElement(obj.ElementID);
                if (isStateNode(elem))
                {
                    collectStateBehaviours(
                        elem,
                        createDState(elem) // create new DState
                        );
                }
                else if (isChoiceNode(elem))
                {
                    _delayParsingNodes.Add(elem);
                }
                else if (elem.Type == "Text")
                {
                    if (elem.Notes != string.Empty)
                    {
                        SourceData.DiagramNotes += "<br />" + elem.Notes + "<br />";
                    }
                }
            }

            var linksList = CurrentDiagram.DiagramLinks;
            foreach (EA.DiagramLink link in linksList)
            {
                EA.Connector ctor = Repository.GetConnectorByID(link.ConnectorID);
                if (!isStateFlowConnector(ctor))
                    continue;

                EA.Element sourceNode = getEAElement(ctor.ClientID);
                EA.Element targetNode = getEAElement(ctor.SupplierID);
                bool sourceNodeIsState = isStateNode(sourceNode);
                bool targetNodeIsState = isStateNode(targetNode);
                if (sourceNodeIsState && targetNodeIsState)
                {
                    createTransition(ctor, sourceNode, targetNode);
                }
                else if (getSubType(sourceNode) == ElementSubType.Initial && targetNodeIsState)
                {
                    EA.Element parent = getEAElement(sourceNode.ParentID);
                    if (parent != null)
                    {
                        DState parentState = getDState(parent);
                        parentState.addDefaultTransition(targetNode.ElementID, targetNode.Name);
                    }
                }
            }


            foreach (var node in _delayParsingNodes)
            {
                collectTransitionOnChoice(node);
            }

            MessageLogger.endLogging();
            return true;
        }

        protected void createTransition(EA.Connector ctor, EA.Element from, EA.Element to)
        {
            DState stateFrom = getDState(from);
            stateFrom.addTransition(ctor.TransitionEvent, ctor.TransitionGuard, ctor.TransitionAction, to.ElementID, to.Name);
        }


        protected void collectTransitionOnChoice(EA.Element eaChoice)
        {
            Debug.Assert(isChoiceNode(eaChoice));
            List<EA.Connector> firstHalfConnectorList = new List<EA.Connector>();
            List<EA.Connector> secondHalfConnectorList = new List<EA.Connector>();
            for(short i = 0; i < eaChoice.Connectors.Count; ++i)
            {
                EA.Connector ctor = eaChoice.Connectors.GetAt(i);
                if (!isStateFlowConnector(ctor))
                    continue;


                //MessageLogger.log(ctor.Type + " --> " + ctor.Direction);
                if(ctor.ClientID == eaChoice.ElementID)
                {
                    secondHalfConnectorList.Add(ctor);
                }
                else
                {
                    firstHalfConnectorList.Add(ctor);
                }
            }

            Debug.Assert(firstHalfConnectorList.Count > 0 && secondHalfConnectorList.Count > 0);

            detectErrorForTransitionThoughChoiceNode(eaChoice, firstHalfConnectorList, secondHalfConnectorList);

            string conditionOnChoice = eaChoice.Name;
            if(conditionOnChoice.ToLower() != "choice")
            {
                foreach (var firstHalf in firstHalfConnectorList)
                {
                    DState sourceState = getDState(getEAElement(firstHalf.ClientID));
                    foreach (var secondHalf in secondHalfConnectorList)
                    {
                        sourceState.addTransition(firstHalf.TransitionEvent, conditionOnChoice + " --> " + secondHalf.TransitionGuard, secondHalf.TransitionAction, secondHalf.SupplierID);
                    }
                }
            }
            else
            {
                string elseCondition = "NOT (";
                EA.Connector connectorContainsElse = null;
                foreach (var secondHalf in secondHalfConnectorList)
                {
                    if(secondHalf.TransitionGuard.ToLower() == "else")
                    {
                        connectorContainsElse = secondHalf;
                        continue;
                    }
                    else
                    {
                        elseCondition += secondHalf.TransitionGuard + " && ";
                    }
                    foreach (var firstHalf in firstHalfConnectorList)
                    {
                        DState sourceState = getDState(getEAElement(firstHalf.ClientID));
                        sourceState.addTransition(firstHalf.TransitionEvent, secondHalf.TransitionGuard, getActions(firstHalf, secondHalf), secondHalf.SupplierID);
                    }
                }

                if(connectorContainsElse != null)
                {
                    elseCondition = elseCondition.Substring(0, elseCondition.Length - 4 /*remove the last " && "*/) + ")"; 
                    foreach (var firstHalf in firstHalfConnectorList)
                    {
                        DState sourceState = getDState(getEAElement(firstHalf.ClientID));
                        sourceState.addTransition(firstHalf.TransitionEvent, elseCondition, getActions(firstHalf, connectorContainsElse), connectorContainsElse.SupplierID);
                    }
                }
            }
            
        }

        protected string getActions(EA.Connector ctorFrom, EA.Connector ctorTo)
        {
            if (ctorFrom.TransitionAction.Trim() == "")
                return ctorTo.TransitionAction;
            else
                return ctorFrom.TransitionAction.Trim() + "\r\n" + ctorTo.TransitionAction;
        }
        protected bool detectErrorForTransitionThoughChoiceNode(EA.Element eaChoice, List<EA.Connector> firstHalfCtors, List<EA.Connector> secondHalfCtors)
        {
            bool errorsDetected = false;
            foreach(var firstHalf in firstHalfCtors)
            {
                if (firstHalf.TransitionEvent.Trim() == "")
                {
                    errorsDetected = true;
                    EA.Element eaState = getEAElement(firstHalf.ClientID);
                    MessageLogger.logError(ErrorMessageFormatIndex.NotPermitToNotContainEventsBeforeChoiceNode, eaState.Name.ToUpper(), eaChoice.Name.ToUpper());
                    continue;
                }
                else if (firstHalf.TransitionGuard.Trim() != string.Empty)
                {
                    errorsDetected = true;
                    EA.Element eaState = getEAElement(firstHalf.ClientID);
                    MessageLogger.logError(ErrorMessageFormatIndex.MustNotContainGuardBeforeChoiceNode,
                        eaState.Name.ToUpper(), eaChoice.Name.ToUpper(),
                        firstHalf.TransitionAction.ToUpper() + "|" + firstHalf.TransitionGuard.ToUpper());
                }
            }
            

            foreach(var secondHalf in secondHalfCtors)
            {
                EA.Element eaNextNode = getEAElement(secondHalf.SupplierID);
                if(secondHalf.TransitionGuard.Trim() == "" && secondHalfCtors.Count > 1)
                {
                    errorsDetected = true;
                    MessageLogger.logError(ErrorMessageFormatIndex.NotPermitToMissActionsAndConditionAfterChoiceNode,
                            eaChoice.Name.ToUpper(),
                            eaNextNode.Name.ToUpper());
                }
                if (!isStateNode(eaNextNode))
                {
                    errorsDetected = true;
                    MessageLogger.logError(ErrorMessageFormatIndex.ConnectionFromChoiceNodeMustToStateNode,
                            eaChoice.Name.ToUpper(),
                            eaNextNode.Name.ToUpper());
                }

                if(secondHalf.TransitionEvent.Trim() != "")
                {
                    errorsDetected = true;
                    MessageLogger.logError(ErrorMessageFormatIndex.MustNotContainEventsAfterChoiceNode,
                            eaChoice.Name.ToUpper(),
                            eaNextNode.Name.ToUpper(), secondHalf.TransitionEvent.ToUpper());
                }

            }

            return errorsDetected;
        }

        protected DState createDState(EA.Element eaState)
        {
            DState dState = new DState(SourceData.Hsm, eaState.ElementID, eaState.Name, eaState.ParentID);
            dState.Notes = eaState.Notes;
            if (!dState.isRootState())
            {
                var parentNode = getEAElement(dState.ParentId);
                if (!isStateNode(parentNode))
                {
                    dState.ParentId = 0;
                }
            }
            return dState;
        }

        protected DState getDState(EA.Element eaState)
        {
            DState dState = SourceData.Hsm.getState(eaState.ElementID);
            if (dState == null)
            {
                dState = createDState(eaState);
            }
            return dState;
        }

        protected void collectStateBehaviours(EA.Element eaState, ARSourceGeneration.DState smState)
        {
            Debug.Assert(eaState != null);
            var methods = eaState.Methods;
            for (short mt_i = 0; mt_i < methods.Count; ++mt_i)
            {
                EA.Method mt = methods.GetAt(mt_i);
                if (mt != null)
                {
                    smState.addInternalOperation(mt.ReturnType, mt.Name);
                }
            }
        }

        protected EA.Element getEAElement(int id)
        {
            if(_eaElementsCache.ContainsKey(id))
            {
                return _eaElementsCache[id];
            }
            else
            {
                EA.Element elem = safelyGetElement(id);
                _eaElementsCache.Add(id, elem);
                return elem;
            }
        }
        protected bool isStateFlowConnector(EA.Connector ctor)
        {
            return ctor.Type == "StateFlow";
        }
        protected bool isStateNode(EA.Element e)
        {
            return e != null && e.Type == "State";
        }
    
        protected List<EA.Element> _delayParsingNodes;
        protected List<EA.Connector> _alreadyParsedConnectors;
        protected Dictionary<int, EA.Element> _eaElementsCache = new Dictionary<int,EA.Element>();

    }

}
