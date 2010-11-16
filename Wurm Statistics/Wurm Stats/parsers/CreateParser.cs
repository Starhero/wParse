using System;
using System.Globalization;
using WParse;

namespace EWOS.parsers
{
    public sealed class Create : IParser
    {
        public string Name
        {
            get
            {
                return "Things you created.";
            }
        }

        public void Parse(string line, EventProxy proxy)
        {
            if (!line.Contains("You create"))
            {
                return;
            }

            int start = line.IndexOf("t") + 3;
            int length = line.LastIndexOf(".") - start;

            if (start <= 0 || length <= 0)
            {
                return;
            }

            string item = line.Substring(start, length);

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

            proxy.Add(this, "Total created items", time);
            proxy.Add(this, String.Format("You created : {0}", item), time);
        }
    }
}
