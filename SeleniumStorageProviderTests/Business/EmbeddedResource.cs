﻿using System.IO;
using System.Linq;
using System.Reflection;

namespace SeleniumStorageProviderTests.Business
{
    public class EmbeddedResource
    {
        public string GetAsString(string fileName)
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

        public byte[] GetAsByteArray(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
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
