using System.CommandLine;

namespace SnmpSender
{
    public static class CommandOptions
    {
        public static Command AddCommonOptions(this Command command)
        {
            var port = new Option(
                new string[] { "-p", "--port" },
                "Port to use for messages(default 161)",
                new Argument<int>(defaultValue: 161));
            var community = new Option(
                new string[] { "-c", "--community" },
                "Community of message(default public)",
                new Argument<string>(defaultValue: "public"));
            var version = new Option(
                            new string[] { "-v", "--version" },
                            "Snmp version of the message(default 2)",
                            new Argument<int>(defaultValue: 2));
            var address = new Option(
                new string[] { "-a", "--address" },
                "Ipaddress to send message to(default 127.0.0.1)",
                new Argument<string>(defaultValue: "127.0.0.1"));
            command.AddOption(port);
            command.AddOption(community);
            command.AddOption(version);
            command.AddOption(address);
            return command;
        }
    }
}
