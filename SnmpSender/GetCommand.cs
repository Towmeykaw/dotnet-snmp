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
    public static class GetCommand
    {
        public static Command CreateCommand()
        {
            var command = new Command("get")
            {
                Argument = new Argument<string>
                {
                    Arity = ArgumentArity.ExactlyOne,
                    Description = "The ObjectId to send",
                    Name = "objectId"
                }
            };
            command.AddCommonOptions();
            command.Handler = CommandHandler.Create<int, int, string, string, string>(Send);
            return command;
        }
        public static void Send(int port, int version, string community, string address, string objectId)
        {
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
                var variable = new List<Variable>
                {
                    new Variable(new ObjectIdentifier(objectId))
                };
                IPEndPoint receiver = new IPEndPoint(ip, port);
                if (versionCode != VersionCode.V3)
                {
                    var response = Messenger.Get(versionCode, receiver, new OctetString(community), variable, 10000);
                    Console.WriteLine(variable);
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
