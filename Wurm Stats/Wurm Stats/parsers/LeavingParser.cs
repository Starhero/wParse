using System;
using System.Globalization;
using WParse;

namespace EWOS.parsers
{
    public sealed class LeavingSettlements : IParser
    {
        public string Name
        {
            get
            {
                return "Settlments you have left.";
            }
        }

        public void Parse(string line, EventProxy proxy)
        {
            if (line.Contains("\""))
            {
                return;
            }
            if (!line.Contains("You leave"))
            {
                return;
            }


            if (line.Contains("You leave Freedom Isles."))
            {
                return;
            }

            if (line.Contains("You leave Mol-Rehan"))
            {
                return;
            }

            if (line.Contains("You leave Jenn-Kellon"))
            {
                return;
            }

            int start = line.IndexOf("e") + 4;
            int length = line.LastIndexOf(".") - start;

            if (start <= 0 || length <= 0)
            {
                return;
            }

            string settlements = line.Substring(start, length);
            
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

            proxy.Add(this, "Leaving settlements", time);
            proxy.Add(this, String.Format("Left {0}", settlements), time);
        }
    }
}
