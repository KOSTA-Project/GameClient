using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class frmMain : Form
    {
        Socket mysocket = null;
        PacketInfo packet = null;

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(Socket sock, string id)
        {
            InitializeComponent();
            mysocket = sock;
            string s_num = mysocket.LocalEndPoint.ToString().Split(':')[1];
            packet = new PacketInfo(s_num,id,"1","0","");
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            frmLogin login = new frmLogin();
            login.ShowDialog();

        }


        private void btn_NB_Click(object sender, EventArgs e)
        {
            /*
            숫자 야구 관련 Form을 불러오자.
            */

            // this.Visible =false;
            // Form lalaa = new Form();
            // lalaa.ShowDialog();
            // this.Visible=True;
        }

        private void btn_Word_Click(object sender, EventArgs e)
        {

            this.Visible = false;
           // WordGame wordgame = new WordGame(mysocket);
            WordGame wordgame = new WordGame();

            wordgame.ShowDialog();

            this.Visible = true;
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mysocket.Shutdown(SocketShutdown.Both);
            mysocket.Close();
            //Login page로 돌아가기.

        }
    }
}
