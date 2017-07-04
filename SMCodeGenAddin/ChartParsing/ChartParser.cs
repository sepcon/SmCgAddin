using System;
using System.Collections.Generic;
using System.Diagnostics;
using ARSourceGeneration;

namespace EAParsing
{
    public abstract class SMChartParser
    {
        protected enum ElementSubType : int
        {
            Initial = 3,
            Final = 4,
            Junction = 10,
            Choice = 11
        }
        public SMChartParser(EA.Repository repository = null, EA.Diagram currentDiagram = null, DSource sourceData = null)
        {
            this.Repository = repository;
            this.SourceData = sourceData;
            this.CurrentDiagram = currentDiagram;
        }

        public EA.Repository Repository
        {
            get { return _repository; }
            set
            {
                _repository = value;
            }
        }

        protected ElementSubType getSubType(EA.Element elem)
        {
            return (ElementSubType)elem.Subtype;
        }

        protected virtual bool isRequiredChartType(EA.Diagram dg)
        {
            return dg != null;
        }
        public virtual bool ready()
        {
            return SourceData != null && Repository != null && CurrentDiagram != null;
        }

        public string chartType()
        {
            return _currentDiagram != null ? _currentDiagram.Type.ToString() : "";
        }

        public abstract bool parse();

        protected bool isChoiceNode(EA.Element eaNode)
        {
            return getSubType(eaNode) == ElementSubType.Choice || getSubType(eaNode) == ElementSubType.Junction;
        }
        public EA.Diagram CurrentDiagram
        {
            get 
            {
                return _currentDiagram;
            }

            set 
            {
                _currentDiagram = value; 
                if(_currentDiagram != null)
                {
                    SourceData.ComponentName = _currentDiagram.Name;
                }
            }
        }


        public ARSourceGeneration.DSource SourceData
        {
            get { return _sourceData; }
            set { _sourceData = value; }
        }

        protected EA.Element safelyGetElement(int id)
        {
            EA.Element elem = null;
            try
            {
                elem = Repository.GetElementByID(id);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                
            }

            return elem;
        }

        private ARSourceGeneration.DSource _sourceData;
        private EA.Diagram _currentDiagram;
        private EA.Repository _repository;
    }

   }
