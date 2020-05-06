using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using RFCProtocolTesting.AsyncListener;

namespace RFCProtocolTesting.Forms
{
    public partial class ChatForm : Form
    {
        private readonly int port;
        private bool closePending = false;
        private string reply = "";
        static object replyChangeLocker = new object();

        private TCPListener chatListener;
        public ChatForm(int port)
        {
            InitializeComponent();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;


            this.port = port;
            chatListener = new TCPListener(true);
            object[] portAndListenerAndReply = new object[] { port, chatListener, reply };
            backgroundWorker1.RunWorkerAsync(portAndListenerAndReply);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            chatListener.StopListening();
            this.Close();
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!backgroundWorker1.CancellationPending)
            {
                object[] portAndListenerAndReply = e.Argument as object[];
                Int32 port = int.Parse(portAndListenerAndReply[0].ToString());

                chatListener = portAndListenerAndReply[1] as TCPListener;

                var incomingMessages = chatListener.Listen(port);
                foreach (string[] dataForUI in incomingMessages)
                {
                    worker.ReportProgress(1, dataForUI);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MessageBox.Show(e.UserState.ToString());
        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (closePending) this.Close();
            closePending = false;
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                closePending = true;
                backgroundWorker1.CancelAsync();
                chatListener.StopListening();
                e.Cancel = true;
                this.Enabled = false;
                return;
            }
            base.OnFormClosing(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            lock(replyChangeLocker)
            {
                reply = textBox1.Text;
            }
        }
    }
}
