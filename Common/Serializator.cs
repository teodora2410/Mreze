using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    public static class MemorySerializer
    {
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

      
    }
}