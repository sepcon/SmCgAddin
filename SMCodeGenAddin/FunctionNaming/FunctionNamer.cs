using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace ARSourceGeneration
{

    //if someone need to have a nicely naming for being generated funtions,
    //define new Types here and implement the FunctionNamerInterface, then update 
    public enum FunctionNamerType
    {
        Numbering,
        SimpleTransAndConds
    }


    public static class FunctionNamerFactory
    {
        public static IFunctionNamer createFunctionNamer(FunctionNamerType type)
        {
            switch(type)
            {
                case FunctionNamerType.Numbering:
                    return new FunctionNamerByNumbering();

                case FunctionNamerType.SimpleTransAndConds:
                    return new FunctionNamerSimpleTransAndConds();

                default:
                    return new FunctionNamerByNumbering();
            }
        }
    }
    public interface IFunctionNamer
    {

        string createConditionFuncName(DStateTransition trans);
        string createTransitionFuncName(DStateTransition trans);
        string createEntryExitFuncName(DStateInternalOperation dOperation);

        SortedDictionary<string, string> getMapTransFuncNameAnOperations();
        SortedDictionary<string, string> getMapCondFuncNameAndOperations();
        SortedDictionary<string, string> getMapEntryFuncNameAndOperations();
        SortedDictionary<string, string> getMapExitFuncNameAndOperations();
        int MaxConditionFunctionNameLength { get; }
        int MaxTransitionFunctionNameLength { get; }
        string[] ListTransitionFunctions { get; }
        string[] ListConditionFunctions { get; }
        string[] ListEntryFunctions { get; }
        string[] ListExitFunctions { get; }

    }


   
 
  }