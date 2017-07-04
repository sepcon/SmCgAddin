using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSourceGeneration
{
    public abstract class CGCodeGeneratorBase
    {
        public virtual string constructSource()
        {
            string format = getFormat();
            string[] args = getFormatArguments();
            return constructSource(format, args);
        }

        public virtual string constructSource(string format, params string[] arguments)
        {
            string codeSnippet;
            if ((format.Length != 0) && (arguments != null))
            {
                StringBuilder stringBuilder = new StringBuilder(predictStringBuilderCapcity(format, arguments));
                codeSnippet = stringBuilder.AppendFormat(format, arguments).ToString();
            }
            else
            {
                codeSnippet = "";
            }

            return codeSnippet;
        }
        protected string formatTextInColumn(string text, int columnSize)
        {
            return text + new string(' ', columnSize - text.Length);
        }

        /// <summary>
        /// derived class has to override this method get arguments for code formating
        /// </summary>
        /// <returns>string[] - list of arguments using to generate code by format</returns>
        protected abstract string[] getFormatArguments();
        /// <summary>
        /// code snippet format
        /// </summary>
        /// <returns>string - format string</returns>
        protected abstract string getFormat();

        protected int predictStringBuilderCapcity(string format, params string[] arguments)
        {
            int capacity = format.Length;
            foreach (var arg in arguments)
            {
                capacity += 2 * arg.Length;
            }
            return capacity;
        }
    }


    public abstract class CGStateOperationBase : CGCodeGeneratorBase
    {
        public CGStateOperationBase(CGState state) { _state = state; }
        public IFunctionNamer FuncNamer
        {
            get { return _state.Hsm.CgSource.FuncNamer; }
        }

        protected CGState _state;
    }



}
