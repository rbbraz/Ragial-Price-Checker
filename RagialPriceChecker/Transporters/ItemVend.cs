using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RagialPriceChecker.Models;
using System.Timers;
using RagialPriceChecker.DataAccess;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Input;
using RagialPriceChecker.ViewModel;
using System.Windows.Threading;

namespace RagialPriceChecker.Transporters
{
    // Might need to add some locks

    /// <summary>
    /// Class that has the amount of items target vend has, current price, vend name
    /// Ragial URL and coordinates
    /// </summary>
    public class ItemVend : ViewModel.ViewModelBase
    {
        string _url;
        public string URL
        {
            get { return _url; }
            set
            {
                if (_url == value) return;
                _url = value;
                OnPropertyChanged("URL");
            }
        }
        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public int Amount { get; set; }
        public int Price { get; set; }
        // Coordinates
        public int X { get; set; }
        public int Y { get; set; }
        public string City { get; set; }
        public string Text
        {
            get
            {
                return String.Format("{0} - [{1}] x{2} @ {3} ({4},{5})", Name, Price, Amount, City, X, Y);
            }
        }

        // URL & Name are the exposed properties
        ICommand _goToUrlCommand;
        public ICommand GoToURLCommand
        {
            get
            {
                if (_goToUrlCommand == null)
                {
                    _goToUrlCommand = new RelayCommand(param => GoToURL());
                }

                return _goToUrlCommand;
            }
        }
        void GoToURL()
        {
            System.Diagnostics.Process.Start(URL);
        }
    }

