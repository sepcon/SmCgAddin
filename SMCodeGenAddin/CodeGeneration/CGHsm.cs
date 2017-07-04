using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ARSourceGeneration
{
    public class CGHsm : CGCodeGeneratorBase
    {
        public CGHsm(DHsm hsm, CGSource cgSource)
        {
            _data = hsm;
            _cgSource = cgSource;
        }

        protected override string getFormat()
        {
            return FormatGetter.get(SourceCodeFormatIndex.Hsm);
        }

        protected override string[] getFormatArguments()
        {
            Debug.Assert(Data.HsmName != "" && Data.StateList.Count > 0);
            DState root = getRootState();
            Debug.Assert(root != null, "State machine must have one root state");
            string hsmContentCodeSnp = (root == null) ? "" : (new CGState(root, this)).constructSource();
            foreach (DState state in Data.StateList)
            {
                if (state.isRootState())
                {
                    continue;
                }
                CGState stateCodeSnp = new CGState(state, this);
                hsmContentCodeSnp += stateCodeSnp.constructSource();
            }
            return new string[] { Data.HsmName, hsmContentCodeSnp };
        }

        protected DState getRootState()
        {
            return Data.StateList.Find(state => state.isRootState());
        }

        private DHsm _data;

        public DHsm Data
        {
            get { return _data; }
        }

        private CGSource _cgSource;

        public CGSource CgSource
        {
            get { return _cgSource; }
        }
    }

}
