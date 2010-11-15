using System;
using System.Globalization;
using WParse;

namespace TestEnvironment
{
    public sealed class BoardParser : IParser
    {
        public string Name
        {
            get
            {
                return "Boarding";
            }
        }

        // [01:19:55] You board on the "Ocean Warrior" as the captain.
        public void Parse(string line, EventProxy proxy)
        {
            if (!line.Contains("You board on the"))
            {
                return;
            }

            int start = line.IndexOf("\"") + 1;
            int length = line.LastIndexOf("\"") - start;

            if (start <= 0 || length <= 0)
            {
                return;
            }

            string name = line.Substring(start, length);

            start = line.LastIndexOf(' ') + 1;
            length = line.Length - start - 1;

            if (start <= 0 || length <= 0)
            {
                return;
            }

            string title = line.Substring(start, length);

            start = line.IndexOf('[') + 1;
            length = line.IndexOf(']') - start;

            if (start <= 0 || length <= 0)
            {
                return;
            }

            DateTime time;
            if (!DateTime.TryParseExact(line.Substring(start, length), "HH:mm:ss", null, DateTimeStyles.None, out time))
            {
                return;
            }

            proxy.Add(this,"Board", time);
            proxy.Add(this,String.Format("Board {0}", name), time);
            proxy.Add(this,String.Format("Board {0} as {1}", name, title), time);
            proxy.Add(this,String.Format("Board as {0}", title), time);
        }
    }
}
