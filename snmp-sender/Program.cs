using McMaster.Extensions.CommandLineUtils;
using System;
using System.Net.Http;

namespace SnmpSender
{
    class Program
    {           
        //[Option(CommandOptionType.MultipleValue, LongName = "message", ShortName = "m", Description = "Set variable to send. The format should be (objectId,value,valuetype(string, int) )")]
        public static string[] Variable { get; set; }


        static int Main(string[] args)
        {           
            var app = new CommandLineApplication
            {
                Name = "snmp",
                Description = "Sends snmp messages",
            };
            app.HelpOption(inherited: true);
            app.Command("get", getCmd =>
            {
                var (port, address, community, version) = AddCommonOptions(getCmd);
                //if (int.TryParse(getCmd.Option("-p|--port", "Port to use for messages(default 161)", CommandOptionType.SingleValue), out int p))
                //Port = p;
                //var ss = getCmd.Option("-p|--port", "Port to use for messages(default 161)", CommandOptionType.SingleValue);

                getCmd.OnExecute(() =>
                {
                    Console.WriteLine("Specify a subcommand");
                    getCmd.ShowHelp();
                    return 1;
                });
            });
            app.Command("set", setCmd =>
            {
                setCmd.Description = "Set config value";
                //var key = setCmd.Argument("key", "Name of the config").IsRequired();
                //var val = setCmd.Argument("value", "Value of the config").IsRequired();
                setCmd.OnExecute(() =>
                {
                    Console.WriteLine("{\"dummy\": \"value\"}");
                    //Console.WriteLine($"Setting config {key.Value} = {val.Value}");
                });
            });

            app.Command("trap", listCmd =>
            {
                var json = listCmd.Option("--json", "Json output", CommandOptionType.NoValue);
                listCmd.OnExecute(() =>
                {
                    if (json.HasValue())
                    {
                        Console.WriteLine("{\"dummy\": \"value\"}");
                    }
                    else
                    {
                        Console.WriteLine("dummy = value");
                    }
                });
            });
            /*app.OnExecute(async () =>
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(Adress), Port);

                var versionCode = SnmpHelpers.GetVersion(Version);
                try
                {
                    var dataPackage = new List<Variable>();
                    //   dataPackage.Add(data3);

                    await Messenger.GetAsync(versionCode, endPoint, new OctetString(Community), dataPackage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                app.ShowHelp();
                return 1;
            });*/

            return app.Execute(args);
        }


        private static (int port, string address, string community, int version) AddCommonOptions(CommandLineApplication command)
        {
            var port = command.Option("-p|--port", "Port to use for messages(default 161)", CommandOptionType.SingleValue);
            var address = command.Argument("address", "Ip to send message to");
            var community = command.Option("-c|--community", "Community of message(default public)", CommandOptionType.SingleValue);
            var version = command.Option("-v|--version", "Snmp version of the message(default 2)", CommandOptionType.SingleValue);

            if(!int.TryParse(port.Value(), out int p))
                p = 161;
            string communityValue = "public";
            if (!string.IsNullOrWhiteSpace(community.Value()))
                communityValue = community.Value();
            if (!int.TryParse(version.Value(), out int v))
                v = 2;
            return (p, address.Value, communityValue, v);        
        }
    }
}
