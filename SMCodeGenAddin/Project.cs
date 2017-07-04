using System;
using System.Diagnostics;

namespace ARSourceGeneration
{
    class Project
    {
        public Project (EA.Repository rep, EA.Diagram currentDiagram, FunctionNamerType fnType, bool oneEntryExitPerState = true)
        {
            FormatGetter.updateFormatsIfFilesModified();
            _sourceData = new DSource();
            _sourceGen = new CGSource(_sourceData, fnType);
            _repository = rep;
            _chartParser = new EAParsing.SMConnectorBaseParser(rep, currentDiagram, _sourceData);
            this.OneEntryPerState = oneEntryExitPerState;
        }
        public void setEARepository(EA.Repository rep)
        {
            _chartParser.Repository = rep;
        }

        public void generateSource(string outPath)
        {
            try
            {
                MessageLogger.startLogging();
                if(FormatGetter.AllDataReady() &&_chartParser.ready())
                {
                    bool done = _chartParser.parse();
                    if(done)
                    {
                        writeSource(SourceFileMode.Header, outPath);
                        writeSource(SourceFileMode.Source, outPath);
                        writeSource(SourceFileMode.Notes, outPath);
                    }
                }

            }
            catch (Exception e)
            {
                MessageLogger.log(e.StackTrace, LogLevel.FATAL);
                throw;
            }
            finally
            {
                MessageLogger.endLogging();
            }
        }

        void writeSource(SourceFileMode sourceMode, string outPath)
        {
            _sourceGen.SourceMode = sourceMode;
            SourceFileWriter.write(outPath + "\\" + _sourceGen.getFileName(), _sourceGen.constructSource());
        }

        string getFileName(SourceFileMode sourceMode)
        {
            _sourceGen.SourceMode = sourceMode;
            return _sourceGen.getFileName();
        }
        public string[] getOutPutFiles()
        {
            if(_sourceGen != null)
            {
                return new string[] { getFileName(SourceFileMode.Header), getFileName(SourceFileMode.Source), getFileName(SourceFileMode.Notes)};
            }
            else
            {
                return null;
            }
        }

        public string ProjectName
        {
            get { return _sourceData.ComponentName; }
        }
        public bool OneEntryPerState 
        { 
            get 
            { 
                return DState.OneEntryExitPerState;
            } 
            set 
            { 
                DState.OneEntryExitPerState = value; 
            } 
        }

        public void changeChartParser(EAParsing.SMChartParser parser)
        {
            _chartParser = parser;
            parser.Repository = _repository;
            parser.SourceData = _sourceData;
        }

        private EAParsing.SMChartParser _chartParser;
        private EA.Repository _repository;
        private DSource _sourceData;
        private CGSource _sourceGen;
    }
}