using OpenQA.Selenium.PhantomJS;
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
using System.Text.RegularExpressions;
using MahApps.Metro.Controls;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Betting_Algorithm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    /// 
    public partial class MainWindow : MetroWindow
    {
        public Parser WEBParser;
        //Proxy IP_Proxy;
        public static MainWindow main;

       public ConcurrentQueue<string> UIQueue = new ConcurrentQueue<string>();

        public MainWindow()
        {
            main = this;
            WEBParser = new Parser();
            InitializeComponent();

            //IP_Proxy = new Proxy();
            

            Thread UIThread = new Thread(UpdateUI_Thread);
            UIThread.Start();


        }

        
        public void UpdateUI_Thread()
        {
            while (true)
            {
                if(UIQueue.Count > 0)
                    if(UIQueue.TryDequeue(out string result))
                        Dispatcher.BeginInvoke(new Action(() => { Prices.Text += result + "\n"; }));
            }
        }

        public void UpdatePricesFromProxy(string text)
        {
            Dispatcher.BeginInvoke(new Action(() => { TextBox_Proxy_List.Text += text + "\n"; }));
        }


        //Stopwatch sw = new Stopwatch();
        private void ParseButton_Click(object sender, RoutedEventArgs e)
        {

            //Console.WriteLine(IP_Proxy.GetProxyIP());
            //Console.WriteLine(IP_Proxy.GetNextProxy());
            WEBParser.InitThreads();
            //GetData("https://csgoempire.com");
        }
    }
}
