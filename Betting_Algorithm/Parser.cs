﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Betting_Algorithm
{
    public class Parser
    {
        /* WebClient[] _WebClient = new WebClient[3];
         string[] URL_List= new string[3];
         Thread[] th = new Thread[3];
         float[] lowestPrice = new float[3];
         string[] dString = new string[3];
         JObject[] json = new JObject[3];*/

        private const int CSGO_ID = 730;
        private const int DOTA2_ID = 570;

        public int numberOfElements = -1;
        private Proxy IP_Proxy;


        public struct DataStructure
        {
            
            public float[] lowestPrice;
            public float[] mediumPrice;
            public WebClient[] WebClient;
            public Thread[] webThread;
            public JObject[] json;
            public string[] downloadString;
            public string[] name;
            public string[] Item_Url;
            public List<string> URL_List;
            
        };

        DataStructure WebData_Structure;

        //ConcurrentQueue<DataStructure> Web_Queue;

        public Parser()
        {
            InitializeStructure();
            IP_Proxy = new Proxy();
            ComputeNamesAndURL();
        //InitThreads();
        }

        private void AddUrlsToStructure()
        {
            WebData_Structure.URL_List = new List<string>();
            StreamReader fileReader = new StreamReader("../../Res/items_list.txt");
            string lineRead;
            while ((lineRead = fileReader.ReadLine()) != null)
            {
                WebData_Structure.URL_List.Add(lineRead);
            }
            numberOfElements = WebData_Structure.URL_List.Count;
            MainWindow.main.UpdateNoOfItems(numberOfElements);
            /*
                WebData_Structure.URL_List[0] = "http://steamcommunity.com/market/priceoverview/?appid=570&currency=3&market_hash_name=Exalted%20Great%20Sage%27s%20Reckoning"; 
                WebData_Structure.URL_List[1] = "http://steamcommunity.com/market/priceoverview/?appid=570&currency=3&market_hash_name=Battlefury";
                WebData_Structure.URL_List[2] = "http://steamcommunity.com/market/priceoverview/?appid=570&currency=3&market_hash_name=Dragonclaw%20Hook";
                WebData_Structure.URL_List[3] = "http://steamcommunity.com/market/priceoverview/?appid=570&currency=3&market_hash_name=Inscribed%20Dragonclaw%20Hook";
                WebData_Structure.URL_List[4] = "http://steamcommunity.com/market/priceoverview/?appid=570&currency=3&market_hash_name=Exalted%20Frost%20Avalanche";
                WebData_Structure.URL_List[5] = "http://steamcommunity.com/market/priceoverview/?appid=570&currency=3&market_hash_name=Vermillion%20Crucible";
                WebData_Structure.URL_List[6] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=AK-47%20%7C%20Bloodsport%20%28Factory%20New%29";
                WebData_Structure.URL_List[7] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=AK-47%20%7C%20Neon%20Revolution%20%28Battle-Scarred%29#";
                WebData_Structure.URL_List[8] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=StatTrak%E2%84%A2%20AWP%20%7C%20Corticera%20%28Minimal%20Wear%29#";
                WebData_Structure.URL_List[9] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=AK-47%20%7C%20Aquamarine%20Revenge%20%28Minimal%20Wear%29#";
                WebData_Structure.URL_List[10] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=AWP%20%7C%20BOOM%20%28Minimal%20Wear%29";
                WebData_Structure.URL_List[11] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=M4A4%20%7C%20Buzz%20Kill%20%28Minimal%20Wear%29#";
                WebData_Structure.URL_List[12] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=StatTrak%E2%84%A2%20M4A4%20%7C%20%E9%BE%8D%E7%8E%8B%20%28Dragon%20King%29%20%28Minimal%20Wear%29";
                WebData_Structure.URL_List[13] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=M4A1-S%20%7C%20Mecha%20Industries%20%28Minimal%20Wear%29";
                WebData_Structure.URL_List[14] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=M4A4%20%7C%20Radiation%20Hazard%20%28Minimal%20Wear%29";
                WebData_Structure.URL_List[15] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=AWP%20%7C%20Hyper%20Beast%20%28Well-Worn%29";
                WebData_Structure.URL_List[16] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=AWP%20%7C%20BOOM%20%28Field-Tested%29";
                WebData_Structure.URL_List[17] = "http://steamcommunity.com/market/priceoverview/?appid=730&currency=3&market_hash_name=AWP%20%7C%20Hyper%20Beast%20%28Well-Worn%29";
                WebData_Structure.URL_List[18] = "http://steamcommunity.com/market/priceoverview/?appid=570&currency=3&market_hash_name=Faceless%20Rex";
            */
        }

        private void InitializeStructure()
        {
            numberOfElements = GetNumberOfLinesInFile("../../Res/items_list.txt");
            WebData_Structure = new DataStructure
            {
                WebClient = new WebClient[numberOfElements],
                //URL_List = new List<string>(),
                webThread = new Thread[numberOfElements],
                lowestPrice = new float[numberOfElements],
                downloadString = new string[numberOfElements],
                json = new JObject[numberOfElements],
                name = new string[numberOfElements],
                Item_Url = new string[numberOfElements],
                mediumPrice = new float[numberOfElements]
            };
            AddUrlsToStructure();
        }

        public void InitThreads()
        {

            for(int index = 0; index < numberOfElements; index++)
            {

                //ComputeNamesAndURL();
                WebData_Structure.mediumPrice[index] = -1;                
                CreateThread(index);
            }
            /*for(int index = 0; index < numberOfElements; index += 4)
            {
                WebData_Structure.webThread[index].Start();
                Console.WriteLine("Thread " + index + " started");
            }*/
        }

        private void ComputeNamesAndURL()
        {
            string nameFromUrl = null;
            string lastPartOfURL = null;
            for (int index = 0; index < numberOfElements; index++)
            {
                lastPartOfURL = (WebData_Structure.URL_List[index].Split('=')[3]);
                nameFromUrl = lastPartOfURL.Replace("%20", " ");
                nameFromUrl = nameFromUrl.Replace("%E2%84", "™");
                nameFromUrl = nameFromUrl.Replace("%7C", "|");
                nameFromUrl = nameFromUrl.Replace("%28", "(");
                nameFromUrl = nameFromUrl.Replace("%29", ")");
                nameFromUrl = nameFromUrl.Replace("%A2", " ");
                nameFromUrl = nameFromUrl.Replace("%27", "'");
                nameFromUrl = nameFromUrl.Replace("%E2%98%85", "★");
                nameFromUrl = nameFromUrl.Replace("E9%BE%8D%E7%8E%8B", "龍王");
                WebData_Structure.name[index] = nameFromUrl;

                WebData_Structure.Item_Url[index] = "http://steamcommunity.com/market/listings/" + GetElementToGameID(index) + "/" + lastPartOfURL;

                PushDataToQueue("init@" +  WebData_Structure.name[index]);
            }
            //PushDataToQueue("update@3@21.1");
        }

        private void CreateThread(int index)
        {
            WebData_Structure.WebClient[index] = new WebClient();
            WebData_Structure.webThread[index] = new Thread(
                () => Start_Web_Th(index))
            {
                Name = "Thread_" + index
            };
            WebData_Structure.webThread[index].Start();
            Console.WriteLine("Thread " + index + " started");
        }

        private int GetElementToGameID(int id)
        {
            string initString = WebData_Structure.URL_List[id].Split('=')[1];
            int numberToReturn = Convert.ToInt32(initString.Split('&')[0]);
            return numberToReturn;
        }

        void Start_Web_Th(int element_ID)
        {
            Console.WriteLine("Thread " + element_ID + " entered");

            WebProxy wp;
            string url = WebData_Structure.URL_List[element_ID];
            while (true)
            {
                if (IP_Proxy.proxyList.Count > 5)
                {
                    if (element_ID % 4 == 0)
                    {
                        Console.WriteLine("Changing proxy...");
                        wp = IP_Proxy.GetNextProxy();
                        Console.WriteLine("Proxy changed to " + wp.Address);
                    }
                    else
                    {
                        wp = IP_Proxy.GetCurrentProxy();
                    }
                    if (wp != null)
                    {
                        // WebData_Structure.WebClient[element_ID] = new WebClient();
                        WebData_Structure.WebClient[element_ID].Proxy = wp;
                        try
                        {

                            while (WebData_Structure.WebClient[element_ID].IsBusy)
                            {
                                Thread.Sleep(20);
                            }
                            if (WebData_Structure.WebClient[element_ID].Proxy == null)
                            {
                                //continue;
                                goto endOfCodeLabel;
                            }
                            Task downloadTask = Task.Run(() =>
                            {
                                try
                                {
                                    WebData_Structure.downloadString[element_ID] = WebData_Structure.WebClient[element_ID].DownloadString(url);
                                }
                                catch
                                {
                                //IP_Proxy.RemoveProxy(wp);
                            }
                            });
                            if (downloadTask.Wait(TimeSpan.FromSeconds(3)))
                            {

                                WebData_Structure.json[element_ID] = JObject.Parse(WebData_Structure.downloadString[element_ID]);
                                WebData_Structure.lowestPrice[element_ID] = (float)(decimal.Parse(WebData_Structure.json[element_ID]["lowest_price"].ToString().Replace("€", ""), System.Globalization.NumberStyles.Currency)) / 100;

                                if (WebData_Structure.mediumPrice[element_ID] == -1)
                                {
                                    try
                                    {
                                        WebData_Structure.mediumPrice[element_ID] = (float)(decimal.Parse(WebData_Structure.json[element_ID]["median_price"].ToString().Replace("€", ""), System.Globalization.NumberStyles.Currency)) / 100;
                                    }
                                    catch
                                    {
                                        WebData_Structure.mediumPrice[element_ID] = 5;
                                    }
                                }
                                else if ((WebData_Structure.lowestPrice[element_ID] < (WebData_Structure.mediumPrice[element_ID]) / 2 && WebData_Structure.lowestPrice[element_ID] < 20) || (WebData_Structure.mediumPrice[element_ID] == -1 && WebData_Structure.lowestPrice[element_ID] <= 5))
                                {
                                    Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", WebData_Structure.Item_Url[element_ID]);
                                }
                                //Console.WriteLine("Lowest price for " + WebData_Structure.URL_List[element_ID] + " = " + WebData_Structure.lowestPrice[element_ID]);
                                // PushDataToQueue(WebData_Structure.name[element_ID] + " @ " + WebData_Structure.lowestPrice[element_ID]);
                                PushDataToQueue("update@" + element_ID + "@" + WebData_Structure.lowestPrice[element_ID] + "@" + WebData_Structure.mediumPrice[element_ID]);
                                //PushDataToQueue("update@3@21.1");
                            }
                            else
                            {
                                // timed out
                                IP_Proxy.RemoveProxy(wp);
                            }
                            endOfCodeLabel:
                            {

                            }

                        }
                        /*catch(WebException e)
                        {
                            Console.WriteLine("WebException occured in [Start_Web_Th]:: " + e.Message);
                        }*/
                        catch (WebException wex)
                        {
                            HttpWebResponse response = wex.Response as HttpWebResponse;
                            HttpStatusCode errorCode = response.StatusCode;
                            if (errorCode == HttpStatusCode.BadGateway || errorCode == HttpStatusCode.ProxyAuthenticationRequired || errorCode == HttpStatusCode.BadRequest)
                            {
                                IP_Proxy.RemoveProxy(wp);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception occured in [Start_Web_Th][" + element_ID + "]:: " + e);
                        }
                        Thread.Sleep(300);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public void PushDataToQueue(string data)
        {
            MainWindow.main.UIQueue.Enqueue(data);
        }

        private int GetNumberOfLinesInFile(string path)
        {
            return File.ReadAllLines(path).Count();
        }

    }
}
