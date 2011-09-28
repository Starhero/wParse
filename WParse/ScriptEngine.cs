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
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using EWOS.SplashScreen;
using System.Windows.Forms;
namespace WParse
{
    internal static class ScriptEngine
    {
        public static List<IParser> GenerateParsers(string path)
        {
            CompilerParameters options = new CompilerParameters()
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };

            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("WParse.dll");

            CompilerResults results;
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                results = provider.CompileAssemblyFromFile(options, path);
                
            }

            if (results.Errors.HasErrors)
            {
                throw new ApplicationException(String.Format("An error was encountered when compiling \"{0}\": {1}", path, results.Errors[0]));
            }

            SplashScreen.SetStatus("Compiled " + path);
            List<IParser> parsers = new List<IParser>();

            Type contract = typeof(IParser);
            Assembly assembly = results.CompiledAssembly;

            foreach (Type type in assembly.GetTypes())
            {
                if (contract.IsAssignableFrom(type))
                {
                    parsers.Add((IParser)assembly.CreateInstance(type.FullName));
                }
            }

            return parsers;
        }
    }
}
