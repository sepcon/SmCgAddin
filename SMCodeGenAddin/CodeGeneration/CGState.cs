using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSourceGeneration
{
    public class CGState : CGCodeGeneratorBase
    {

        public CGState(DState state, CGHsm hsm)
        {
            Data = state;
            _hsm = hsm;
        }

        protected override string getFormat()
        {
            return Data.isRootState() ?
                FormatGetter.get(SourceCodeFormatIndex.StateTableRoot)
                : FormatGetter.get(SourceCodeFormatIndex.StateTableNorm);
        }

        protected override string[] getFormatArguments()
        {
            Data.updateEntryExitFunction();
            string transitionTableCode = "";
            List<CGCodeGeneratorBase> listOperation = new List<CGCodeGeneratorBase>();
            foreach (var operation in Data.OperationList)
            {
                var operationCodeSnippet = getOperationCodeGenerator(operation);
                listOperation.Add(operationCodeSnippet);
            }

            foreach (CGCodeGeneratorBase operationCodeSnip in listOperation)
            {
                transitionTableCode += operationCodeSnip.constructSource();
            }

            if (Data.isRootState())
            {
                return new string[] { Data.Name, transitionTableCode };
            }
            else
            {
                return new string[] { Data.Name, Data.getParentStateName(), transitionTableCode };
            }
        }

        protected CGCodeGeneratorBase getOperationCodeGenerator(DStateOperationBase operation)
        {
            CGCodeGeneratorBase generator;
            switch (operation.getOperationType())
            {
                case DStateOperationBase.OperationType.Transition:
                    generator = new CGStateTranstion((DStateTransition)operation, this);
                    break;
                case DStateOperationBase.OperationType.InternalOperation:
                    generator = new CGStateInnerOperation((DStateInternalOperation)operation, this);
                    break;
                default:
                    generator = null;
                    break;
            }
            return generator;
        }

        public DState Data
        {
            get { return _data; }
            set { _data = value; }
        }

        private CGHsm _hsm;

        public CGHsm Hsm
        {
            get { return _hsm; }
        }
        private DState _data;

    }

}
