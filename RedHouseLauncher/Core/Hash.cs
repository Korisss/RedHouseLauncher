using Force.Crc32;

namespace RedHouseLauncher.Core
{
    internal class Hash
    {
        internal static int Crc32(byte[] buffer)
        {
            return unchecked((int)Crc32Algorithm.Compute(buffer));
        }
    }
}
