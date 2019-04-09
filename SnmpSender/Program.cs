using System.CommandLine;
using System.CommandLine.Invocation;

namespace SnmpSender
{
    class Program
    {           
        public static string[] Variable { get; set; }

        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("Sends snmp messages");
            var trapCommand = new Command("trap");
            rootCommand.Add(GetCommand.CreateCommand());
            rootCommand.Add(SetCommand.CreateCommand());
            rootCommand.Add(trapCommand);            

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }
                
        
    }
}
