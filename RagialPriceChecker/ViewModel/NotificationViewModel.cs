using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RagialPriceChecker.Transporters;
using System.Windows.Threading;
using RagialPriceChecker.View;
using System.Timers;

namespace RagialPriceChecker.ViewModel
{
    public class NotificationViewModel : ViewModel.ViewModelBase
    {
        public ObservableCollection<ItemPriceList> List { get; set; }
        private SortedDictionary<string, ItemPriceList> LocalList { get; set; }
        string OldText = "";
        public string Text
        {
            get
            {
                string txt = "";
                foreach (string key in LocalList.Keys)
                {
                    txt = txt + LocalList[key].Text + Environment.NewLine;
                }
                return txt.Trim();
            }
        }
        Dispatcher _dispatcher;
        Timer timer;

        public NotificationViewModel(Dispatcher dispatcher)
        {
            List = new ObservableCollection<ItemPriceList>();
            LocalList = new SortedDictionary<string, ItemPriceList>();

            _dispatcher = dispatcher;

            timer = new Timer(15000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            //if ((wnd == null) || (wnd.Visibility != System.Windows.Visibility.Visible) || (!wnd.IsActive))
            if (wnd == null)
            {
                wnd = new NotificationWnd();
                wnd.DataContext = this;
            }
        }

        public void RegisterSearchEvents(SearchEngine sList)
        {
            sList.SearchStarted += OnSearchStarted;
            sList.SearchFinished += OnSearchFinished;
            sList.SearchStopped += OnSearchStopped;
        }
        public void UnregisterSearchEvents(SearchEngine sList)
        {
            sList.SearchStarted -= OnSearchStarted;
            sList.SearchFinished -= OnSearchFinished;
            sList.SearchStopped -= OnSearchStopped;
        }

         // When the search is finished, add the relevant items
        void OnSearchStarted(object obj, EventArgs e)
        {
            DispatchSearchStarted(obj as SearchEngine);
        }
        void DispatchSearchStarted(SearchEngine sList)
        {
            Action del = () => SearchStarted(sList);
            _dispatcher.Invoke(del);
        }
        void SearchStarted(SearchEngine sList)
        {
            lock ("ItemLists")
            {
                foreach (ItemPriceList list in sList.LocalSearchItemL)
                {
                    if (LocalList.ContainsKey(list.ItemName))
                    {
                        List<ItemVend> ToRemove = new List<ItemVend>();
                        foreach (ItemVend item in list.ItemPriceL)
                        {
                            if (LocalList[list.ItemName].ItemPriceL.Contains(item))
                            {
                                ToRemove.Add(item);
                            }
                        }
                        foreach (ItemVend item in ToRemove)
                        {
                            LocalList[list.ItemName].ItemPriceL.Remove(item);
                        }
                        if (LocalList[list.ItemName].ItemPriceL.Count == 0)
                        {
                            LocalList.Remove(list.ItemName);
                        }
                    }
                }
            }
        }

         // When the search is finished, add the relevant items
        void OnSearchStopped(object obj, EventArgs e)
        {
            DispatchSearchStopped(obj as SearchEngine);
        }
        void DispatchSearchStopped(SearchEngine sList)
        {
            Action del = () => SearchStopped(sList);
            _dispatcher.Invoke(del);
        }
        void SearchStopped(SearchEngine sList)
        {
            lock ("ItemLists")
            {
                foreach (ItemPriceList list in sList.SearchItemL)
                {
                    if (LocalList.ContainsKey(list.ItemName))
                    {
                        List<ItemVend> ToRemove = new List<ItemVend>();
                        foreach (ItemVend item in list.ItemPriceL)
                        {
                            if (LocalList[list.ItemName].ItemPriceL.Contains(item))
                            {
                                ToRemove.Add(item);
                            }
                        }

                        foreach (ItemVend item in ToRemove)
                        {
                            LocalList[list.ItemName].ItemPriceL.Remove(item);
                        }
                        if (LocalList[list.ItemName].ItemPriceL.Count == 0)
                        {
                            LocalList.Remove(list.ItemName);
                        }
                    }
                }
            }

            sList.Dispose();
        }

        // When the search is finished, add the relevant items
        void OnSearchFinished(object obj, EventArgs e)
        {
            DispatchSearchFinished(obj as SearchEngine);
        }
        void DispatchSearchFinished(SearchEngine sList)
        {
            Action del = () => SearchFinished(sList);
            _dispatcher.Invoke(del);
        }
        void SearchFinished(SearchEngine sList)
        {
            lock ("ItemLists")
            {
                foreach (ItemPriceList list in sList.SearchItemL)
                {
                    if (LocalList.ContainsKey(list.ItemName))
                    {
                        List<ItemVend> ToAdd = new List<ItemVend>();
                        foreach (ItemVend item in list.ItemPriceL)
                        {
                            if (!LocalList[list.ItemName].ItemPriceL.Contains(item))
                            {
                                ToAdd.Add(item);
                            }
                        }
                        foreach (ItemVend item in ToAdd)
                        {
                            LocalList[list.ItemName].ItemPriceL.Add(item);
                        }
                    }
                    else
                    {
                        LocalList.Add(list.ItemName, list);
                    }
                }
            }
            if (Text != OldText)
            {
                OldText = Text;
                Commit();
                ShowWindow();
            }
        }

        void Commit()
        {
            lock ("ItemLists")
            {
                List.Clear();
                foreach (ItemPriceList s in LocalList.Values)
                {
                    List.Add(s);
                }
            }
        }       

        NotificationWnd wnd;
        void ShowWindow()
        {
            timer.Stop();
            
            wnd.Show();
            timer.Start();
        }
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            DispatchClose();
        }
        //void DispatchShow()
        //{
        //    Action del = () => ShowWindow();
        //    _dispatcher.Invoke(del);
        //}
        void DispatchClose()
        {
            Action del = () => CloseWindow();
            _dispatcher.Invoke(del);
        }
        void CloseWindow()
        {
            if (wnd != null && (wnd.IsActive || wnd.Visibility == System.Windows.Visibility.Visible))
            {
                wnd.Hide();
                //wnd = null;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (wnd != null)
                wnd.Close();
        }
    }
}