    public class ItemPriceList : ViewModel.ViewModelBase
    {
        public ObservableCollection<ItemVend> ItemPriceL { get; set; }
        public List<ItemVend> LocalItemPriceL { get; set; }
        string _name;
        public string ItemName
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged("ItemName");
            }
        }
        string _url;
        public string URL
        {
            get { return _url; }
            set
            {
                if (_url == value) return;
                _url = value;
                OnPropertyChanged("URL");
            }
        }
        public string Text
        {
            get
            {
                if (LocalItemPriceL.Count == 0) return "";
                string txt = ItemName + Environment.NewLine;
                foreach (ItemVend list in LocalItemPriceL)
                {
                    txt = txt + list.Text + Environment.NewLine;
                }
                return txt.Trim();
            }
        }

        public ItemPriceList()
        {
            ItemPriceL = new ObservableCollection<ItemVend>();
            LocalItemPriceL = new List<ItemVend>();
        }

        public void Clear()
        {
            LocalItemPriceL.Clear();
        }

        public void Add(string storeName, string url,
                        int amt, int price, string city, int x, int y)
        {
            ItemVend v = new ItemVend()
            {
                Name = storeName,
                URL = url,
                Amount = amt,
                City = city,
                Price = price,
                X = x,
                Y = y
            };

            LocalItemPriceL.Add(v);
        }

        public void Commit()
        {
            ItemPriceL.Clear();
            foreach (ItemVend v in LocalItemPriceL) ItemPriceL.Add(v);
        }

        // URL & Name are the exposed properties
        ICommand _goToUrlCommand;
        public ICommand GoToURLCommand
        {
            get
            {
                if (_goToUrlCommand == null)
                {
                    _goToUrlCommand = new RelayCommand(param => GoToURL());
                }

                return _goToUrlCommand;
            }
        }
        void GoToURL()
        {
            System.Diagnostics.Process.Start(URL);
        }
    }

    public class SearchEngine : ViewModel.ViewModelBase
    {
        public ObservableCollection<ItemPriceList> SearchItemL { get; set; }
        public List<ItemPriceList> LocalSearchItemL { get; set; }
        Search _search;
        ItemRepository _itemRepo;
        LogsViewModel _logs;
        Timer timer;
        Dispatcher _dispatcher;
        string OldText;
        public string Text
        {
            get
            {
                string txt = "";
                foreach (ItemPriceList list in LocalSearchItemL)
                {
                    txt = txt + list.Text + Environment.NewLine;
                }
                return txt.Trim();
            }
        }
        public bool IsRunning { get { return timer.Enabled; } }

        public SearchEngine(ItemRepository repo, Search search, LogsViewModel logs, Dispatcher dispatcher)
        {
            SearchItemL = new ObservableCollection<ItemPriceList>();
            LocalSearchItemL = new List<ItemPriceList>();

            _logs = logs;

            timer = new Timer(60000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            OldText = "";

            _dispatcher = dispatcher;
            _search = search;
            _itemRepo = repo;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            SearchStarted(this, null);
            Clear();

            DoSearch();

            if (OldText != Text)
            {
                DispatchCommit();
                SearchFinished(this, null);
            }

            timer.Start();
        }

        public void Start()
        {
            timer_Elapsed(this, null);
        }
        public void Stop()
        {
            OldText = "";
            timer.Stop();
            SearchStopped(this, null);
        }

        public event EventHandler SearchStarted = delegate { };
        public event EventHandler SearchFinished = delegate { };

        public event EventHandler SearchStopped = delegate { };

        void Clear()
        {
            LocalSearchItemL.Clear();
        }

        void DoSearch()
        {
           try
           {
                string strRegex = @"<tr( class=""odd"")?>[ \n]*<td class=""name"">[ \n]*<a href=""http://ragial.com/item/iRO-Renewal/(?<ragiallink>[a-z0-9A-Z]*)"" class=""activate_tr"">[ \n]*<img src=""http://img1.ragial.com/res/img/item/(?<itemid>[0-9]*).png""/>[ \n]*(?<itemname>[a-zA-Z0-9+'\[\] ]*)[ \n]*</a>[ \n]*</td>[ \n]*<td class=""cnt"">[ \n]*(?<itemamount>[0-9]*)[ \n]*</td>[ \n]*[ \n]*<td class=""price [a-zA-Z]*"">[ \n]*[ \n]*<a href=""http://ragial.com/item/iRO-Renewal/[a-z0-9A-Z:./]*"">[ \n]*(?<itemprice>[0-9,]*)z[ \n]*</a>[ \n]*</td>[ \n]*<td class=""tag"">[ \n]*<a href=""http://ragial.com/tag/iRO-Renewal/[a-z0-9A-Z]*"">[ \n]*<img src=""http://img1.ragial.com/res/v10/tag.png""/>[ \n]*</a>[ \n]*</td>[ \n]*</tr>";
                RegexOptions myRegexOptions = RegexOptions.ExplicitCapture;
                Regex myRegex = new Regex(strRegex, myRegexOptions);

                string strTargetString = "";
                using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                {
                    strTargetString = client.DownloadString(_search.URL);
                }

                foreach (Match myMatch in myRegex.Matches(strTargetString))
                {
                    if (myMatch.Success)
                    {
                        string Name = myMatch.Groups["itemname"].Value;
                        int Amount = Convert.ToInt32(myMatch.Groups["itemamount"].Value);
                        //it.ItemID = Convert.ToInt32(myMatch.Groups["itemid"].Value);
                        string RagialLink = @"http://ragial.com/item/iRO-Renewal/" + myMatch.Groups["ragiallink"].Value;
                        int Price = Convert.ToInt32(myMatch.Groups["itemprice"].Value.Replace(",", ""));

                        Item it = _itemRepo.GetItems().FirstOrDefault(x => Name.Contains(x.Name));//x.Name == Name);

                        if (it != null && Price <= it.TargetPrice)
                        {
                            CheckItem(Name, RagialLink, it.TargetPrice);
                        }

                        _logs.Add(string.Format("{0} - x{1} - [{2}]", Name, Amount, Price));
                    }
                }
            } catch { }
        }
        void CheckItem(string name, string itemlink, int targetprice)
        {
            try
            {
                string strRegex = @"<tr( class=""odd"")?><td id=""date"" class=""nm""><a href=""http://ragial.com/shop/iRO-Renewal/(?<ShopLink>[a-zA-Z0-9]*)"">Vending Now</a></td><td id=""amt"">(?<Amount>[0-9]+)x</td><td id=""pc"" class=""nm""><a href=""http://ragial.com/shop/iRO-Renewal/[a-zA-Z0-9]*"" rel=""notip"">(?<Price>[0-9,]+)z</a></td>";

                RegexOptions myRegexOptions = RegexOptions.ExplicitCapture;
                Regex myRegex = new Regex(strRegex, myRegexOptions);

                string strTargetString = "";

                using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                {
                    strTargetString = client.DownloadString(itemlink);
                }

                foreach (Match myMatch in myRegex.Matches(strTargetString))
                {
                    if (myMatch.Success)
                    {
                        int amount = Convert.ToInt32(myMatch.Groups["Amount"].Value);
                        string shoplink = @"http://ragial.com/shop/iRO-Renewal/" + myMatch.Groups["ShopLink"].Value;
                        int readprice = Convert.ToInt32(myMatch.Groups["Price"].Value.Replace(",", ""));

                        if (readprice <= targetprice)
                        {
                            CheckShop(name, itemlink, readprice, amount, shoplink);
                        }
                    }
                }
            } catch { }
        }
        void CheckShop(string name, string itemlink, int price, int amount, string shoplink)
        {
            try
            {
                string shopname = "";
                string city = "";
                int x = 0;
                int y = 0;

                string strRegex = @"<div id=""map_zone"" class=""map"" style=""height: 120px; background-image: url\(http://(img2.)?ragial.com/res/maps/(?<City>[a-zA-Z0-9_]*).png\); background-position: 0px -[0-9]*px;""><div>(?<StoreName>[\s\S]*)</div><div class=""c_vend"" style="" top: [0-9]*px; left: [0-9]*px;""></div><div class=""c_txt"" style=""top: [0-9]*px; left: [0-9]*px;"">(2F )?(?<X>[0-9]+), (?<Y>[0-9]+)</div></div>";

                RegexOptions myRegexOptions = RegexOptions.ExplicitCapture;
                Regex myRegex = new Regex(strRegex, myRegexOptions);

                string strTargetString = "";

                using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                {
                    strTargetString = client.DownloadString(shoplink);
                }

                foreach (Match myMatch in myRegex.Matches(strTargetString))
                {
                    if (myMatch.Success)
                    {
                        shopname = myMatch.Groups["StoreName"].Value;
                        x = Convert.ToInt32(myMatch.Groups["X"].Value);
                        y = Convert.ToInt32(myMatch.Groups["Y"].Value);

                        if (myMatch.Groups["City"].Value == "payon_lazy") city = "Payon";
                        else if (myMatch.Groups["City"].Value == "") city = "Eden Group 2F";
                        else if (myMatch.Groups["City"].Value == "prontera_vend_clean") city = "Prontera";
                        else city = "Prontera or Unknown";

                        Add(name, shopname, itemlink, amount, price, city, x, y, shoplink);

                        return;
                    }
                }
            } catch { }
        }

        void Add(string itemName, string storeName, string itemlink,
                        int amt, int price, string city, int x, int y, string shoplink)
        {
            lock ("ItemLists")
            {
                ItemPriceList list = LocalSearchItemL.FirstOrDefault(par => par.ItemName == itemName);
                if (list == null)
                {
                    list = new ItemPriceList() { ItemName = itemName, URL = itemlink };
                    LocalSearchItemL.Add(list);
                }

                list.Add(storeName, shoplink, amt, price, city, x, y);
            }
        }

        void DispatchCommit()
        {
            Action del = () => Commit();
            _dispatcher.Invoke(del);
        }
        void Commit()
        {
            lock ("ItemLists")
            {
                SearchItemL.Clear();
                foreach (ItemPriceList v in LocalSearchItemL)
                {
                    v.Commit();
                    SearchItemL.Add(v);
                }
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Clear();
        }
    }
}
