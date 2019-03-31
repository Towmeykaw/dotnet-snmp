using Lextm.SharpSnmpLib;
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
    }
}
