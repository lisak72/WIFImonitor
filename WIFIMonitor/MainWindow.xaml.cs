using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Timers;
using System.Net.NetworkInformation;

namespace WIFIMonitor
{
    // <copyright Jiri Liska">
    // Copyright (c) 2018 All Rights Reserved
    // </copyright>
    // <author>Jiri Liska, Trebon, lisak72@seznam.cz</author>
    // <date>05.10.2018</date>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int refresh=360000; //time to automaticaly check in milliseconds

        public MainWindow()
        {
            InitializeComponent();
            FillGrid();
            InitTimer();            
    }

        //system timer
        private Timer timer1;
        public void InitTimer()
        {
            timer1 = new Timer(refresh);            
            timer1.Elapsed += AutoRefreshTimed;
            timer1.AutoReset=true; 
            timer1.Enabled=true;
        }


        //misto listu je tu grid0
        List<WDevice> ListOfDevices = new List<WDevice>();
            //neni
        
        private void Button0_Click(object sender, RoutedEventArgs e)
        {
            Button0.Background = Brushes.Purple;

            try
            {
                CheckAllDevices();
            }
            catch(PingException ex)
            {
                MessageBox.Show("Network socket exception "+ ex.Message);
                Environment.Exit(99);
            }
            Button0.Background = Brushes.GreenYellow;
        }

        private void FillGrid()
        {
           
            try
            {
                using (StreamReader instream = new StreamReader("WIFIMonitor.cfg"))
                {
                    string iline;
                    while ((iline = instream.ReadLine()) != null)
                    {
                        if (iline.StartsWith("#")) //comment
                        {
                            continue;
                        }

                        if (iline.Contains("$refresh"))
                        {
                            Configure(iline);
                            continue;
                        }

                        if (iline.Contains("$H")) //http type, WDeviceHttp
                        {
                            string[] ilArrH = new string[2];
                            ilArrH = iline.Split(';');
                            try
                            {
                                WDeviceHttp newBtn = new WDeviceHttp(ilArrH[0], ilArrH[1]);
                                Grid.SetRow(newBtn, WDevice.WDrow);
                                Grid.SetColumn(newBtn, WDevice.WDcolumn);
                                grid0.Children.Add(newBtn);
                                ListOfDevices.Add(newBtn);
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            catch
                            {
                                MessageBox.Show("Constructor exception");
                            }
                            continue;
                        }



                        string[] ilArr = new string[2];
                        ilArr = iline.Split(';');
                        try
                        {
                            WDevice newBtn = new WDevice(ilArr[0], ilArr[1]);



                            Grid.SetRow(newBtn, WDevice.WDrow);
                            Grid.SetColumn(newBtn, WDevice.WDcolumn);
                            grid0.Children.Add(newBtn);
                            ListOfDevices.Add(newBtn);
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        catch
                        {
                            MessageBox.Show("Constructor exception");
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Configuration file WIFIMonitor.cfg missing or wrong format \n .. or something else happened wrong");
            }
        }

       /* 
        private void PCheckAllDevices()
        {            
            {
                Parallel.ForEach(ListOfDevices, (dev) =>
                        {
                            try
                            {
                                dev.Live();
                            }
                            catch(Exception e) { MessageBox.Show(e.Message); }
                        });
            }
            
        }
       */ 
            
       private void CheckAllDevices()
        {
            Button0.Background = Brushes.BlueViolet;
            int counter = 0;
            foreach (WDevice dev in ListOfDevices)
            {
                try
                {
                    dev.Live();
                    counter++;                    
                }
                catch (PingException ex)
                {
                    MessageBox.Show("Network socket exception " + ex.Message);                   
                    //Environment.Exit(99);
                }
                catch(Exception e)
                {
                    MessageBox.Show("Live() other exception +DISPOSE " + e.Message);
                    DisposeCurrentObject(dev, counter);
                    break;
                }

            }
            Button0.Background = Brushes.Yellow;
        }

        private void AutoRefreshTimed(Object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke((Action) (() => CheckAllDevices()));
        }

        private void Configure(string il)
        {
            try
            {
                string[] pom = il.Split('=');
                refresh = int.Parse(pom[1]);
                InitTimer(); //timer reinit
            }
            catch
            {
                MessageBox.Show("Bad type of refresh line (config), correct is $refresh='value' \n set is "+il+ " continue with "+refresh);
            }
        }

        private void DisposeCurrentObject(WDevice d, int indexD)
        {
           // Grid.SetRow(null, WDevice.WDrow);
          //  Grid.SetColumn(null, WDevice.WDcolumn);
            grid0.Children.Remove(d);
            ListOfDevices.RemoveAt(indexD);
            d.Dispose();
        }

    }
}
