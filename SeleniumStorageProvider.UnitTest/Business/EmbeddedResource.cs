using System.IO;
using System.Linq;
using System.Reflection;

namespace SeleniumStorageProvider.UnitTest.Business
{
    public class EmbeddedResource
    {
        public string Get(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }

        public byte[] Get()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string fileName = assembly.GetManifestResourceNames().First();
            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            {
                if (stream != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }

            return null;
        }
    }
}
