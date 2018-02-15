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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

#endregion

namespace Minerva
{
    internal class IniReader
    {
        object mlock = new object();
        string filename = null;
        bool lazy = false;
        Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();
        bool cacheModified = false;

        public string FileName { get { return filename; } }

        public IniReader(string filename, bool lazy = false)
        {
            this.filename = filename;
            this.lazy = lazy;

            if (!lazy)
                Refresh();
        }

        public void Refresh()
        {
            lock (mlock)
            {
                StreamReader reader = null;

                try
                {
                    sections.Clear();

                    try { reader = new StreamReader(filename); }
                    catch (Exception) { return; }

                    Dictionary<string, string> currentSection = null;
                    string s;

                    while ((s = reader.ReadLine()) != null)
                    {
                        s = s.Trim();

                        if (s.StartsWith(";"))
                            continue;

                        if (s.StartsWith("[") && s.EndsWith("]"))
                        {
                            if (s.Length > 2)
                            {
                                string name = s.Substring(1, s.Length - 2);

                                if (sections.ContainsKey(name))
                                    currentSection = null;
                                else
                                {
                                    currentSection = new Dictionary<string, string>();
                                    sections.Add(name, currentSection);
                                }
                            }
                        }
                        else if (currentSection != null)
                        {
                            int i;

                            if ((i = s.IndexOf('=')) > 0)
                            {
                                int j = s.Length - i - 1;
                                string key = s.Substring(0, i).Trim();

                                if (key.Length > 0)
                                {
                                    if (!currentSection.ContainsKey(key))
                                    {
                                        string value = (j > 0) ? (s.Substring(i + 1, j).Trim()) : "";
                                        currentSection.Add(key, value);
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (reader != null)
                        reader.Close();

                    reader = null;
                }
            }
        }

        public void Flush()
        {
            lock (mlock)
            {
                if (!cacheModified)
                    return;

                cacheModified = false;

                StreamWriter writer = new StreamWriter(filename);

                try
                {
                    bool first = false;

                    foreach (var s in sections)
                    {
                        var section = s.Value;

                        if (first)
                            writer.WriteLine();

                        first = true;

                        writer.Write('[');
                        writer.Write(s.Key);
                        writer.WriteLine(']');

                        foreach (var d in section)
                        {
                            writer.Write(d.Key);
                            writer.Write('=');
                            writer.WriteLine(d.Value);
                        }
                    }
                }
                finally
                {
                    if (writer != null)
                        writer.Close();

                    writer = null;
                }
            }
        }

        public string GetValue(string section, string key, string defaultValue)
        {
            string result;

            if (lazy)
            {
                lazy = false;
                Refresh();
            }

            lock (mlock)
            {
                Dictionary<string, string> sect;

                if (!sections.TryGetValue(section, out sect) || !sect.TryGetValue(key, out result))
                    return defaultValue;

                return result;
            }
        }

        string EncodeByteArray(byte[] value)
        {
            Contract.Requires(value != null);

            var result = "";

            foreach (var v in value)
                result += v.ToString("X2");

            return result;
        }

        byte[] DecodeByteArray(string value)
        {
            Contract.Requires(value != null);

            var result = new byte[value.Length / 2];

            for (int i = 0; i < result.Length; i++)
                result[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);

            return result;
        }

        public bool GetValue(string section, string key, bool defaultValue)
        {
            string val = GetValue(section, key, defaultValue.ToString());

            return (val.ToLower() == "true") ? true : false;
        }

        public int GetValue(string section, string key, int defaultValue)
        {
            string val = GetValue(section, key, defaultValue.ToString());

            int result;

            if (int.TryParse(val, out result))
                return result;

            return defaultValue;
        }

        public double GetValue(string section, string key, double defaultValue)
        {
            string val = GetValue(section, key, defaultValue.ToString());

            double result;

            if (double.TryParse(val, out result))
                return result;

            return defaultValue;
        }

        public byte[] GetValue(string section, string key, byte[] defaultValue)
        {
            string val = GetValue(section, key, EncodeByteArray(defaultValue));

            try { return DecodeByteArray(val); }
            catch (Exception) { return defaultValue; }
        }
    }
}