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
using System.Timers;
using System.Windows.Forms;

namespace Client
{
    public partial class NBGame : Form
    {

        Socket player = null;
        string uid = null;
        PacketInfo pi = null;
        string ses = null;
        string msg_room = null;     // "방번호/"

        System.Timers.Timer t = null;
        Thread threadRead = null;

        int round_cnt = 0;
        int nb_ans = -1;
        int[] numcnt = new int[10];
        bool isWinner = false;
        const int round_time = 20000;       // round 하나의 시간
        int sec = 20;           // 한 round 카운트다운용

        int nb_len = 4;
        string query = "";
        string qResult = "";

        delegate void cbAddText(string str);

        void AddText(string str)
        {
            if (tbMain.InvokeRequired)
            {
                cbAddText cb = new cbAddText(AddText);
                Invoke(cb, new object[] { str });
            }
            else
            {
                tbMain.AppendText(str + "\n");
            }
        }
        public NBGame()
        {
            InitializeComponent();
        }

        public NBGame(Socket sock, string id)
        {
            InitializeComponent();

            player = sock;
            uid = id;
            ses = player.LocalEndPoint.ToString().Split(':')[1];
            pi = new PacketInfo(ses, id,"1","2","");

            threadRead = new Thread(ReadProcess);
            threadRead.IsBackground = true;
            threadRead.Start();

            lbUser1.Text = $"ID: {id}"; // 괄호 안에 player소켓의 아이디
            //timer1.Interval = 1000;     // 1초마다 카운트다운

            t = new System.Timers.Timer();
            t.Interval = 1000;
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);

        }
        // timer_tick
        public void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            sec--;
            changeTimer($"남은 시간: {sec}초");
            if (sec > 0) return;

            t.Stop();

            AddText($"=========== Round {round_cnt} ===========");

            if (query == null)
            {
                AddText("입력된 질문이 없었습니다. 라운드하나 날림 ㅎ");
            }
            else
            {
                //string result = queryResult(query);
                
                AddText($"{query} >> {qResult}");   // 상대방 결과 반환과 동시에 해야할지
            }
            // 
            SendToServer($"{isWinner}/{round_cnt}");
        }


        void SendToServer(string msg)
        {
            pi.setMessage(msg_room + msg);
            player.Send(Encoding.Default.GetBytes(pi.makePacket()));
        }


        delegate void CB(string str);
        public void changeTimer(string str)
        {
            if (lbTimer.InvokeRequired)
            {
                CB cb = new CB(changeTimer);
                Invoke(cb, new object[] { str });
            }
            else
            {
                lbTimer.Text = str;
            }
        }

        void ReadProcess()
        {
            while (true)
            {
                int n = player.Available;
                if (n>0)
                {
                    byte[] ba = new byte[n];
                    player.Receive(ba); //socket이름 player
                    string pkt = Encoding.Default.GetString(ba);
                    // 이부분 수정 필요----------------------------------------> 서버에서 받은 msg
                    // 이부분 수정 필요----------------------------게임 시작 시작 시, (1) 클->서버 "gamestart"
                    // 이부분 수정 필요----------------------------게임 시작 시작 시, (2) 서버->클 "gamestart/상대id/문제정답"
                    // 이부분 수정 필요----------------------------게임 중간, 클->서버 "false(또는 true)" ==> 해당 클이 winner인지
                    // 이부분 수정 필요-------게임 중간, 서버는 두 클의 isWinner가 true인거 하나라도 있으면
                    // 서버->클 "gameend/true(false)"-> 상대방isWinner인지 패킷 전송  / isWinner=true인 클에게 money추가
                    // 이부분 수정 필요-------게임 중간, 두 클 모두 isWinner=false면 "continue"
                    if(msg_room==null) msg_room = pi.getRoom(pkt);
                    string msg = pi.getMessage(pkt);

                    if (msg.StartsWith("true") || msg.StartsWith("false"))     // 게임 도중인 경우
                    {
                        endGame(msg);
                    }
                    else
                    {
                        // gamestart받는 경우
                        string numb = pi.getNumber(pkt);
                        if (msg.StartsWith("gamestart")) startGame(numb);
                        nextRound();
                    }
                }
            }
        }

        // 게임 시작 메서드
        void startGame(string msg)
        {
            //lbUser2.Text = $"상대방 ID: {sa[1]}";
            isWinner = false;

            nb_ans = int.Parse(msg);
            round_cnt = 0;
            query = "";

            // numcnt초기화
            numcnt = new int[10];
            int val = nb_ans, div = 1000;
            while (div >= 1)
            {
                int mok = val / div;
                numcnt[mok]++;
                val %= div;
                div /= 10;
            }

            AddText("게임을 시작합니다~!");
            timer1.Start();
        }

        // gameend 아닌 경우 - (다음) 라운드 시작 메서드
        void nextRound()
        {
            query = null;
            round_cnt++;
            sec = round_time / 1000;
            tbQuery.Text = "";
            t.Start();
//            timer1.Start();
        }

        // 게임 끝 메서드
        void endGame(string msg)
        {
            if (msg.StartsWith("true")) // 상대방이 이긴경우
            {
                if (isWinner) AddText("비김");
                else AddText("짐");
            }
            else
            {
                if (isWinner) AddText("이김");
                else nextRound();
            }
        }

        // nb 쿼리 결과
        string queryResult(string msg)
        {
            int q = int.Parse(msg);
            int ball = 0, strike = 0;
            int div = (int)Math.Pow(10, nb_len - 1);

            int ans = nb_ans;
            int[] nums = new int[10];
            Array.Copy(numcnt, nums, 10);

            while (div >= 1)
            {
                int cur = q / div;
                if (ans / div == cur) strike++;
                if (nums[cur] > 0) ball++;
                nums[cur]--;
                q %= div;
                ans %= div;
                div /= 10;
            }
            ball -= strike;
            if (strike == nb_len) isWinner = true;

            qResult = $"{strike} strike, {ball} ball";
            return $"{strike} strike, {ball} ball";
        }

        // 질문 유효 체크
        bool isValidQuery(string str)
        {
            if (str.Length != nb_len) return false;
            foreach (char c in str)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;

        }

        //delegate void cbSend(string msg);


        // 소켓 연결 체크
        bool isAlive(Socket ss)
        {
            if (ss == null) return false;
            if (!ss.Connected) return false;

            bool r1 = ss.Poll(1000, SelectMode.SelectRead);
            bool r2 = ss.Available == 0;
            if (r1 && r2) return false;
            else
            {
                try
                {
                    byte[] b = new byte[1]; b[0] = 0;
                    int sentByteCount = ss.Send(new byte[1], 0, SocketFlags.OutOfBand);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void frmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (threadRead != null) threadRead.Abort();
            if (player != null) player.Close();
        }


        private void btnSubmit_Click(object sender, EventArgs e)
        { 
            if (!t.Enabled) return;
            string msg = tbQuery.Text;
            if (!isValidQuery(msg))
            {
                AddText("잘못된 질문입니다. 다시 입력해주세요");
                return;
            }
            query = msg;
            qResult = queryResult(query);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbMemo.Clear();
        }
    }
}
