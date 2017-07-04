using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;


namespace ARSourceGeneration
{
    public class FunctionNamerSimpleTransAndConds : IFunctionNamer
    {
        public string createConditionFuncName(DStateTransition trans)
        {
            Debug.Assert(trans != null);
            string funcName;
            if (trans.Conditions == string.Empty)
            {
                funcName = FormatGetter.get(SourceCodeFormatIndex.NoCondition);
            }
            else
            {
                string conditions = trans.Conditions.Replace(" ", "");
                if (_conditionFunctionNameMap.ContainsKey(conditions))
                {
                    funcName = _conditionFunctionNameMap[conditions];
                }
                else
                {
                    int indexOfEquationSign = conditions.IndexOf("==");
                    if(indexOfEquationSign != -1)
                    {
                        string before = conditions.Substring(0, indexOfEquationSign).Trim();
                        string after = conditions.Substring(indexOfEquationSign + 2).Trim();
                        funcName = string.Format(FormatGetter.get(SourceCodeFormatIndex.ConditionFuncName), trans.ComponentName, "_Is_" + before + "_" + after);
                    }
                    else
                    {
                        funcName = string.Format(FormatGetter.get(SourceCodeFormatIndex.ConditionFuncName), trans.ComponentName, conditions.Replace("\n", ""));
                    }

                    checkFunctionNameError(funcName);
                    _conditionFunctionNameMap.Add(conditions, funcName);
                }
            }

            updateMaxLengt(funcName, ref _maxCondFuncNameLength);

            return funcName;

        }

        public string createTransitionFuncName(DStateTransition trans)
        {
            Debug.Assert(trans != null);
            string funcName;
            if(trans.Actions == string.Empty)
            {
                funcName = FormatGetter.get(SourceCodeFormatIndex.NoAction);
            }
            else
            {
                string Actions = trans.Actions.Replace(" ", "");
                if (_transitionFunctionNameMap.ContainsKey(Actions))
                {
                    funcName = _transitionFunctionNameMap[Actions];
                }
                else
                {
                    int indexOfEquationSign = Actions.IndexOf('=');
                    if (indexOfEquationSign != -1)
                    {
                        string before = Actions.Substring(0, indexOfEquationSign).Trim();
                        string after = Actions.Substring(indexOfEquationSign + 1).Trim();
                        funcName = string.Format(FormatGetter.get(SourceCodeFormatIndex.ActionFuncName), trans.ComponentName, "Set_" + before + "_To_" + after);
                    }
                    else
                    {
                        funcName = string.Format(FormatGetter.get(SourceCodeFormatIndex.ActionFuncName), trans.ComponentName, Actions.Replace("\n", ""));
                    }
                    checkFunctionNameError(funcName);
                    _transitionFunctionNameMap.Add(Actions, funcName);
                }
            }
            
            updateMaxLengt(funcName, ref _maxTransFuncNameLength);

            return funcName;
        }

        public string createEntryExitFuncName(DStateInternalOperation dOperation)
        {
            Debug.Assert(dOperation != null);
            string funcName = "";
            string Operations = dOperation.Operations.Replace(" ", "").Trim();
            SortedDictionary<string, string> functionNameMap = dOperation.Type.ToLower() == "entry" ? _entryFunctionNameMap : _exitFunctionNameMap;
            string functionNameFormat = FormatGetter.get(SourceCodeFormatIndex.EntryExitFuncName);

            if (functionNameMap.ContainsKey(Operations))
            {
                funcName = functionNameMap[Operations];
            }
            else
            {
                int indexOfEquationSign = Operations.IndexOf('=');
                if (indexOfEquationSign != -1)
                {
                    string before = Operations.Substring(0, indexOfEquationSign).Trim();
                    string after = Operations.Substring(indexOfEquationSign + 1).Trim();
                    funcName = string.Format(functionNameFormat, dOperation.ComponentName, dOperation.Type, "Set_" + before + "_To_" + after);
                }
                else
                {
                    funcName = string.Format(functionNameFormat, dOperation.ComponentName, dOperation.Type, Operations);
                }

                checkFunctionNameError(funcName);
                functionNameMap.Add(Operations, funcName);
            }

            return funcName;
        }


        public SortedDictionary<string, string> getMapTransFuncNameAnOperations()
        {
            return getMapFunctionAndItsContent(_transitionFunctionNameMap);
        }
        public SortedDictionary<string, string> getMapCondFuncNameAndOperations()
        {
            return getMapFunctionAndItsContent(_conditionFunctionNameMap);
        }
        public SortedDictionary<string, string> getMapEntryFuncNameAndOperations()
        {
            return getMapFunctionAndItsContent(_entryFunctionNameMap);
        }
        public SortedDictionary<string, string> getMapExitFuncNameAndOperations()
        {
            return getMapFunctionAndItsContent(_exitFunctionNameMap);
        }
        public int MaxConditionFunctionNameLength { get { return _maxCondFuncNameLength; } }
        public int MaxTransitionFunctionNameLength { get { return _maxTransFuncNameLength; } }
        public string[] ListTransitionFunctions 
        { 
            get 
            {
                return getListFunctions(_transitionFunctionNameMap); 
            } 
        }
        public string[] ListConditionFunctions
        {
            get
            {
                return getListFunctions(_conditionFunctionNameMap);
            }
        }

        private string[] getListFunctions(SortedDictionary<string, string> functionNameMap)
        {
            string[] listFunction = new string[functionNameMap.Values.Count];
            functionNameMap.Values.CopyTo(listFunction, 0);
                return listFunction;
        }

        public string[] ListEntryFunctions { get { return getListFunctions(_entryFunctionNameMap); } }

        public string[] ListExitFunctions { get { return getListFunctions(_exitFunctionNameMap); } }

        private SortedDictionary<string, string> getMapFunctionAndItsContent(SortedDictionary<string, string> functionNameMap)
        {
            SortedDictionary<string, string> map = new SortedDictionary<string, string>();
            foreach(var pair in functionNameMap)
            {
                try
                {
                    map.Add(pair.Value, pair.Key);
                }
                catch{}
            }
            return map;
        }

        private bool isAValidFunctionName(string functionName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(functionName, _regexFunctionNameTemplate);
            return match.Length == functionName.Length;
        }

        private void checkFunctionNameError(string functionName)
        {
            if(!isAValidFunctionName(functionName))
            {
                throw new Exception("there some complex expresions in transition conditions and transition actions\n" +
                "please use \"Generate Source with function naming by numbering\"");
            }
        }
        private void updateMaxLengt(string str, ref int maxStrLength)
        {
            if(str.Length > maxStrLength)
            {
                maxStrLength = str.Length;
            }
        }
        private int _maxCondFuncNameLength = 0;
        private int _maxTransFuncNameLength = 0;

        private SortedDictionary<string, string> _entryFunctionNameMap = new SortedDictionary<string, string>();
        private SortedDictionary<string, string> _exitFunctionNameMap = new SortedDictionary<string, string>();

        private SortedDictionary<string, string> _conditionFunctionNameMap = new SortedDictionary<string,string>();
        private SortedDictionary<string, string> _transitionFunctionNameMap = new SortedDictionary<string, string>();
        private const string _regexFunctionNameTemplate = @"[_a-zA-Z]+[\w]*";
    }

}
