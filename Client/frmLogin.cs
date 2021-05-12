using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyLibrary;
using static MyLibrary.mylib;

namespace Client
{
    public partial class frmLogin : Form
    {

        Socket mySocket = null;
        PacketInfo packet = null;
        string id = null;
        public frmLogin()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string choose = GetLog_in_up();

            string uid = textBox1.Text.Trim();
            string pwd = textBox2.Text.Trim();
            if (choose == radioButton1.Text)
            {
                if (uid != null)
                {
                    SQLDB db = new SQLDB(@"Data Source=192.168.0.85;Initial Catalog=myDB;Persist Security Info=True;User ID=kosta;Password=kosta");
                    if (pwd == db.Get($"select pwd from users where uid='{uid}'").ToString().Trim())
                    {
                        mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        mySocket.Connect("192.168.0.85", 9000); // Daemon Serverprocess
                        if (mySocket != null)
                        {
                            string chn = mySocket.LocalEndPoint.ToString().Split(':')[1];
                            packet = new PacketInfo(chn, uid, "1", "0", "");
                        }
                        id = uid;
                        string pkg = packet.makePacket();
                        mySocket.Send(Encoding.Default.GetBytes(pkg));
                        this.Visible = false;
                        frmMain fmain = new frmMain(mySocket, uid);
                        fmain.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show("Check your ID and PWD");
                }

            }
            else
            {
                if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(pwd))
                {
                    MessageBox.Show("빈 값은 저장할 수 없습니다");

                    return;
                }
                else
                {
                    SqlConnection sqlconn = new SqlConnection();
                    SqlCommand sqlcmd = new SqlCommand();

                    sqlconn.ConnectionString = @"Data Source=192.168.0.85;Initial Catalog=myDB;Persist Security Info=True;User ID=kosta;Password=kosta";
                    sqlconn.Open();
                    sqlcmd.Connection = sqlconn;

                    string sql = $"INSERT INTO users VALUES('{uid}','{pwd}','100')";
                    sqlcmd.CommandText = sql;

                    SqlCommand cmd = new SqlCommand(sql, sqlconn);
                    cmd.ExecuteNonQuery();

                }
            }
            

        }

        private string GetLog_in_up()
        {
            if (radioButton1.Checked) return radioButton1.Text;
            else return null;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }
    }
}
