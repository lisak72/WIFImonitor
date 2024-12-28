using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WIFIMonitor
{
    class WDeviceHttp : WDevice
    {
        public WDeviceHttp(string bipname, string bipaddr)
            :base(bipname, bipaddr)
        {           
        }

        public override bool Live()
        {
            try
            {
                this.live = HttpLive(ipaddr);
                if (live) this.Background = Brushes.GreenYellow;
                else this.Background = Brushes.Purple;
                return this.live;
            }
            catch
            {
                throw new Exception("Exception in Live() in possition " + this.name + " " + this.ipaddr);
            }
        }

        private bool HttpLive(string NameOrAddress)
        {
            //HttpClient client = new HttpClient();
            //var response = client.GetAsync("http://" + NameOrAddress);
            try
            {
                HttpWebRequest webrequest = HttpWebRequest.Create("http://" + NameOrAddress) as HttpWebRequest;
                webrequest.Timeout = 6000;
                var response = (HttpWebResponse)webrequest.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.Close();
                    return true;
                }
                response.Close();
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
