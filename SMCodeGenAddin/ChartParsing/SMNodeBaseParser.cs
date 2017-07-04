using System;
using System.Collections.Generic;
using System.Diagnostics;

using ARSourceGeneration;


namespace EAParsing
{
    [System.Obsolete("SMNodeBaseParser is worst in performance")]
    class SMNodeBaseParser : SMChartParser
    {

        public SMNodeBaseParser(EA.Repository repository = null, EA.Diagram currentDiagram = null, DSource sourceData = null)
            : base(repository, currentDiagram, sourceData)
        {
        }


        protected override bool isRequiredChartType(EA.Diagram dg)
        {
            return (dg != null && chartType().ToLower() == "statechart");
        }


        public override bool parse()
        {
            bool parseSuccessfull = false;
            if (CurrentDiagram != null)
            {
                SourceData.DiagramNotes = CurrentDiagram.Notes;
                var diagramObjects = CurrentDiagram.DiagramObjects;
                
                foreach (EA.DiagramObject eaDiag in diagramObjects)
                {

                    EA.Element elem = safelyGetElement(eaDiag.ElementID);
                    if (elem == null)
                    {
                        continue;
                    }

                    if (elem.Type == "State")
                    {
                        parseState(elem);
                    }
                    else if (getSubType(elem) == ElementSubType.Initial)
                    {
                        DelayParsingNodes.Add(elem);
                    }
                    else if(elem.Type == "Text")
                    {
                        if(elem.Notes != string.Empty)
                        {
                            SourceData.DiagramNotes += "<br />" + elem.Notes+ "<br />";
                        }
                    }
                }

                parseDelayedNodes();
                parseSuccessfull = true;
            }
            else
            {
                parseSuccessfull = false;
            }
            return parseSuccessfull;
        }

        public void parseState(EA.Element eaState)
        {
            ARSourceGeneration.DState smState = new ARSourceGeneration.DState(SourceData.Hsm, eaState.ElementID, eaState.Name, eaState.ParentID);
            if(smState.ParentId != 0)
            {
                var parent = Repository.GetElementByID(smState.ParentId);
                if(parent != null && parent.Type != "State")
                {
                    smState.ParentId = 0;
                }
            }

            smState.Notes = eaState.Notes;

            var statesConnectors = eaState.Connectors;
            foreach (EA.Connector connector in statesConnectors)
            {
                if (!isConnectorDirectionOutOfNode(connector, eaState))
                {
                    continue;
                }
                else
                {
                    EA.Element eaNextElem = safelyGetElement(connector.SupplierID);
                    if (eaNextElem == null)
                    {
                        continue;
                    }

                    if (elementIsState(eaNextElem))
                    {
                        collectTransitions(smState, connector);
                    }
                    else if (isChoiceNode(eaNextElem))
                    {

                        if (connector.TransitionEvent.Trim() == string.Empty)
                        {
                            MessageLogger.logError(ErrorMessageFormatIndex.NotPermitToNotContainEventsBeforeChoiceNode, eaState.Name.ToUpper(), eaNextElem.Name.ToUpper());
                        }
                        else
                        {
                            if (connector.TransitionAction.Trim() != string.Empty || connector.TransitionGuard.Trim() != string.Empty)
                            {
                                MessageLogger.logError(ErrorMessageFormatIndex.MustNotContainGuardBeforeChoiceNode,
                                    eaState.Name.ToUpper(), eaNextElem.Name.ToUpper(),
                                    connector.TransitionAction.ToUpper() + "|" + connector.TransitionGuard.ToUpper());
                            }
                            collectTransitionOnChoice(smState, connector, eaNextElem);
                        }
                    }
                    else
                    {
                        //ErrorLogger.log() the connector on elemment must be state or choice
                    }
                }
            }

            collectStateBehaviours(eaState, smState);
        }

        protected void collectTransitionOnChoice(ARSourceGeneration.DState smState, EA.Connector ctorIn, EA.Element eaChoiceNode)
        {
            var connectors = eaChoiceNode.Connectors;
            string conditionOfChoiceNode = "";
            if (eaChoiceNode.Name.ToLower() != "choice")
            {
                conditionOfChoiceNode = eaChoiceNode.Name;
            }

            List<string> listConditions = new List<string>();
            EA.Connector connectorHasElseCondition = null;
            for (short i = 0; i < connectors.Count; ++i)
            {
                EA.Connector ctorOut = connectors.GetAt(i);
                if (!isConnectorDirectionOutOfNode(ctorOut, eaChoiceNode))
                {
                    continue;
                }
                else
                {
                    EA.Element eaNextNode = safelyGetElement(ctorOut.SupplierID);
                    if (eaNextNode == null)
                    {
                        continue;
                    }
                    if (!elementIsState(eaNextNode))
                    {
                        MessageLogger.logError(ErrorMessageFormatIndex.ConnectionFromChoiceNodeMustToStateNode,
                            eaChoiceNode.Name.ToUpper(),
                            eaNextNode.Name.ToUpper());
                    }
                    else if (ctorOut.TransitionEvent.Trim() != string.Empty)
                    {
                        MessageLogger.logError(ErrorMessageFormatIndex.MustNotContainEventsAfterChoiceNode,
                            eaChoiceNode.Name.ToUpper(),
                            eaNextNode.Name.ToUpper(), ctorOut.TransitionEvent.ToUpper());
                    }
                    else
                    {
                        if (conditionOfChoiceNode != string.Empty) //Choice node has not contain conditions
                        {
                            collectTransitions(smState, ctorIn, ctorOut, conditionOfChoiceNode);
                        }
                        else
                        {
                            if (ctorOut.TransitionGuard.ToLower() == "else")
                            {
                                connectorHasElseCondition = ctorOut;
                            }
                            else
                            {
                                listConditions.Add(ctorOut.TransitionGuard);
                                collectTransitions(smState, ctorIn, ctorOut);
                            }
                        }
                    }
                }
            }

            if (connectorHasElseCondition != null)
            {
                string conditionForElse = "";
                for (int i = 0; i < listConditions.Count; ++i)
                {
                    conditionForElse += listConditions[i] + " && ";
                }
                if (conditionForElse != string.Empty)
                {
                    conditionForElse = string.Format("NOT ({0})", conditionForElse);
                }

                smState.addTransition(ctorIn.TransitionEvent, conditionForElse, connectorHasElseCondition.TransitionAction, connectorHasElseCondition.SupplierID);
            }
        }

