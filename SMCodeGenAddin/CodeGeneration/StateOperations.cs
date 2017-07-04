using System;
using System.Collections.Generic;
using System.Text;

namespace ARSourceGeneration
{
    public class CGStateTranstion : CGStateOperationBase
    {
        public CGStateTranstion(DStateTransition trans, CGState state)
            : base(state)
        {
            _data = trans;
            _transFuncName = FuncNamer.createTransitionFuncName(_data);
            _condFuncName = FuncNamer.createConditionFuncName(_data);
        }

        /// <summary>
        /// prevent to create code snippet wihout Transition instance
        /// </summary>
        /// <param name="ev"></param>
        /// <param name="conditionFunction"></param>
        /// <param name="actionFunction"></param>
        /// <param name="nextState"></param>
        private CGStateTranstion(string ev = "", string conditionFunction = "", string actionFunction = "", string nextState = "") : base(null) { }
        protected override string getFormat()
        {
            if (_data.isDefaultTransition())
            {
                return FormatGetter.get(SourceCodeFormatIndex.DefaultTrans);
            }
            else if (_data.isInternalTransition())
            {
                return FormatGetter.get(SourceCodeFormatIndex.InternalTrans);
            }
            else
            {
                return FormatGetter.get(SourceCodeFormatIndex.NormalTransition);
            }
        }

        protected override string[] getFormatArguments()
        {

            if (_data.isInternalTransition()) // check that 
            {
                return new string[] {
                    this.formatTextInColumn(_data.Event, DStateTransition.MaxEventNameLength + 2), //+2 because INTERNAL.length + 2 = TRASITION.length
                    this.formatTextInColumn(_condFuncName, FuncNamer.MaxConditionFunctionNameLength),
                    this.formatTextInColumn(_transFuncName, FuncNamer.MaxTransitionFunctionNameLength)
                };
            }
            else if (_data.isDefaultTransition())
            {
                return new string[] { _data.NextStateName };
            }
            else
            {
                return new string[] {
                    this.formatTextInColumn(_data.Event, DStateTransition.MaxEventNameLength),
                    this.formatTextInColumn(_condFuncName, FuncNamer.MaxConditionFunctionNameLength),
                    this.formatTextInColumn(_transFuncName, FuncNamer.MaxTransitionFunctionNameLength),
                    _data.NextStateName};
            }
        }

        private string _transFuncName;
        private string _condFuncName;
        private DStateTransition _data;
    }

    public class CGStateInnerOperation : CGStateOperationBase
    {
        public CGStateInnerOperation(DStateInternalOperation operation, CGState state)
            : base(state)
        {
            _data = operation;
            _funcName = FuncNamer.createEntryExitFuncName(_data);
        }

        protected override string getFormat()
        {
            return _data.Type == "entry" ?
                FormatGetter.get(SourceCodeFormatIndex.Entry) : FormatGetter.get(SourceCodeFormatIndex.Exit);
        }

        protected override string[] getFormatArguments()
        {

            return new string[] { _funcName };
        }

        private string _funcName;
        private DStateInternalOperation _data;
    }

}
