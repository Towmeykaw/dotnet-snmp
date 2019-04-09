using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SnmpSender
{
    public static class TrapCommand
    {
        public static Command CreateCommand()
        {
            var command = new Command("trap")
            {
                Argument = new Argument<string>
                {
                    Arity = ArgumentArity.ExactlyOne,
                    Description = "The Enterprise of trap",
                    Name = "enterprise"
                }
            };
            command.AddCommonOptions();
            var variables = new Option(
                new string[] { "-s", "--setVar" },
                "Variables to set. Use format objectId,type,value. Where type can be s(string),i(int),u(uint)",
                new Argument<string>()
                {
                    Arity = ArgumentArity.OneOrMore
                });
            var requestId = new Option(
                new string[] {"-r", "--requestId"},
                "Optional requestId of message",
                new Argument<string>()
                {
                    Arity = ArgumentArity.ZeroOrOne
                });
            command.AddOption(variables);
            command.Handler = CommandHandler.Create<int, int, string, string, string, int?, List<string>>(Send);
            return command;
        }
        public static void Send(int port, int version, string community, string address, string enterprise, int? requestId, List<string> setVar)
        {
            var setVariables = SnmpHelpers.ParseVariables(setVar);
            var versionCode = SnmpHelpers.GetVersion(version);
            if (!IPAddress.TryParse(address, out IPAddress ip))
            {
                var addresses = Dns.GetHostAddresses(address);
                ip = addresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
                if (ip == null)
                {
                    Console.WriteLine("IpAddress invalid: " + address);
                    return;
                }
            }
            try
            {
                IPEndPoint receiver = new IPEndPoint(ip, port);
                if (versionCode == VersionCode.V1)
                {
                    Messenger.SendTrapV1(receiver, IPAddress.Loopback, new OctetString(community),
                        new ObjectIdentifier(enterprise), GenericCode.ColdStart, 0, 0, setVariables);
                    return;
                }
                else if (versionCode == VersionCode.V2)
                {
                    Messenger.SendTrapV2(requestId ?? 0, versionCode, receiver, new OctetString(community), 
                        new ObjectIdentifier(enterprise),0, setVariables);
                    return;
                }
            }
            catch (SnmpException ex)
            {
                Console.WriteLine(ex);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
