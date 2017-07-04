using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ARSourceGeneration
{
    public class FunctionNamerByNumbering : IFunctionNamer
    {
        public FunctionNamerByNumbering()
        {
            _conditionFunctionsNameMap = new SortedDictionary<ComparableSet<string>, string>();
            _transitionFunctionsNameMap = new SortedDictionary<ComparableSet<string>, string>();
            _entryFunctionsNameMap = new SortedDictionary<ComparableSet<string>, string>();
            _exitFunctionsNameMap = new SortedDictionary<ComparableSet<string>, string>();
            _maxConditionFunctionLength = 0;
            _maxTransitionFunctionLength = 0;
        }
        public string createConditionFuncName(DStateTransition trans)
        {
            Debug.Assert(trans != null);
            string funcName = createFunctionNameOnOperations(
                FormatGetter.get(SourceCodeFormatIndex.ConditionFuncName),
                trans.Conditions,
                trans.ComponentName,
                FormatGetter.get(SourceCodeFormatIndex.NoCondition),
                _conditionFunctionsNameMap
                );

            if (_maxConditionFunctionLength < funcName.Length)
            {
                _maxConditionFunctionLength = funcName.Length;
            }

            return funcName;
        }

        public string createTransitionFuncName(DStateTransition trans)
        {
            Debug.Assert(trans != null);
            string funcName = createFunctionNameOnOperations(
                FormatGetter.get(SourceCodeFormatIndex.ActionFuncName),
                trans.Actions,
                trans.ComponentName,
                FormatGetter.get(SourceCodeFormatIndex.NoAction),
                _transitionFunctionsNameMap
                );

            if (_maxTransitionFunctionLength < funcName.Length)
            {
                _maxTransitionFunctionLength = funcName.Length;
            }

            return funcName;
        }

        public string createFunctionNameOnOperations(string functionFormat, string operations, string componentName, string defaultValue, SortedDictionary<ComparableSet<string>, string> operationsFunctionMap)
        {
            string funcName;
            if (operations.Trim() != string.Empty)
            {
                var conditionList = getOperationsList(operations);
                try
                {
                    funcName = operationsFunctionMap[conditionList];
                }
                catch (KeyNotFoundException)
                {
                    funcName = string.Format(functionFormat, componentName, operationsFunctionMap.Count);
                    operationsFunctionMap.Add(conditionList, funcName);
                }

            }
            else
            {
                funcName = defaultValue;
            }


            return funcName;
        }


        public string createEntryExitFuncName(DStateInternalOperation dOperation)
        {
            Debug.Assert(dOperation != null && dOperation.Operations.Trim() != string.Empty);

            string funcName;
            var operationsFunctionMap = dOperation.Type == "entry" ? _entryFunctionsNameMap : _exitFunctionsNameMap;
            var operationsList = getOperationsList(dOperation.Operations);

            try
            {
                funcName = operationsFunctionMap[operationsList];
            }
            catch (KeyNotFoundException)
            {
                funcName = string.Format(FormatGetter.get(SourceCodeFormatIndex.EntryExitFuncName), dOperation.ComponentName, dOperation.Type, operationsFunctionMap.Count);
                operationsFunctionMap.Add(operationsList, funcName);
            }
            return funcName;
        }


        public SortedDictionary<string, string> getMapTransFuncNameAnOperations()
        {
            return getMapFuncNameAndOperations(_transitionFunctionsNameMap);
        }

        public SortedDictionary<string, string> getMapCondFuncNameAndOperations()
        {
            return getMapFuncNameAndOperations(_conditionFunctionsNameMap);
        }
        public SortedDictionary<string, string> getMapEntryFuncNameAndOperations()
        {
            return getMapFuncNameAndOperations(_entryFunctionsNameMap);
        }

        public SortedDictionary<string, string> getMapExitFuncNameAndOperations()
        {
            return getMapFuncNameAndOperations(_exitFunctionsNameMap);
        }

        private SortedDictionary<string, string> getMapFuncNameAndOperations(SortedDictionary<ComparableSet<string>, string> source)
        {
            SortedDictionary<string, string> ret = new SortedDictionary<string, string>();
            foreach (var pair in source)
            {
                StringBuilder strBuilder;
                int strBdLength = 0;
                string transFuncName = pair.Value;

                foreach (string operation in pair.Key)
                {
                    strBdLength += operation.Length + 1;
                }
                strBuilder = new StringBuilder(strBdLength);

                foreach (string operation in pair.Key)
                {
                    strBuilder.Append("\n\t//" + operation);
                }

                ret.Add(transFuncName, strBuilder.ToString());
            }
            return ret;
        }


        /// <summary>
        /// currently, the actions, conditions got from connector in EA diagram are separated by "\r\n",
        /// </summary>
        /// <param name="opeartions"></param>
        /// <returns></returns>
        private ComparableSet<string> getOperationsList(string opeartions)
        {
            ComparableSet<string> set = new ComparableSet<string>();
            string[] operationArray = opeartions.Split('\r', '\n');
            if (operationArray != null)
            {
                foreach (string operation in operationArray)
                {
                    string operationTrimmed = operation.Trim();
                    if (operationTrimmed != string.Empty)
                    {
                        set.Add(operationTrimmed);
                    }
                }
            }
            return set;
        }

        public string[] ListTransitionFunctions
        {
            get
            {
                return getListFunctionName(_transitionFunctionsNameMap);
            }
        }
        public string[] ListConditionFunctions
        {
            get
            {
                return getListFunctionName(_conditionFunctionsNameMap);
            }
        }
        public string[] ListEntryFunctions
        {
            get
            {
                return getListFunctionName(_entryFunctionsNameMap);
            }
        }

        public string[] ListExitFunctions
        {
            get
            {
                return getListFunctionName(_exitFunctionsNameMap);
            }
        }

        private string[] getListFunctionName(SortedDictionary<ComparableSet<string>, string> functionNameMap)
        {
            string[] ret = new string[functionNameMap.Count];
            functionNameMap.Values.CopyTo(ret, 0);
            return ret;
        }


        public int MaxConditionFunctionNameLength { get { return _maxConditionFunctionLength; } }
        public int MaxTransitionFunctionNameLength { get { return _maxTransitionFunctionLength; } }

        private SortedDictionary<ComparableSet<string>, string> _conditionFunctionsNameMap;
        private SortedDictionary<ComparableSet<string>, string> _transitionFunctionsNameMap;
        private SortedDictionary<ComparableSet<string>, string> _entryFunctionsNameMap;
        private SortedDictionary<ComparableSet<string>, string> _exitFunctionsNameMap;
        private int _maxConditionFunctionLength;
        private int _maxTransitionFunctionLength;
    }

}
