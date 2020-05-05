using System;
using System.Text;
using System.Text.Json;

namespace Microsoft.Extensions.Logging.Console
{
    public class JsonConsoleLogFormatterOptions : ConsoleLoggerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public override string Formatter { get => "Json"; set => throw new NotImplementedException("why set this"); }

        public JsonWriterOptions JsonWriterOptions { get; set; }
    }
}
