using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFCProtocolTesting.Forms
{
    public partial class ChatForm : Form
    {
        private readonly int port;
        public ChatForm(int port)
        {
            InitializeComponent();
            this.port = port;
        }

    }
}
