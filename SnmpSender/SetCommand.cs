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
    public static class SetCommand
    {
        public static Command CreateCommand()
        {
            var command = new Command("set");
            command.AddCommonOptions();
            var variables = new Option(
                new string[] { "-s", "--setVar" },
                "Variables to set. Use format objectId,type,value. Where type can be s(string),i(int),u(uint)",
                new Argument<string>()
                {
                    Arity = ArgumentArity.OneOrMore
                });
            command.AddOption(variables);
            command.Handler = CommandHandler.Create<int, int, string, string, List<string>>(Send);
            return command;
        }
        public static void Send(int port, int version, string community, string address, List<string> setVar)
        {           
            if(setVar == null)
            {
                Console.WriteLine("Missing variable --setVar");
                return;
            }
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
                if (versionCode != VersionCode.V3)
                {
                    var response = Messenger.Set(versionCode, receiver, new OctetString(community), setVariables, 10000);
                    Console.WriteLine(response);
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
