using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace RFCProtocolTesting
{
    public sealed class LogManager
    {
        private LogManager()
        {
        }

        public bool writeToLogFile = false;
        public bool writeToSQL = false;
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

            if (writeToSQL)
            {
                try
                {
                    string db = @"URI=file:C:\Users\bgeorge\source\repos\RFCProtocolTesting\RFCProtocolTesting\Logging\serverLog.db";

                    using (var con = new SQLiteConnection(db))
                    {
                        con.Open();

                        using (var cmd = new SQLiteCommand(con))
                        {

                            cmd.CommandText = "INSERT INTO FRCLog(logTime, message) VALUES('" + DateTime.Now.ToString() + "','" + body.Replace("'", "\"") + "')";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (locker)
                    {
                        StringBuilder log = new StringBuilder();
                        log.Append(DateTime.Now.ToString());
                        log.Append(" --- ");
                        log.Append(e.ToString());
                        log.Append(Environment.NewLine);
                        log.Append(body);
                        log.Append(Environment.NewLine);
                        File.AppendAllText("C:\\Users\\bgeorge\\source\\repos\\RFCProtocolTesting\\RFCProtocolTesting\\Logging\\server.log", log.ToString());
                    }
                }

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
