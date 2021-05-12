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
    public partial class NBGame : Form
    {

        Socket player = null;
        string uid = null;

        Thread threadRead = null;

        int round_cnt = 0;
        int nb_ans = -1;
        int[] numcnt = new int[10];
        bool isWinner = false;
        const int round_time = 20000;       // round 하나의 시간
        int sec = 20;           // 한 round 카운트다운용

        int nb_len = 4;
        string query = "";

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
            player = sock;
            uid = id;

            threadRead = new Thread(ReadProcess);
            threadRead.IsBackground = true;
            threadRead.Start();

            //packetinfo 만들어주세요. 

            //lbUser1.Text = $"ID: {}"; // 괄호 안에 player소켓의 아이디
            timer1.Interval = 1000;     // 1초마다 카운트다운
        }

        void ReadProcess()
        {
            while (true)
            {
                if (isAlive(player))
                {
                    byte[] ba = new byte[player.Available];
                    player.Receive(ba); //socket이름 player

                    // 이부분 수정 필요----------------------------------------> 서버에서 받은 msg
                    // 이부분 수정 필요----------------------------게임 시작 시작 시, (1) 클->서버 "gamestart"
                    // 이부분 수정 필요----------------------------게임 시작 시작 시, (2) 서버->클 "gamestart/상대id/문제정답"
                    // 이부분 수정 필요----------------------------게임 중간, 클->서버 "false(또는 true)" ==> 해당 클이 winner인지
                    // 이부분 수정 필요-------게임 중간, 서버는 두 클의 isWinner가 true인거 하나라도 있으면
                    // 서버->클 "gameend/true(false)"-> 상대방isWinner인지 패킷 전송  / isWinner=true인 클에게 money추가
                    // 이부분 수정 필요-------게임 중간, 두 클 모두 isWinner=false면 "continue"

                    string msg = Encoding.Default.GetString(ba);
                    if (msg.StartsWith("gameend")) endGame(msg);
                    else
                    {
                        if (msg.StartsWith("gamestart")) startGame(msg);
                        nextRound();
                    }
                }
            }
        }

        // 게임 시작 메서드
        void startGame(string msg) //msg = 
        {
            string[] sa = msg.Split('/');
            lbUser2.Text = $"상대방 ID: {sa[1]}";
            isWinner = false;

            nb_ans = int.Parse(sa[2]);
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
            timer1.Start();
        }

        // 게임 끝 메서드
        void endGame(string msg)
        {
            // 상대방 이김

            if (msg.Split('/')[1] == "true")
            {
                if (isWinner) tbMain.Text = "비김";
                else tbMain.Text = "짐";
            }
            else tbMain.Text = "이김";
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

        // 서버로 정보 전송
        void SendToServer(string msg)
        {
            player.Send(Encoding.Default.GetBytes(msg));
        }

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

        // 게임 준비 완료
        private void btnStart_Click(object sender, EventArgs e)
        {
            string msg = "gamestart";
            SendToServer(msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            sec--;
            lbTimer.Text = $"남은 시간: {sec}초";
            if (sec > 0) return;
            timer1.Stop();

            AddText($"=========== Round {round_cnt} ===========");

            if (query == null)
            {
                AddText("입력된 질문이 없었습니다. 라운드하나 날림 ㅎ");
            }
            else
            {
                string result = queryResult(query);
                tbQuery.Text = "";
                AddText($"{query} >> {result}");   // 상대방 결과 반환과 동시에 해야할지
            }

            SendToServer($"{isWinner}/{round_cnt}");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string msg = tbQuery.Text;
            if (!isValidQuery(msg))
            {
                AddText("잘못된 질문입니다. 다시 입력해주세요");
                return;
            }
            query = msg;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbMemo.Clear();
        }
    }
}
