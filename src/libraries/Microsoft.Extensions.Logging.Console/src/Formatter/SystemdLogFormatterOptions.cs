using System;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    public class SystemdLogFormatterOptions : ConsoleLoggerOptions
    {
        public override ConsoleLoggerFormat Format => ConsoleLoggerFormat.Systemd;
        public override string Formatter => "Systemd";
    }
}
