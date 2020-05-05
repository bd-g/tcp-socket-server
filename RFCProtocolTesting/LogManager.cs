using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFCProtocolTesting
{
    public sealed class LogManager
    {
        private LogManager()
        {
        }

        public bool writeToLogFile = false;
        static object locker = new object();

        public async Task logMessage(string body)
        {
            if (writeToLogFile)
            {
                int timeOut = 100;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (true)
                {
                    try
                    {
                        lock (locker)
                        {
                            StringBuilder log = new StringBuilder();
                            log.Append(DateTime.Now.ToString());
                            log.Append(" --- ");
                            log.Append(body);
                            log.Append(Environment.NewLine);
                            File.AppendAllText("C:\\Users\\bgeorge\\source\\repos\\RFCProtocolTesting\\RFCProtocolTesting\\Logging\\server.log", log.ToString());
                        }
                        break;
                    }
                    catch
                    {
                    }
                    if (stopwatch.ElapsedMilliseconds > timeOut)
                    {
                        break;
                    }
                    await Task.Delay(5);
                }
                stopwatch.Stop();
            }
        }

        public static LogManager Instance { get { return Nested.instance; } }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly LogManager instance = new LogManager();
        }
    }
}
