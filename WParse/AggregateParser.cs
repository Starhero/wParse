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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EWOS.SplashScreen;

namespace WParse
{
    public sealed class AggregateParser
    {
        private static readonly int ReaderConcurrencyThreshold = Environment.ProcessorCount / 2;
        private static readonly int ParserConcurrencyThreshold = Environment.ProcessorCount;
        private static readonly int TotalConcurrencyThreshold = ReaderConcurrencyThreshold * ParserConcurrencyThreshold;

        private readonly List<TextReader> readers;
        private readonly List<IParser> parsers;
        private readonly EventProxy proxy;

        public EventProxy Proxy
        {
            get
            {
                return this.proxy;
            }
        }

        public AggregateParser(IEnumerable<string> logs, IEnumerable<string> scripts)
        {
            this.readers = new List<TextReader>();
            foreach (string log in logs)
            {
                this.readers.Add(new StreamReader(log));
            }

            this.parsers = new List<IParser>();
            foreach (string script in scripts)
            {
                this.parsers.AddRange(ScriptEngine.GenerateParsers(script));
            }

            this.proxy = new EventProxy();
        }

        public AggregateParser(IEnumerable<string> wEvent,IEnumerable<string> wSkills, IEnumerable<string> scripts)
        {
            this.readers = new List<TextReader>();
            foreach (string log in wEvent)
            {
                this.readers.Add(new StreamReader(log));
            }
            foreach(string log in wSkills)
            {
                this.readers.Add(new StreamReader(log));
            }

            this.parsers = new List<IParser>();
            foreach (string script in scripts)
            {
                this.parsers.AddRange(ScriptEngine.GenerateParsers(script));
            }

            this.proxy = new EventProxy();
        }  
        

        private void ParseFullConcurrency()
        {
            Parallel.ForEach<TextReader>(this.readers, reader =>
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Parallel.ForEach<IParser>(this.parsers, parser =>
                            {
                                parser.Parse(line, this.proxy);
                            });
                    }
                });
        }

        private void ParseInnerConcurrency()
        {
            foreach (TextReader reader in this.readers)
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Parallel.ForEach<IParser>(this.parsers, parser =>
                    {
                        parser.Parse(line, this.proxy);
                    });
                }
            }
        }

        private void ParseOuterConcurrency()
        {
            Parallel.ForEach<TextReader>(this.readers, reader =>
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    foreach (IParser parser in this.parsers)
                    {
                        parser.Parse(line, this.proxy);
                    }
                }
            });
        }

        private void ParseNoConcurrency()
        {
            foreach (TextReader reader in this.readers)
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    foreach (IParser parser in this.parsers)
                    {
                        parser.Parse(line, this.proxy);
                    }
                }
            }
        }

        public void AddLog(string path)
        {
            this.readers.Add(new StreamReader(path));
        }

        public void AddParser(IParser parser)
        {
            this.parsers.Add(parser);
        }

        public void AddParsers(IEnumerable<IParser> parsers)
        {
            this.parsers.AddRange(parsers);
        }

        public void Parse()
        {
            this.proxy.Clear();

            if (this.readers.Count * this.parsers.Count >= TotalConcurrencyThreshold)
            {
                SplashScreen.SetStatus("Parsing at full Concurrency: " + this.parsers.Count.ToString() + " Threads. " + readers.Capacity.ToString() + " logs at once.");
                this.ParseFullConcurrency();
            }
            else if (this.readers.Count >= ReaderConcurrencyThreshold)
            {
                SplashScreen.SetStatus("Outer?");

                this.ParseOuterConcurrency();
            }
            else if (this.parsers.Count >= ParserConcurrencyThreshold)
            {
                SplashScreen.SetStatus("Parsing at full Concurrency");

                this.ParseInnerConcurrency();
            }
            else
            {
                SplashScreen.SetStatus("Parsing at full Concurrency");

                this.ParseNoConcurrency();
            }
        }
    }
}
