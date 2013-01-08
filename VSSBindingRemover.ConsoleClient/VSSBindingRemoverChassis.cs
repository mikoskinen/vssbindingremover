using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engines;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace VSSBindingRemover.ConsoleClient
{
    class VSSBindingRemoverChassis
    {
        static void Main(string[] args)
        {
            // See this article for more details on Engines and Chassis: http://msdn.microsoft.com/en-us/magazine/cc164014.aspx
            Engines.ConsoleEngineBase engine = new VSSBindingRemoverEngine();
            engine.Main(args);
        }
    }
}
