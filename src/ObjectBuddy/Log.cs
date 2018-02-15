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
using System.Threading;
using System.IO;

#endregion

namespace Minerva
{
    public static class Log
    {
        const int SILENT_MODE = 0;
        const int NORMAL_MODE = 1;
        const int DEBUG_MODE = 2;
        public static int Level { get; set; }                           // logging level

        public static ConsoleColor DefaultBG = ConsoleColor.Black;      // the default background colour
        public static ConsoleColor DefaultFG = ConsoleColor.DarkGreen;  // the default foreground colour

        static ConsoleColor IPC_FG = ConsoleColor.DarkRed;              // foreground colour for IPC network messages
        static ConsoleColor error = ConsoleColor.Red;                   // error message colour
        static ConsoleColor warning = ConsoleColor.Yellow;              // warning message colour
        static ConsoleColor notice = ConsoleColor.Green;                // notice message colour

        static string server = "none";                                  // server name
        static Queue<LogMessage> messages = new Queue<LogMessage>();
        static bool stopped;

        static void Listen()
        {
            while (!stopped)
            {
                if (messages.Count > 0)
                {
                    lock (messages)
                    {
                        Message(messages.Dequeue());
                    }
                }
                else
                {
                    Thread.Sleep(150);
                }
            }
        }

        static void Message(LogMessage m)
        {
            // only fatal errors and notices
            if (Level == SILENT_MODE)
            {
                if (m.Tag == String.Empty || m.Tag == "ERROR" || m.Tag == "WARNING")
                    return;
            }

            // errors and warnings
            if (Level == NORMAL_MODE)
            {
                if (m.Tag == String.Empty)
                    return;
            }

            // errors, warnings and networking
            // level == DEBUG_MODE

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMsg = "";
            string message = m.Message;
            string tag = m.Tag;
            ConsoleColor colour = m.Colour;

            Console.Write("[{0}] ", time);

            if (tag != "")
            {
                bool bright = !((int)colour < 8);
                Console.BackgroundColor = !bright ? (ConsoleColor)((int)colour | 8) : (ConsoleColor)((int)colour ^ 8);
                Console.ForegroundColor = !bright ? ConsoleColor.Black : ConsoleColor.White;
                Console.Write(" ##{0}## ", tag);
                logMsg = string.Format("##{0}## ", tag);
            }

            Console.BackgroundColor = DefaultBG;
            Console.ForegroundColor = colour;

            if (tag != "")
            {
                Console.Write(" {0}\n", message);
                logMsg = string.Format("[{0}] {1}{2}", time, logMsg, message);
            }
            else
            {
                Console.Write("{0}\n", message);
                logMsg = string.Format("[{0}] {1}", time, message);
            }

            Console.ForegroundColor = DefaultFG;
            WriteFile(logMsg);
        }

        public static void Received(string type, int opcode, int size)
        {
            Message(String.Format("Received packet: CSC_{0} (opcode: {1}, size: {2})", type, opcode, size), DefaultFG);
        }

        public static void Sent(string type, int opcode, int size)
        {
            Message(String.Format("Sent packet: SSC_{0} (opcode: {1}, size: {2})", type, opcode, size), DefaultFG);
        }

        public static void IPC_Received(IPC opcode, int size)
        {
            Message(String.Format("Received IPC packet: {0} (opcode: {1}, size: {2})", opcode, (byte)opcode, size), IPC_FG);
        }

        public static void IPC_Sent(IPC opcode, int size)
        {
            Message(String.Format("Sent IPC packet: {0} (opcode: {1}, size: {2})", opcode, (byte)opcode, size), IPC_FG);
        }

        public static void FatalError(string message)
        {
            Message(message, error, "FATAL ERROR");
        }

        public static void FatalError(string format, params object[] arg)
        {
            FatalError(String.Format(format, arg));
        }

        public static void Error(string message)
        {
            Message(message, error, "ERROR");
        }

        public static void Error(string format, params object[] arg)
        {
            Error(String.Format(format, arg));
        }

        public static void Notice(string message)
        {
            Message(message, notice, "NOTICE");
        }

        public static void Notice(string format, params object[] arg)
        {
            Notice(String.Format(format, arg));
        }

        public static void Warning(string message)
        {
            Message(message, warning, "WARNING");
        }

        public static void Warning(string format, params object[] arg)
        {
            Warning(String.Format(format, arg));
        }

        public static void Message(string message, ConsoleColor colour, string tag = "")
        {
            lock (messages)
            {
                messages.Enqueue(new LogMessage(message, colour, tag));
            }
        }

        public static void Start(string server, bool log = false, int lgLvl = 0)
        {
            Console.ForegroundColor = DefaultFG;
            Log.server = server;
            Log.Level = DEBUG_MODE;

            Message("Starting Log Service...", Log.DefaultFG);

            var t = new Thread(new ThreadStart(Listen));
            t.Start();
        }

        public static void Stop()
        {
            stopped = true;
        }

        static void WriteFile(string args)
        {
            string address = "logs/" + server + ".log";
            StreamWriter logFile = null;

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            if (!File.Exists(address))
                logFile = new StreamWriter(address);
            else
                logFile = File.AppendText(address);

            logFile.WriteLine(args);
            logFile.Close();
            logFile.Dispose();
        }
    }

    public struct LogMessage
    {
        public string Message;
        public ConsoleColor Colour;
        public string Tag;

        public LogMessage(string message, ConsoleColor colour, string tag = "")
        {
            Message = message;
            Colour = colour;
            Tag = tag;
        }
    }
}