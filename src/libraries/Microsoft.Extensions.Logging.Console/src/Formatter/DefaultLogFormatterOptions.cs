using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    internal class DefaultLogFormatterOptions : ConsoleLoggerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Formatter { get => "Default"; set => throw new NotImplementedException("why set this"); }
    }
}
