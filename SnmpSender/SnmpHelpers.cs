using Lextm.SharpSnmpLib;
using System;
using System.Collections.Generic;

namespace SnmpSender
{
    public static class SnmpHelpers
    {
        public static VersionCode GetVersion(int version)
        {
            return version switch
            {
                1 => VersionCode.V1,
                2 => VersionCode.V2,
                3 => VersionCode.V3,
                _ => VersionCode.V2
            };
        }
        public static ISnmpData GetType(string shortName, string data)
        {
            try
            {
                return shortName switch
                {
                    "s" => new OctetString(data),
                    "i" => new Integer32(int.Parse(data)),
                    "u" => new Gauge32(long.Parse(data)),
                    _ => new OctetString("") as ISnmpData
                };
            }
            catch(Exception e)
            {
                Console.WriteLine($"Failed to parse data {data}. Error message: {e.Message}");
                throw (e);
            }
        }
        public static List<Variable> ParseVariables(List<string> variables)
        {
            try
            {
                var parsedVariables = new List<Variable>();
                foreach (var variable in variables)
                {
                    var splitString = variable.Split(',', 3);
                    var objectId = new ObjectIdentifier(splitString[0]);
                    var data = SnmpHelpers.GetType(splitString[1], splitString[2]);
                    parsedVariables.Add(new Variable(objectId, data));
                }
                return parsedVariables;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to parse variable");
                throw new ArgumentException(e.Message);
            }
        }
    }
}
