using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BeiderMorse.Encoder.Util
{
    public static class ResourceUtil
    {
       public static IEnumerable<string> ReadAllResourceLines(string input)
        {
            using (StringReader reader = new StringReader(input))
            {
                return EnumerateLines(reader).ToList();
            }
        }

        static IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}
