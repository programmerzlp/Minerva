/*
    Copyright © 2010 The Divinity Project; 2013-2016 Dignity Team.
    All rights reserved.
    https://github.com/dignityteam/minerva
    http://www.ragezone.com


    This file is part of Minerva.

    Minerva is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Minerva is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Minerva.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Includes

using System;
using System.IO;

using IronRuby;
using Microsoft.Scripting.Hosting;

#endregion

namespace Minerva
{
    /// <summary>A container class for the hosted IronRuby scripting system.</summary>
    public class ScriptHandler
    {
        ScriptRuntime _runtime;
        ScriptEngine _engine;
        ScriptScope _scope;
        dynamic _instance;

        /// <summary>Gets the ScriptRuntime associated with this scripting instance.</summary>
        public ScriptRuntime RubyRuntime { get { return _runtime; } }
        /// <summary>Gets the ScriptEngine associated with this scripting instance.</summary>
        public ScriptEngine RubyEngine { get { return _engine; } }
        /// <summary>Gets the current ScriptScope for this scripting instance.</summary>
        public ScriptScope Scope { get { return _scope; } }
        /// <summary>Gets the current class instance.</summary>
        public dynamic Instance { get { return _instance; } }

        public ScriptHandler()
        {
            _runtime = Ruby.CreateRuntime();
            _runtime.LoadAssembly(GetType().Assembly);
            _engine = Ruby.GetEngine(_runtime);
            _scope = _engine.CreateScope();
        }

        /// <summary>Concatenates all source fragments into a single, class-wrapped source file.</summary>
        /// <param name="directory">The name of the directory to read the source fragments from.</param>
        /// <param name="require">An array of requirements to be written to the completed source file.</param>
        /// <param name="include">An array of includes to be written to the completed source file.</param>
        public void Concatenate(string directory, string[] require = null, string[] include = null)
        {
            if (require == null)
                require = new string[] { };

            if (include == null)
                include = new string[] { };

            // First, let's create the new file to hold all of our source
            var source = File.CreateText(string.Format("scripts/{0}.rb", directory));
            source.AutoFlush = true;

            // Let's get some requirements in there
            foreach (var r in require)
                source.WriteLine("require '{0}'", r);
            source.WriteLine();

            // And now the class name :)
            source.WriteLine("class {0}\r\n", directory);

            // Now, let's add in those includes
            foreach (var i in include)
                source.WriteLine("    include {0}", i);
            source.WriteLine("    include Minerva");

            // This class needs a constructor!
            source.WriteLine(@"
    def initialize()
        @Globals = IronRuby.globals
    end
");

            // Get the filenames of each source file in the specified directory
            string[] files = Directory.GetFiles(string.Format("scripts/{0}", directory), "*.rb", SearchOption.AllDirectories);

            // Here's where we start copying each source file into our concatenated source file
            foreach (var f in files)
            {
                var file = File.OpenText(f);
                source.Write("{0}\r\n\r\n", file.ReadToEnd().Insert(0, "\t").Replace("\r\n", "\r\n\t"));
                file.Close();
                file.Dispose();
            }

            // Lastly, we just need to close off the class
            source.Write("end");

            // Done :D
            source.Close();
            source.Dispose();
        }

        /// <summary>Creates an instance of the specified class.</summary>
        /// <param name="className">The name of the class to create an instance of.</param>
        public void CreateInstance(string className)
            { _instance = _engine.Execute(string.Format("x = {0}.new", className), _scope); }

        /// <summary>Gets the value of a global variable from the runtime associated with this scripting instance.</summary>
        /// <param name="name">The name of the global variable.</param>
        /// <returns>The value of the specified global variable.</returns>
        public dynamic GetGlobal(string name)
            { return _runtime.Globals.GetVariable(name); }

        /// <summary>Gets the value as T of a global variable from the runtime associated with this scripting instance.</summary>
        /// <typeparam name="T">The desired type of the return value.</typeparam>
        /// <param name="name">The name of the global variable.</param>
        /// <returns>The value as T of the specified global variable.</returns>
        public T GetGlobal<T>(string name)
            { return _runtime.Globals.GetVariable<T>(name); }

        /// <summary>Checks to see if the provided method name exists in our class instance.</summary>
        /// <param name="method">The method name to check.</param>
        /// <returns>True if the method exists.</returns>
        public bool CanInvoke(string method)
            { return _engine.Operations.ContainsMember(_instance, method, true); }

        /// <summary>Invokes a member of the specified class.</summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        /// <returns>The invoked method's return value.</returns>
        public dynamic Invoke(string method, params object[] parameters)
            { return _engine.Operations.InvokeMember(_instance, method, parameters); }

        /// <summary>Executes the specified script file.</summary>
        /// <param name="file">The name of the script file to execute.</param>
        public void Run(string name)
            { _scope = _engine.ExecuteFile(string.Format("scripts/{0}.rb", name), _scope); }

        /// <summary>Sets the value of a global variable of the runtime associated with this scripting instance.</summary>
        /// <param name="name">The name of the global variable.</param>
        /// <param name="value">The desired value of the global variable.</param>
        public void SetGlobal(string name, object value)
            { _runtime.Globals.SetVariable(name, value); }

        public string Execute(string command)
        {
            try { _engine.Execute(command, _scope); }
            catch (Exception e) { return e.Message; }

            return null;
        }

        public void ExecuteFile(string file)
            { _scope = _engine.ExecuteFile(file, _scope); }

        public string Fetch(string domain, string file)
        {
            file = string.Format("http://{0}.pastebin.com/download.php?i={1}", domain, file);

            var client = new System.Net.WebClient();
            Stream netstream = client.OpenRead(file);
            var stream = new MemoryStream();
            netstream.CopyTo(stream);
            netstream.Close();

            var reader = new StreamReader(stream);

            var lines = new System.Collections.Generic.List<string>();

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            string full = reader.ReadToEnd();

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            string line = reader.ReadLine();

            while (line[0] == '#')
            {
                lines.Add(line);
                line = reader.ReadLine();
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            foreach (var l in lines)
            {
                var ls = l.Split(' ');

                if (ls[0] == "#@name")
                {
                    file = string.Format("scripts/fetched/{0}.rb", ls[1]);

                    if (File.Exists(file)) File.Delete(file);

                    StreamWriter saved = File.CreateText(file);

                    saved.Write(full);

                    //saved.Flush();

                    saved.Close();
                    reader.Close();
                    stream.Close();
                    client.Dispose();

                    return file;
                }
            }

            return null;
        }
    }
}