using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RFCProtocolTesting
{
    public class TCPListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static bool listening = true;
        private static bool dataReady = false;
        private Socket listener;
        private static string[] dataForUI;
        private readonly object dataLock = new object();

        public TCPListener() { }

        public void StopListening()
        {
            listening = false;
            allDone.Set();
        }

        public IEnumerable<string[]> Listen(int port)
        {
            listening = true;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(localAddr, port);
            listener = new Socket(localAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            while (listening)
            {
                try
                { 
                    allDone.Reset();

                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    allDone.WaitOne();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                if (listening)
                {
                    lock (dataLock)
                    {
                        while (!dataReady) { }
                        yield return dataForUI;
                        dataReady = false;
                    }
                    
                }
            }
            listening = false;
            listener.Close();
            

            yield break;
        }

        public static void AcceptCallback(IAsyncResult ar)
        { 
            allDone.Set();
            if (listening == false) return;

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);


            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static async void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            string body;

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                body = new HttpParser().getBody(content);
            }
            else
            {
                body = "";
            }

            string connection = IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString()) + Environment.NewLine +
                "Port " + ((IPEndPoint)handler.RemoteEndPoint).Port;
            dataForUI = new string[] { connection, body };
            dataReady = true;  

            byte[] response = ResponseManager.Instance.getResponse(body);

            Send(handler, response);

            await LogManager.Instance.logMessage(body);
        }

        private static void Send(Socket handler, byte[] byteData)
        {
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

                int bytesSent = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}