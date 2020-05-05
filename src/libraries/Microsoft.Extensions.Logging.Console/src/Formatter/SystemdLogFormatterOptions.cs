using System;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    internal class SystemdLogFormatterOptions : ConsoleLoggerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Formatter { get => "Systemd"; set => throw new NotImplementedException("why set this"); }
    }
}
