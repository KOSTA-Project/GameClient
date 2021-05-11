using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class PacketInfo
    {
        string channel;
        string id;
        string alive;
        string state;
        string message;

        public PacketInfo(string channel, string id, string alive, string state, string message)
        {
            this.channel = channel;
            this.id = id;
            this.alive = alive;
            this.state = state;
            this.message = message;
        }

        public string makePacket()
        {
            string result = "";
            result += channel + ",";
            result += id + ",";
            result += alive + ",";
            result += state + ",";
            result += message;
            return result;
        }
        public void setAlive(string target)
        {
            this.alive = "1";
        }

        public void setState(string target)
        {
            this.state = target;
        }

        public void setMessage(string target)
        {
            this.message = target;
        }

        public string getMessage(string pkg)
        {
            string result = pkg.Split(',')[4].Split('/')[1];
            return result;
        }
    }
}
