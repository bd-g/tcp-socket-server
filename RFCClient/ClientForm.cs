using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFCClient
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
            //textBox2.KeyPress += new KeyPressEventHandler(onlyNumbers);
            //textBox4.KeyPress += new KeyPressEventHandler(onlyNumbers);


            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.All(c => c >= '0' && c <= '9'))
            {
                if (textBox4.Text.All(c => c >= '0' && c <= '9'))
                {
                    if (backgroundWorker1.IsBusy != true)
                    {
                        button1.Enabled = false;
                        backgroundWorker1.RunWorkerAsync(new List<string>()
                        {
                            textBox1.Text,
                            textBox2.Text,
                            textBox4.Text
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Enter a valid number of reqeusts");
                }
            }
            else
            {
                MessageBox.Show("Enter a valid port number");
            }
        }

        private void onlyNumbers(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<string> args = e.Argument as List<string>;
            string data = args[0];
            int port = int.Parse(args[1]);
            int numRequests = int.Parse(args[2]);
            AsynchronousClient asynchronousClient = new AsynchronousClient();
            var responses = asynchronousClient.StartClient(data, port, numRequests);

            foreach (string response in responses)
            {
                worker.ReportProgress(1, response);
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (textBox3)
            {
                textBox3.Text += e.UserState.ToString() + Environment.NewLine + "-----------" + Environment.NewLine;
            }
        }

        // This event handler deals with the results of the background operation.
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
            if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
        }

    }
}
