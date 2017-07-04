using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ARSourceGeneration
{
    public abstract class DStateOperationBase : IComparable<DStateOperationBase>
    {
        public enum OperationType
        {
            Null,
            Transition,
            InternalOperation
        }

        public DStateOperationBase(DState state = null)
        {
            Debug.Assert(state != null);
            CurrentState = state;
        }
        public abstract OperationType getOperationType();
        public abstract int CompareTo(DStateOperationBase other);

        public IFunctionNamer FuncNamer
        {
            get { return CurrentState.Hsm.PrjData.FuncNamer;}
        }

        public SortedSet<string> EventSet
        {
            get { return CurrentState.Hsm.PrjData.EventSet; }
        }

        public string ComponentName
        {
            get { return CurrentState.Hsm.PrjData.ComponentName; }
        }
        private DState _currentState;
        public DState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
        
    }
    public class DStateTransition : DStateOperationBase
    {
        /// <summary>
        /// normal transition
        /// </summary>
        /// <param name="ev">trigger event</param>
        /// <param name="currentState">source state</param>
        /// <param name="nextStateName">destination state</param>
        /// <param name="conditions">guard</param>
        /// <param name="actions">action</param>
        public DStateTransition(string ev, string conditions, string actions, DState state, int nextStateId, string nextStateName = "") 
            : base(state)
        {
            this.Event = ev;
            this.NextStateId = nextStateId;
            this.Conditions = conditions;
            this.Actions = actions;
            this.CurrentState = state;
            this.NextStateName = nextStateName;
        }

        private DStateTransition(){}
        
        /// <summary>
        /// implement the method of interface IComparable
        /// this method using for sorting Transition in State table declaration in autosar statemachine framework
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override int CompareTo(DStateOperationBase other)
        {
            int ret = 0;
            if(other.getOperationType() == OperationType.InternalOperation)
            {
                ret = 1;
            }
            else if(other.getOperationType() == this.getOperationType())
            {
                DStateTransition otherTrans = (DStateTransition)other;
                if(otherTrans.isDefaultTransition() && this.isDefaultTransition())
                {
                    ret = this.NextStateName.CompareTo(otherTrans.NextStateName);
                }
                else if( this.isDefaultTransition() )
                {
                    ret = -1;
                }
                else
                {
                    ret = 1;
                }
            }

            return ret;
        }
        public override DStateOperationBase.OperationType getOperationType()
        {
            return OperationType.Transition;
        }
        public bool isInternalTransition()
        {
            return NextStateName.Equals(CurrentState.Name);
        }

        public bool isDefaultTransition()
        {
            return (this.Event.Length == 0); //tbd: need to check relationship between CurrentState and NextState, they should be parent-sub tie
        }
       
        /*--------class attributes section-------------*/
        /// <summary>
        /// for format the state table in column
        /// </summary>
        private static int _maxEventNameLength = 0;

        public static int MaxEventNameLength
        {
            get { return DStateTransition._maxEventNameLength; }
            set { DStateTransition._maxEventNameLength = value; }
        }
    
       
        private string _event;

        public string Event
        {
            get { return _event; }
            set 
            {
                _event = value;
                if(_event != string.Empty)
                {
                    EventSet.Add(_event);
                    if (_event.Length > DStateTransition.MaxEventNameLength)
                    {
                        DStateTransition.MaxEventNameLength = _event.Length;
                    }
                }
            }
        }


        private int _nextStateId;

        public int NextStateId
        {
            get { return _nextStateId; }
            set { _nextStateId = value; }
        }

        private string _nextStateName;

        private DState _nextState;

        public DState NextState
        {
            get
            {
                if(_nextState == null)
                {
                    _nextState = CurrentState.Hsm.getState(NextStateId);
                }
                return _nextState; 
            }
            set { _nextState = value; }
        }
        public string NextStateName
        {
            get { return _nextStateName != "" ? _nextStateName : (NextState != null ? NextState.Name : ""); }
            set { _nextStateName = value; }
        }
    
        private string _conditions;
        public string Conditions
        {
            get { return _conditions; }
            set
            {
                _conditions = value;
            }
        }

        private string _actions;
        public string Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }

    }

    public class DStateInternalOperation : DStateOperationBase
    {
        
        public DStateInternalOperation(string type, string actions, DState state) : base(state)
        {
            this.Type = type;
            _actions = actions;
        }

        private DStateInternalOperation()
        {
        }

        public override DStateOperationBase.OperationType getOperationType()
        {
            return OperationType.InternalOperation;
        }
    
        public override int CompareTo(DStateOperationBase other)
        {
            int ret = 0;
            if ((this.Type == "entry") || (other.getOperationType() == OperationType.Transition))
            {
                ret = -1;
            }
            else
            {
                ret = 1;
            }

            return ret;
        }


        private string _type;
        private string _actions;
        public string Operations
        {
            get { return _actions; }
            set { _actions = value; }
        }
        

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

    }

    public class DState
    {
        
        public DState(DHsm hsm, int id, string stateName, int parentId = 0)
        {
            this.Hsm = hsm;
            this.Name = stateName;
            this.Id = id;
            this.ParentId = parentId;
            this.OperationList = new SortedSet<DStateOperationBase>();
            Hsm.addState(this);
        }
        private DState()
        {
        }

        public void addOperation(DStateOperationBase operation)
        {
            this._operationList.Add(operation);
        }

        public void addInternalOperation(string type, string actions)
        {
            if(OneEntryExitPerState)
            {
                if(type == "entry")
                {
                    _entryOperations += actions + ConstantValues.NEW_LINE;
                }
                else
                {
                    _exitOperations += actions + ConstantValues.NEW_LINE;
                }
            }
            else
            {
                DStateInternalOperation operation = new DStateInternalOperation(type, actions, this);
                this.addOperation(operation);
            }

        }

        public void addDefaultTransition(int defaultStateId, string defaltStateName = "")
        {
            this.addOperation(new DStateTransition("", "", "", this, defaultStateId, defaltStateName));
        }
        public void addTransition(string triggers, string conditions, string actions, int nextStateId, string nextStateName = "")
        {
            if (triggers.Contains(","))
            {
                string[] eventList = triggers.Split(',', ' ');
                foreach (string ev in eventList)
                {
                    if (ev != string.Empty)
                    {
                        DStateTransition smTransition = new DStateTransition(ev, conditions, actions, this, nextStateId, nextStateName);
                        this.addOperation(smTransition);
                    }
                }
            }
            else
            {
                DStateTransition smTransition = new DStateTransition(triggers, conditions, actions, this, nextStateId, nextStateName);
                this.addOperation(smTransition);
            }
        }


        public bool isRootState()
        {
            return ParentId == 0 || ParentId == Id;
        }

        public string getParentStateName()
        {
            if(ParentId == 0)
            {
                return "";
            }
            else
            {
                DState parent = Hsm.getState(ParentId);
                return parent.Name;
            }
            
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void updateEntryExitFunction()
        {
            if (_entryOperations != string.Empty)
            {
                DStateInternalOperation entryOperation = new DStateInternalOperation("entry", _entryOperations, this);
                addOperation(entryOperation);
            }
            if(_exitOperations != string.Empty)
            {
                DStateInternalOperation exitOperation = new DStateInternalOperation("exit", _exitOperations, this);
                addOperation(exitOperation);
            }
        }
        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public DHsm Hsm { get { return _hsm; } set { _hsm = value; } }

        public SortedSet<DStateOperationBase> OperationList
        {
            get { return _operationList; }
            set { _operationList = value; }
        }

        public static bool OneEntryExitPerState
        {
            get { return DState._oneEntryExitPerState; }
            set { DState._oneEntryExitPerState = value; }
        }

        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private static bool _oneEntryExitPerState = true;
        private SortedSet<DStateOperationBase> _operationList = new SortedSet<DStateOperationBase>();
        private string _entryOperations = "";
        private string _exitOperations = "";
        private int _id;
        private string _name;
        private string _notes = "";
        private int _parentId = int.MaxValue;
        private DHsm _hsm;

    }


    public class DHsm 
    {
        public DHsm(DSource prjData)
        {
            this._prjData = prjData;
        }

        public void cleanData()
        {
            this._hsmName = "";
            this._stateList.Clear();
        }

        public DState addState(int stateId, string stateName, int parentStateId)
        {
            DState state = getState(stateId);
            if(state != null)
            {
                return state;
            }
            else
            {
                return new DState(this, stateId, stateName, parentStateId);
            }
        }
        public void addState(DState state)
        {
            _stateList.Add(state);
        }

        public DState getState(int id)
        {
            return _stateList.Find(state => state.Id == id);
        }
        public string HsmName
        {
            get { return _hsmName; }
            set { _hsmName = value; }
        }


        public List<DState> StateList
        {
            get { return _stateList; }
            set { _stateList = value; }
        }

        public DSource PrjData
        {
            get { return _prjData; }
        }


        string _hsmName;
        DSource _prjData;
        List<DState> _stateList = new List<DState>();

    }

    public class DSource
    {
        public DSource()
        {
            Hsm = new DHsm(this);
            _eventSet = new SortedSet<string>();
            _conditionFunctionNameList = new Dictionary<string, string>();
            _actionFunctionNameList = new Dictionary<string, string>();
        }

        
        public void addEvent(string eventName)
        {
            this._eventSet.Add(eventName);
        }

        public void addEvent(string[] eventList)
        {
            foreach(var ev in eventList)
            {
                if(ev.Length != 0)
                {
                    addEvent(ev);
                }
            }
        }

        /// <summary>
        /// create functionName by format, and store for generate code later
        /// </summary>
        /// <param name="name">name of action function</param>
        /// <param name="actions">list of action to be implemented in function body</param>
        /// <returns>the function name that was formated</returns>
        public void addActionFunction(string name, string actions)
        {
            if(name != string.Empty && actions != string.Empty)
            {
                this._actionFunctionNameList[actions.Replace("\n", "\n\t//")] = name;
            }
        }
        public void addConditinoFunction(string name, string conditions)
        {
            if (name != string.Empty && conditions != string.Empty)
            {
                this._conditionFunctionNameList[conditions.Replace("\n", "\n\t//")] = name;
            }
        }


        public string ComponentName
        {
            get { return _componentName; }
            set { _componentName = value; Hsm.HsmName = _componentName; }
        }

        public IFunctionNamer FuncNamer
        {
            get { return _funcNamer; }
            set { _funcNamer = value; }
        }

        public DHsm Hsm { get { return _hsm; } set { _hsm = value; } }


        public SortedSet<string> EventSet
        {
            get { return _eventSet; }
        }


        public string DiagramNotes
        {
            get { return _diagramNotes; }
            set { _diagramNotes = value; }
        }


        private string _componentName;
        private DHsm _hsm;
        private string _diagramNotes = "";
        private SortedSet<string> _eventSet;
        private Dictionary<string, string> _conditionFunctionNameList;
        private Dictionary<string, string> _actionFunctionNameList;
        private IFunctionNamer _funcNamer;

        
    }
}






