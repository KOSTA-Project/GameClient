using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;
using System.Threading;
using System.Timers;

namespace Client
{
    public partial class WordGame : Form
    {
        string url = null;
        string apikey = null;
        string type = null;
        List<string> history = new List<string>();

        XmlDocument xml = new XmlDocument();

        Socket mySocket = null;

        PacketInfo packet = null;
        string uid = null;

        Thread socketThread = null;

        System.Timers.Timer t = null;

        public WordGame()
        {
            InitializeComponent();
            //Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //mySocket.Connect("192.168.0.85", 9000);
            //string chanel = mySocket.LocalEndPoint.ToString().Split(':')[1];
            //mySocket.Send(Encoding.Default.GetBytes($"{chanel},park,1,1,"));
            //url = "https://krdict.korean.go.kr/api/search?key=";
            //apikey = "EBB6D3290D88C645CF1452F7DA3229D0";
            //type = "&part=word&pos=1&q=";
            //socketThread = new Thread(socketListener);
            //socketThread.Start();

            //timer1.Start();
        }

        public WordGame(Socket ss, string uid)
        {
            InitializeComponent();
            
            mySocket = ss;
            string sess = mySocket.LocalEndPoint.ToString().Split(':')[1];
            this.uid = uid;
            //packet = pi;
            url = "https://krdict.korean.go.kr/api/search?key=";
            apikey = "EBB6D3290D88C645CF1452F7DA3229D0";
            type = "&part=word&pos=1&q=";

            packet = new PacketInfo(sess,uid,"1","1","");
            socketThread = new Thread(socketListener);
            socketThread.Start();

            t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);


        }
        delegate void CB(string str);
        public void changeTimer(string str)
        {
            if (textBox1.InvokeRequired)
            {
                CB cb = new CB(changeTimer);
                Invoke(cb, new object[] { str });
            }
            else
            {
                textBox1.Text = str;
            }
        }

        public void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            int count = int.Parse(textBox1.Text);
            if (--count == 0)
            {
                // 시간 초과로 인해 게임이 끝난다.
                // 소켓에 담아서 보내줘야 한다.
                //timer1.Stop();
                t.Stop();

                //wordlist.Text += "-----Game Over-----\r\n";
                MessageBox.Show("당신이 패배했습니다.");
                //wordInput.Enabled = false;
                //this.Close();
            }
            string c = count.ToString();
            changeTimer(c);
        }


        public void socketListener()
        {
            if (mySocket != null)
            {
                while (true)
                {
                    int n = mySocket.Available;
                    if (n > 0)
                    {
                        byte[] bArr = new byte[n];
                        mySocket.Receive(bArr);
                        string pkg = Encoding.Default.GetString(bArr);

                        if (pkg.Contains(','))
                        {
                            string[] msg = pkg.Split(',');
                            packet = new PacketInfo(msg[0], msg[1], msg[2], msg[3], msg[4]);

                        }
                        string str = packet.getMessage(pkg);
                        if (str == "gamestart")
                        {
                            //timer1.Enabled = true;
                            //timer1.Start();
                            t.Start();
                        }
                        if (str == "lose")
                        {
                            t.Stop();
                            MessageBox.Show("당신이 이겼습니다.");
                        }


                    }
                }
            }
        }


        public bool searchWord(string search)
        {
            // Query문 만들기
            string query = url + apikey + type + search;
            // Request문 보내기.
            WebRequest request = WebRequest.Create(query);
            request.Method = "GET";

            // Response 받기.
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string result = reader.ReadToEnd();

            // XML로 만들어 주기.
            xml.LoadXml(result);
            stream.Close();


            XmlNodeList xnlist = xml.GetElementsByTagName("item");
            XmlNodeList word_count = xml.GetElementsByTagName("total");
            int count = int.Parse(word_count[0].InnerText);

            if (count == 0)
            {
                wordlist.Text += "존재하지 않습니다\r\n";
                return false;
            }
            else
            {
                string mean = xnlist[0]["sense"]["definition"].InnerText;
                wordlist.Text += mean + "\r\n";
                return true;
            }
        }

        public bool checkLastWord(string word)
        {
            if (history.Count == 0)
                return true;
            else
            {
                string lastword = history.Last();
                if (lastword[lastword.Length - 1] == word[0])
                    return true;
                else
                    return false;
            }
        }

        public bool checkDuplicate(string word)
        {
            if (history.Contains(word))
                return true;
            else
                return false;
        }


        private void wordInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string word = wordInput.Text;
                wordlist.Text += word + "\r\n";
                if (checkLastWord(word) == false)
                {
                    wordlist.Text += "끝 단어와 일치하지 않습니다.\r\n";
                    wordInput.Clear();
                }
                else
                {
                    if (checkDuplicate(word) == true)
                    {
                        wordlist.Text += "사용된 단어 입니다.\r\n";
                        wordInput.Clear();
                    }
                    else // 진짜 성공했을 때.
                    {
                        if (searchWord(word) == true)
                        {
                            // 소켓에 내가 성공했다는걸 보내줘야 한다.
                            //mySocket.Send();
                            history.Add(wordInput.Text);
                            textBox1.Text = "5";
                        }

                        wordInput.Clear();
                    }
                }


            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            int count = int.Parse(textBox1.Text);
            if (--count == 0)
            {
                // 시간 초과로 인해 게임이 끝난다.
                // 소켓에 담아서 보내줘야 한다.
                timer1.Stop();
                wordlist.Text += "-----Game Over-----\r\n";
                MessageBox.Show("당신이 패배했습니다.");
                wordInput.Enabled = false;
            }
           
            textBox1.Text = count.ToString();
        }

        private void WordGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            socketThread.Abort();
        }


    }
}
