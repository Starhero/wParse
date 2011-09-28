using System;
using System.Globalization;
using WParse;

namespace EWOS.parsers
{
    public sealed class Titles : IParser
    {
        public string Name
        {
            get
            {
                return "Titles you have achieved.";
            }
        }

        public void Parse(string line, EventProxy proxy)
        {
            if (!line.Contains("You have just received the title"))
            {
                return;
            }

            int start = line.IndexOf("'") + 1;
            int length = line.LastIndexOf("'") - start;

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

            proxy.Add(this, "Total Titles", time);
            proxy.Add(this, String.Format("Achieved : {0}", title), time);
        }
    }
}