        protected void collectTransitions(ARSourceGeneration.DState smState, EA.Connector ctor1, EA.Connector ctor2 = null, string conditionOnChoice = "")
        {
            //update Project data
            EA.Element eaNextState;
            string actions = "";
            string conditions = "";
            string triggers = ctor1.TransitionEvent;

            if (ctor2 != null)
            {

                eaNextState = safelyGetElement(ctor2.SupplierID);
                if (ctor2.TransitionEvent.Trim() != string.Empty)
                {
                    EA.Element eaChoiceNode = safelyGetElement(ctor2.ClientID);
                    MessageLogger.logError(ErrorMessageFormatIndex.MustNotContainEventsAfterChoiceNode,
                        eaChoiceNode.Name,
                        eaNextState.Name, 
                        ctor2.TransitionEvent);
                }

                actions = ctor2.TransitionAction;
                conditions = conditionOnChoice == string.Empty ? ctor2.TransitionGuard : string.Format("{0} -->{1}", conditionOnChoice, ctor2.TransitionGuard);
            }
            else
            {
                eaNextState = safelyGetElement(ctor1.SupplierID);
                actions = ctor1.TransitionAction;
                conditions = ctor1.TransitionGuard;
            }


            Debug.Assert(eaNextState != null);

            smState.addTransition(triggers, conditions, actions, eaNextState.ElementID);
        }


        private bool elementIsState(EA.Element e)
        {
            if (e == null)
                return false;
            else
            {
                return e.Type == "State";
            }
        }


        private bool hasDuplicateConditionInConnectors(EA.Element eaChoice, EA.Collection connectors)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            bool hasDupplicate = false;
            for (short i = 0; i < connectors.Count; ++i)
            {
                EA.Connector ctor = connectors.GetAt(i);
                if (!isConnectorDirectionOutOfNode(ctor, eaChoice) || !elementIsState(safelyGetElement(ctor.SupplierID)))
                {
                    continue;
                }
                string conditions = ctor.TransitionGuard.Trim();
                if (dic.ContainsKey(conditions))
                {
                    dic[conditions]++;
                }
                else
                {
                    dic.Add(conditions, 1);
                }
            }

            var enumerator = dic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value > 1)
                {
                    hasDupplicate = true;
                }
            }

            return hasDupplicate;
        }


        private bool isConnectorDirectionOutOfNode(EA.Connector connector, EA.Element currentNode)
        {
            EA.Element DestNode = safelyGetElement(connector.SupplierID);
            EA.Element srcNode = safelyGetElement(connector.ClientID);
            return !((srcNode.Name != currentNode.Name) && (DestNode.Name == currentNode.Name));

        }
        protected void collectStateBehaviours(EA.Element eaState, ARSourceGeneration.DState smState)
        {
            Debug.Assert(eaState != null);
            var methods = eaState.Methods;
            if (methods.Count > 0)
            {
                for (short mt_i = 0; mt_i < methods.Count; ++mt_i)
                {
                    EA.Method mt = methods.GetAt(mt_i);
                    if (mt != null)
                    {
                        smState.addInternalOperation(mt.ReturnType, mt.Name);
                    }
                }
            }

        }

        private void parseDelayedNodes()
        {
            foreach (var eaNode in DelayParsingNodes)
            {
                if (eaNode.Connectors.Count < 1)
                {
                    continue;
                }

                EA.Connector ctor = eaNode.Connectors.GetAt(0);

                EA.Element parentNode = safelyGetElement(eaNode.ParentID);

                if (parentNode != null)
                {
                    ARSourceGeneration.DState smParentState = SourceData.Hsm.getState(parentNode.ElementID);
                    if (smParentState != null)
                    {
                        collectTransitions(smParentState, ctor); //add a default transition
                    }
                }
            }
        }

        private List<EA.Element> DelayParsingNodes
        {
            get
            {
                if (_delayParsingNodes == null)
                {
                    _delayParsingNodes = new List<EA.Element>();
                }
                return _delayParsingNodes;
            }
        }

        private List<EA.Element> _delayParsingNodes;
    }

}
