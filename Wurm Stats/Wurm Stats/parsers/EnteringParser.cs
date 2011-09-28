using System;
using System.Globalization;
using WParse;

namespace EWOS.parsers
{
    public sealed class EnteringSettlements : IParser
    {
        public string Name
        {
            get
            {
                return "Settlments you have entered.";
            }
        }

        // [01:19:55] You board on the "Ocean Warrior" as the captain.
        public void Parse(string line, EventProxy proxy)
        {
            if (!line.Contains("You enter"))
            {
                return;
            }


            if (line.Contains("You enter Freedom Isles."))
            {
                return;
            }

            if (line.Contains("You enter Mol-Rehan"))
            {
                return;
            }

            if (line.Contains("You enter Jenn-Kellon"))
            {
                return;
            }

            int start = line.IndexOf("r") + 2;
            int length = line.LastIndexOf(".") - start;

            if (start <= 0 || length <= 0)
            {
                return;
            }

            string settlements = line.Substring(start, length);

            //start = line.LastIndexOf(' ') + 1;
            //length = line.Length - start - 1;

            //if (start <= 0 || length <= 0)
            //{
            //    return;
            //}

            //string title = line.Substring(start, length);

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

            proxy.Add(this, "Entering settlements", time);
            proxy.Add(this, String.Format("Entered {0}", settlements), time);
            //proxy.Add(this, String.Format("Board {0} as {1}", settlements, title), time);
            //proxy.Add(this, String.Format("Board as {0}", title), time);
        }
    }
}
