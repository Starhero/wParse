using System;
using System.Globalization;
using WParse;

namespace EWOS.parsers
{
    public sealed class Dropped : IParser
    {
        public string Name
        {
            get
            {
                return "SINGLE things you dropped";
            }
        }

        public void Parse(string line, EventProxy proxy)
        {
            if (!line.Contains("You drop a"))
            {
                return;
            }

            int start = line.IndexOf("a") + 2;
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

            proxy.Add(this, "Total drops", time);
            proxy.Add(this, String.Format("You dropped : {0}", item), time);
        }
    }
}
