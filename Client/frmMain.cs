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
        Socket mySocket = null;
        PacketInfo packet = null;
        string uid = null;

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(Socket socket, string id)
        {
            InitializeComponent();
            mySocket = socket;
            uid = id;
            tbShow.Text = $"{uid}님 환영합니다. 게임을 선택해 주세요.";
        }



        private void frmMain_Load(object sender, EventArgs e)
        {

        }


        private void btn_NB_Click(object sender, EventArgs e)
        {
            /*
            숫자 야구 관련 Form을 불러오자.
            */

            packet.setState("2");
            //string msg = packet.makePacket();
            //mysocket.Send(Encoding.Default.GetBytes(msg));


            // this.Visible =false;
            // Form lalaa = new Form();
            // lalaa.ShowDialog();
            // this.Visible=True;
        }

        private void btn_Word_Click(object sender, EventArgs e)
        {

            this.Visible = false;
            // Word 게임 선택했다고 보내주기.

            string chn = mySocket.LocalEndPoint.ToString().Split(':')[1];

            packet = new PacketInfo(chn, uid, "1", "0", "");
            packet.setState("1");
            string msg = packet.makePacket();

            mySocket.Send(Encoding.Default.GetBytes(msg));

            WordGame wordgame = new WordGame(mySocket, uid);

            wordgame.ShowDialog();

            this.Visible = true;
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mySocket.Shutdown(SocketShutdown.Both);
            mySocket.Close();
            //Login page로 돌아가기.

        }
    }
}
