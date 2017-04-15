using System;
using System.Windows;
using MahApps.Metro.Controls;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

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

        ObservableCollection<DataObject> _ObsCollection_List = new ObservableCollection<DataObject>();

        public MainWindow()
        {
            main = this;
            WEBParser = new Parser();
            InitializeComponent();

            //_ObsCollection_List.Add(new DataObject() { Name = "item 1", Price = 20.41f });

            this.DataGrid_Prices.ItemsSource = _ObsCollection_List;


            //IP_Proxy = new Proxy();


            Thread UIThread = new Thread(UpdateUI_Thread);
            UIThread.Start();


        }

        //public void UpdateGrid

        public void InitializeDataGrid(string ItemName, string ItemPrice)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.DataBind, new Action(
                ()
                    =>_ObsCollection_List.Add(new DataObject()
                    {
                        Name = ItemName,
                        Price = ItemPrice,
                        BuyOutPrice = "-1"
                    })
                ));
        }

        public void UpdateDataGrid(int index, string ItemPrice, string BuyPrice)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.DataBind, new Action(
                () =>
                    {
                        _ObsCollection_List[index].Price = ItemPrice;
                        _ObsCollection_List[index].BuyOutPrice = BuyPrice;
                        //_ObsCollection_List[index] = (new DataObject() { Price = ItemPrice });
                    }
                ));
        }

        public void UpdateUI_Thread()
        {
            string[] parseString = null;
            //string command = null;
            while (true)
            {
                if (UIQueue.Count > 0)
                {
                    if (UIQueue.TryDequeue(out string result))
                    {
                        parseString = result.Split('@');
                        if (parseString[0] == "init") // check command
                        {
                            InitializeDataGrid(parseString[1], "N/A");
                        }
                        else if(parseString[0] == "update")
                        {
                            UpdateDataGrid(Int32.Parse(parseString[1]), parseString[2], parseString[3]);
                        }
                    }
                }
                        //Dispatcher.BeginInvoke(new Action(() => { Prices.Text += result + "\n"; }));
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
