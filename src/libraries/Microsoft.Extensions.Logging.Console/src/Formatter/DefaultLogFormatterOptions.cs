// // Licensed to the .NET Foundation under one or more agreements.
// // The .NET Foundation licenses this file to you under the MIT license.
// // See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Logging.Console
{
    public class DefaultLogFormatterOptions : ConsoleLoggerOptions
    {
        public override ConsoleLoggerFormat Format => ConsoleLoggerFormat.Default;
        public override string Formatter => "Default";
    }
}
// using System;

// namespace Microsoft.Extensions.Logging.Console
// {
//     /// <summary>
//     /// Options for a <see cref="ConsoleLogger"/>.
//     /// </summary>
//     public class DefaultLogFormatterOptions : ConsoleLoggerOptions
//     {
        
//         public override ConsoleLoggerFormat Format
//         {
//             get
//             {
//                 return base.Format;
//             }
//             set
//             {
//                 base.Virtual = "Derived:" + value;
//             }
//         }
//     }
// }
