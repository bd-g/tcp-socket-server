using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFCProtocolTesting
{
    public enum ResponseSetting
    {
        Echo = 1,
        Static = 2, 
        File = 3
    }
    public partial class ServerForm : Form
    {
        private bool isListening;
        private TCPListener tCPListener;
        private ConnectionCounter connectionCounter;

        public ServerForm()
        {
            InitializeComponent();
            isListening = false;
            //textBox1.KeyPress += new KeyPressEventHandler(textBox1_KeyPress);
            connectionCounter = new ConnectionCounter();

            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            if (isListening)
            {
                backgroundWorker1.CancelAsync();
                tCPListener.StopListening();
                if (backgroundWorker1.IsBusy == true)
                {
                    System.Threading.Thread.Sleep(100);
                }
                button1.Text = "Listen";
                isListening = !isListening;
                button1.Enabled = true;
                lock (connectionCounter)
                {
                    int oldValue = connectionCounter.ResetCurrentCounter();
                    label6.Text = "0";
                }
            }
            else
            {
                if (textBox1.Text.All(c => c >= '0' && c <= '9'))
                {
                    if (backgroundWorker1.IsBusy != true)
                    {
                        backgroundWorker1.RunWorkerAsync(textBox1.Text);
                    }
                }
                else
                {
                    MessageBox.Show("Enter a valid port number");
                }
            }
        }

     
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

     

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            while (!backgroundWorker1.CancellationPending)
            {
                Int32 port = int.Parse(e.Argument.ToString());
                worker.ReportProgress(0, "Stop listening");
                tCPListener = new TCPListener();

                var incomingConnections = tCPListener.Listen(port);
                foreach (string connection in incomingConnections)
                {
                    worker.ReportProgress(1, connection);
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (e.ProgressPercentage == 0)
            {
                button1.Text = "Stop Listening";
                button1.Enabled = true;
                isListening = !isListening;
            }
            else
            {
                textBox2.Text = e.UserState.ToString();
                lock(connectionCounter)
                {
                    label5.Text = connectionCounter.IncrementTotalCounter().ToString();
                    label6.Text = connectionCounter.IncrementCurrentCounter().ToString();
                }
                
            }
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
        }

        private void responseSettingChanged(object sender, EventArgs e)
        {
            ResponseSetting currentSetting = (radioButton1.Checked) ? ResponseSetting.Echo :
                                                                          radioButton2.Checked ? ResponseSetting.Static :
                                                                                                 ResponseSetting.File;
            ResponseManager.Instance.responseSetting = currentSetting;

            if (radioButton2.Checked)
            {
                textBox3.Enabled = true;
            }
            else
            {
                textBox3.Enabled = false;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ResponseManager.Instance.staticResponse = ((TextBox)sender).Text;
        }
    }
}
