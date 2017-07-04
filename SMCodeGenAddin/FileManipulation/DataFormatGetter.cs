using System;
using ARAddinDeploy;

namespace ARSourceGeneration
{
    public static class ConstantValues
    {
        public const string TAB_VALUE = "\t";
        public const string NEW_LINE = "\n";
    }

    public enum ErrorMessageFormatIndex : int
    {
        NoDiagramToParse,
        NotPermitToNotContainEventsBeforeChoiceNode,
        MustNotContainGuardBeforeChoiceNode,
        NotPermitToMissActionsAndConditionAfterChoiceNode,
        MustNotContainEventsAfterChoiceNode,
        ConnectionFromChoiceNodeMustToStateNode,
        GeneratingCodeSuccessful,
        GeneratingCodeFailed,
        ConnectorError
        //define more others here
    }
    public enum SourceCodeFormatIndex : int
    {
        NoCondition,
        NoAction,
        Ev_user,
        InternalTrans,                  //{0} - Event, {1} - ConditionFunction, {2} - ActionFunction
        Entry,                          //{0} - EntryFunction
        Exit,                           //{0} - ExitFunction
        DefaultTrans,                   //{0} - DefaultState
        NormalTransition,               //{0} - Event, {1} - ConditionFunction, {2} - ActionFunction, {3} - NextState
        StateTableNorm,                 //{0} - StateName, {1} - ParentStateName, {2} - TransitionTable
        StateTableRoot,                 //{0} - RootStateName, {1} - TransitionTable
        ActionFuncSignature ,           //{0} - FunctionName
        ConditionFuncSignature,         //{0} - FunctionName
        ActionFuncName,                 //{0} - ComponentName(e.g: SystemSm_Prj), {1} - ActionName(e.g: setEnableHmi)
        ConditionFuncName,              //{0} - ComponentName(e.g: SystemSm_Prj), {1} - ConditionName(e.g: checkHighVoltage)
        EntryExitFuncName,              //{0} - ComponentName(e.g: SystemSm_Prj), {1} - entry/exit, {2} - ActionName (e.g: enterStateA...)
        Hsm,                            //{0} - HsmName, {1} - StateTable
        Enum,                           //{0} - PrefixForEventEnumName(should be ComponentName), {1} - EventList
        FuncWithDoxComment,             //{0} - FunctionName, {1} - returnType(e.g:void/boolean), {2} - functionBody
        ConditionFuncBody,              //{0} - Conditions (e.g: voltage != high || thermal != high) and must be OneLine
        ActionFuncBody,                 //{0} - Actions (e.g: SetLowVoltageTrue ; setEnableHmi ) and must be OneLine
        HeaderFile,                     //{0} - ComponentName, {1} - fileName, {2} - IncludeGuardName, {3} - SourceInsideIncludeGuard(event enum declaration and functions prototype), {4} - HsmTable
        SourceFile,                     //{0} - ComponentName, {1} - headerFileName, {3} - AllFunctionDefinitions
        NoFormat
    }

   
    public class FormatGetter
    {
        /// <summary>
        /// prevent other class from self-willed create new instance of this class
        /// </summary>
        /// <param name="formatFile"></param>
        private FormatGetter(string fileName)
        {
            _formatFile = DeployInfo.getInstallationPath(DeployInfo.InstallationInfo.DataKey) + "\\" + fileName;
        }

        private FormatGetter() { }

        public static string get(SourceCodeFormatIndex index)
        {
            return _sourceFormatGetter.get((int)index);
        }

        public static string get(ErrorMessageFormatIndex index)
        {
            return _errorFormatGetter.get((int)index);
        }

        public static bool AllDataReady()
        {
            try
            {
                _sourceFormatGetter.ready();
                _errorFormatGetter.ready();
            }
            catch
            {
                throw;
            }

            return true;
        }
        private string get(int index)
        {
            
            string format = "";
            try
            {
                format = _formats[index];
            }
            catch (System.IndexOutOfRangeException)
            {
                throw new System.Exception(string.Format("Error: get wrong index --> max index = {0}, request index = {1}", format.Length, index)); 
            }
            return format;
        }
        private bool ready()
        {
            if ((_formats == null))
            {
                _formats = FileFormatReader.read(_formatFile);
                if(_formats == null)
                {
                    string message = "File not found --> " + _formatFile + " please reinstall the AddIn";
                    MessageLogger.log(message, LogLevel.FATAL);
                    throw new System.Exception(message);
                }
            }
            return true;
        }

        public static void updateFormatsIfFilesModified()
        {
            updateFormatsIfFilesModified(_sourceFormatGetter);
            updateFormatsIfFilesModified(_errorFormatGetter);
        }

        private static void updateFormatsIfFilesModified(FormatGetter formatGetter)
        {
            if (FileFormatReader.shouldReadBecauseOfFileChanged(formatGetter._formatFile))
            {
                formatGetter._formats = FileFormatReader.read(formatGetter._formatFile);
            }
        }

        private static FormatGetter _sourceFormatGetter = new FormatGetter(DeployInfo.InstallationInfo.SourceFormatFileName);
        private static FormatGetter _errorFormatGetter = new FormatGetter(DeployInfo.InstallationInfo.ErrorFormatFileName);


        private string[] _formats;
        private string _formatFile;

    }


}