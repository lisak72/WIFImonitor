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
    class WDevice : System.Windows.Controls.Button , IDisposable
    {
        public string name { get; }
        public string ipaddr { get; }
        public bool live { get; set; }
        public static int WDrow=0; //0-8, 
        public static int WDcolumn=0; //0-3 0 is reserved for check button
        
        //constructor
        public WDevice(/*System.Windows.Controls.Grid grd,*/ string name, string ipaddr) 
            : base()
        {
            this.name = name;
            this.ipaddr = (ipaddr.Replace(" ",""));
            this.Content = name;
            SetButtonProperty();
            SetPosition();
        }

        protected void SetButtonProperty()
        {
 //           this.Name = name;
            this.live = false;
            this.MouseEnter += new System.Windows.Input.MouseEventHandler(button_MouseEnter);
            this.MouseLeave += new System.Windows.Input.MouseEventHandler(button_MouseLeave);

        }

        //setting static values WDrow and WDcolumn
        protected void SetPosition()
        {
            if (WDcolumn < 3) WDcolumn++; //new column only
            else { //new line
                    WDrow++;
                    WDcolumn = 0;
                }
            if (WDrow > 8)
            {
                throw new IndexOutOfRangeException ("Too many devices in WIFIMonitor.cfg");
            }
        }

        protected void button_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                Live();
                this.Content = name + "\n" + ipaddr;
            }
            catch (Exception ex1)
            {
                this.Content="Live() other exception \n" + ex1.Message;
            }
        }

        protected void button_MouseLeave(object sender, EventArgs e)
        {
            this.Content = name;
        }

        virtual public bool Live()
        {
            try
            {
                this.live = PingHost(ipaddr);
                if (live) this.Background = Brushes.GreenYellow;
                else this.Background = Brushes.Purple;
                return this.live;
            }
            catch
            {
                throw new Exception("Exception in Live() in possition " + this.name + " " + this.ipaddr);
            }
        }
      
        public void Dispose(){}

        protected bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;            
            try
            { 
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress,1000);
                pingable = (reply.Status == IPStatus.Success);
            }
            catch (PingException exc)
            {
                pinger.Dispose();
                throw(exc);
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            
            return pingable;
        }
    }
}
