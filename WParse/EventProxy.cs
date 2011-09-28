#region MIT License
/*
 * Copyright (c) 2010 Michael Gray
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WParse
{
    public sealed class EventProxy
    {
        private static readonly int ConcurrencyThreshold = Environment.ProcessorCount * Environment.ProcessorCount;

        private readonly ConcurrentDictionary<string, List<string>> links;
        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> entries;

        public EventProxy()
        {
            this.links = new ConcurrentDictionary<string, List<string>>();
            this.entries = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();
        }

        public void Add(IParser parser, string key, DateTime time)
        {
            List<string> links = new List<string>();
            links = this.links.GetOrAdd(parser.Name, links);

            lock (this.links)
            {
                if (!links.Contains(key))
                {
                    links.Add(key);
                }
            }

            ConcurrentQueue<DateTime> entries = new ConcurrentQueue<DateTime>();
            entries = this.entries.GetOrAdd(key, entries);
            entries.Enqueue(time);
        }

        public void Clear()
        {
            this.links.Clear();
            this.entries.Clear();
        }

        public long GetCount()
        {
            long result = 0;
            foreach (KeyValuePair<string, ConcurrentQueue<DateTime>> pair in this.entries)
            {
                result += pair.Value.Count;
            }

            return result;
        }

        public long GetCount(DateTime start, DateTime end)
        {
            long result = 0;
            foreach (KeyValuePair<string, ConcurrentQueue<DateTime>> pair in this.entries)
            {
                if (pair.Value.Count <= ConcurrencyThreshold)
                {
                    foreach (DateTime time in pair.Value)
                    {
                        if (start <= time && end >= time)
                        {
                            result++;
                        }
                    }
                }
                else
                {
                    Parallel.ForEach<DateTime>(pair.Value, time =>
                       {
                           if (start <= time && end >= time)
                           {
                               Interlocked.Increment(ref result);
                           }
                       });
                }
            }

            return result;
        }

        public string[] GetLinks(string parser)
        {
            List<string> links;
            if (this.links.TryGetValue(parser, out links))
            {
                lock (links)
                {
                    return links.ToArray();
                }
            }
            else
            {
                return new string[0];
            }
        }

        public Dictionary<string, List<string>> GetLinks()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            lock (this.links)
            {
                foreach (KeyValuePair<string, List<string>> pair in this.links)
                {
                    List<string> links = new List<string>(pair.Value.ToArray());
                    result.Add(pair.Key, links);
                }
            }

            return result;
        }

        public Dictionary<string, Queue<DateTime>> GetEntries()
        {
            Dictionary<string, Queue<DateTime>> result = new Dictionary<string, Queue<DateTime>>();
            foreach (KeyValuePair<string, ConcurrentQueue<DateTime>> pair in this.entries)
            {
                Queue<DateTime> times = new Queue<DateTime>(pair.Value.ToArray());
                result.Add(pair.Key, times);
            }

            return result;
        }

        public bool TryGetCount(string key, out long result)
        {
            ConcurrentQueue<DateTime> entries;
            if (this.entries.TryGetValue(key, out entries))
            {
                result = entries.Count;
                return true;
            }
            else
            {
                result = 0;
                return false;
            }
        }

        public bool TryGetCount(string key, DateTime start, DateTime end, out long result)
        {
            result = 0;

            ConcurrentQueue<DateTime> entries;
            if (!this.entries.TryGetValue(key, out entries))
            {
                return false;
            }

            DateTime[] times = entries.ToArray();
            for (int i = 0; i != times.Length; i++)
            {
                if (times[i] >= start && times[i] <= end)
                {
                    result++;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }
    }
}
