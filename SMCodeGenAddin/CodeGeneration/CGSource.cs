using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ARSourceGeneration
{
    public enum SourceFileMode
    {
        Header,
        Source,
        Notes
    }
    public class CGSource : CGCodeGeneratorBase
    {
        public CGSource(DSource data, FunctionNamerType fnType = FunctionNamerType.Numbering)
        {
            this.Data = data;
            SourceMode = SourceFileMode.Header;
            this._funcNamer = FunctionNamerFactory.createFunctionNamer(fnType);
        }

        public string getFileName()
        {
            switch(SourceMode)
            {
                case SourceFileMode.Header:
                    return createHeaderFileName ();
                case SourceFileMode.Source:
                    return createSourceFileName();
                case SourceFileMode.Notes:
                    return Data.ComponentName + "_States.html";
                default:
                    throw new Exception("can not detemine file type");;
            }
        }


        public string getAllNotes()
        {
            int stringCapacity = Data.DiagramNotes.Length + Data.Hsm.StateList.Count * Data.DiagramNotes.Length;
            StringBuilder noteBuilder = new StringBuilder(stringCapacity);
            noteBuilder.Append("<h1>" + Data.ComponentName + "</h1>" + Data.DiagramNotes);
            for(int i = 0; i < Data.Hsm.StateList.Count; ++i)
            {
                var state = Data.Hsm.StateList[i];
                if(state.Notes == string.Empty)
                {
                    continue;
                }
                noteBuilder.Append("<h2>" + state.Name + "</h2>" + state.Notes );
            }

            return noteBuilder.ToString();
        }
        protected override string getFormat()
        {
            switch (SourceMode)
            {
                case SourceFileMode.Header:
                    return FormatGetter.get(SourceCodeFormatIndex.HeaderFile);
                case SourceFileMode.Source:
                    return FormatGetter.get(SourceCodeFormatIndex.SourceFile);
                case SourceFileMode.Notes:
                    return "{0}";
                default:
                    throw new Exception("can not detemine file type");
            }
           
        }
        
        protected override string[] getFormatArguments()
        {
            switch (SourceMode)
            {
                case SourceFileMode.Header:
                    return getFormatArgsForCreatingHeaderFile();
                case SourceFileMode.Source:
                    return getFormatArgsForCreatingSourceFile();
                case SourceFileMode.Notes:
                    return new string[] { getAllNotes() };
                default:
                    throw new Exception("can not detemine file type");
            }
        }
        private string[] getFormatArgsForCreatingHeaderFile()
        {
            string headerFile = createHeaderFileName();
            string includeGuard = headerFile.ToUpper().Replace('.', '_');
            string HsmTable = (new CGHsm(Data.Hsm, this)).constructSource();
            string sourceInsideIncludeGuard = generateEventEnum(Data.ComponentName) + ConstantValues.NEW_LINE + generateFunctionsPrototype();
            return new string[] { Data.ComponentName, headerFile, includeGuard, sourceInsideIncludeGuard, HsmTable};
        }

        private string[] getFormatArgsForCreatingSourceFile()
        {
            return new string[] { Data.ComponentName, createHeaderFileName(), genAllFunctionsDefinition() };
        }

        private string createHeaderFileName()
        {
            return Data.ComponentName + "_States.h";
        }

        private string createSourceFileName()
        {
            return Data.ComponentName + "_States.c";
        }
        private string generateEventEnum(string enumName)
        {
            StringBuilder enumValues = new StringBuilder(Data.EventSet.Count * 30);
            if (Data.EventSet.Count > 0)
            {
                var eventList = Data.EventSet.GetEnumerator();

                eventList.MoveNext();
                enumValues.Append(ConstantValues.TAB_VALUE + eventList.Current + " = " + FormatGetter.get(SourceCodeFormatIndex.Ev_user) + ",\n");

                while(eventList.MoveNext())
                {
                    enumValues.Append(ConstantValues.TAB_VALUE + eventList.Current + ",\n");
                }
            }

            return constructSource(FormatGetter.get(SourceCodeFormatIndex.Enum), enumName, enumValues.ToString());
        }

        private string generateFunctionsPrototype()
        {
            string[] listConditionFuncName = FuncNamer.ListConditionFunctions;
            string[] listTransitionFuncName = FuncNamer.ListTransitionFunctions;
            string[] listEntryFuncName = FuncNamer.ListEntryFunctions;
            string[] listExitFunctionName = FuncNamer.ListExitFunctions;

            StringBuilder funcPrototypes = new StringBuilder((listConditionFuncName.Length + listTransitionFuncName.Length + listEntryFuncName.Length) * 100);

            foreach (var eextFuncName in listEntryFuncName)
            {
                funcPrototypes.Append(genTransFuncPrototype(eextFuncName));
            }

            foreach (var eextFuncName in listExitFunctionName)
            {
                funcPrototypes.Append(genTransFuncPrototype(eextFuncName));
            }



            funcPrototypes.Append(ConstantValues.NEW_LINE + ConstantValues.NEW_LINE);


            foreach (var condFuncName in listConditionFuncName)
            {
                funcPrototypes.Append( genCondFucnPrototype(condFuncName));
            }

            funcPrototypes.Append(ConstantValues.NEW_LINE + ConstantValues.NEW_LINE);


            foreach (var transFuncName in listTransitionFuncName)
            {
                funcPrototypes.Append(genTransFuncPrototype(transFuncName));
            }

            return funcPrototypes.Append(ConstantValues.NEW_LINE + ConstantValues.NEW_LINE).ToString();

        }

        private string genAllFunctionsDefinition()
        {
            
            var mapCondFuncNameAndOperations = FuncNamer.getMapCondFuncNameAndOperations();
            var mapTransFuncNameAndOperations = FuncNamer.getMapTransFuncNameAnOperations();
            var mapEntryFunctionAndOperations = FuncNamer.getMapEntryFuncNameAndOperations();
            var mapExitFunctionAndOperations = FuncNamer.getMapExitFuncNameAndOperations();
            StringBuilder functionsBody = new StringBuilder((
                mapCondFuncNameAndOperations.Count + 
                mapTransFuncNameAndOperations.Count +
                mapEntryFunctionAndOperations.Count +
                mapExitFunctionAndOperations.Count) * 400);
            
            
            foreach (var etFuncPair in mapEntryFunctionAndOperations)
            {
                functionsBody.Append(genTransFuncBody(etFuncPair.Key, etFuncPair.Value));
            }

            foreach (var exFuncPair in mapExitFunctionAndOperations)
            {
                functionsBody.Append(genTransFuncBody(exFuncPair.Key, exFuncPair.Value));
            }

            foreach (var condFuncPair in mapCondFuncNameAndOperations)
            {
                functionsBody.Append(genCondFuncBody(condFuncPair.Key, condFuncPair.Value));
            }

            foreach (var transFuncPair in mapTransFuncNameAndOperations)
            {
                functionsBody.Append(genTransFuncBody(transFuncPair.Key, transFuncPair.Value));
            }

            
            return functionsBody.ToString();
        }



        private string genTransFuncBody(string name, string operations)
        {
            return genFunctionBody(name, operations,
                FormatGetter.get(SourceCodeFormatIndex.ActionFuncSignature),
                FormatGetter.get(SourceCodeFormatIndex.ActionFuncBody),
                "void");
        }

        private string genCondFuncBody(string name, string operations)
        {
            return genFunctionBody(name, operations, 
                FormatGetter.get(SourceCodeFormatIndex.ConditionFuncSignature), 
                FormatGetter.get(SourceCodeFormatIndex.ConditionFuncBody), 
                "boolean");
        }

        private string genFunctionBody(string name, string operations, string signatureFm, string bodyFm, string retType)
        {
            string signature = constructSource(signatureFm, name);
            string body = constructSource(bodyFm, operations);
            return constructSource(FormatGetter.get(SourceCodeFormatIndex.FuncWithDoxComment), signature, retType, body);
        }
        private string genCondFucnPrototype(string name)
        {
            return constructSource(FormatGetter.get(SourceCodeFormatIndex.ConditionFuncSignature), name) + ";" + ConstantValues.NEW_LINE;
        }

        private string genTransFuncPrototype(string name)
        {
            return constructSource(FormatGetter.get(SourceCodeFormatIndex.ActionFuncSignature), name) + ";" + ConstantValues.NEW_LINE;
        }


        public DSource Data
        {
            get { return _data; }
            set { _data = value; }
        }
        public SourceFileMode SourceMode
        {
            get { return _sourceMode; }
            set { _sourceMode = value; }
        }

        public IFunctionNamer FuncNamer
        {
            get { return _funcNamer; }
        }

        private DSource _data;
        private SourceFileMode _sourceMode;
        IFunctionNamer _funcNamer;

        
    }
}
