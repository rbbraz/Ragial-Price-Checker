using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RagialPriceChecker.Models;
using RagialPriceChecker.DataAccess;
using System.Windows.Input;
using RagialPriceChecker.Transporters;
using System.Windows.Threading;

namespace RagialPriceChecker.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        #region Fields
        readonly Search _search;
        readonly SearchRepository _searchRepository;
        RelayCommand _saveCommand;
        RelayCommand _deleteCommand;
        NotificationViewModel _notifVM;

        SearchEngine sList;
        #endregion

        #region Properties
        private int _index;
        public int Index
        {
            get { return _index; } // _search.URL; }
            set
            {
                if (value == _index) return; //_search.URL) return;

                _index = value; // _search.URL = value;
                base.OnPropertyChanged("Index");
            }
        }
        public int SearchIndex
        {
            get { return _search.Index; }
            set
            {
                if (value == _search.Index) return;
                _search.Index = value;
                base.OnPropertyChanged("SearchIndex");
            }
        }
        private string _url;
        public string URL
        {
            get { return _url; } // _search.URL; }
            set
            {
                if (value == _url) return; //_search.URL) return;

                _url = value; // _search.URL = value;
                base.OnPropertyChanged("URL");
            }
        }
        public string SearchURL
        {
            get { return _search.URL; }
            set
            {
                if (value == _search.URL) return;
                _search.URL = value;
                base.OnPropertyChanged("SearchURL");
            }
        }
        private bool _autoStart;
        public bool AutoStart
        {
            get { return _autoStart; } //_search.AutoStart; }
            set
            {
                if (value == _autoStart) return; //_search.AutoStart) return;

                //_search.AutoStart = value;
                _autoStart = value;
                base.OnPropertyChanged("AutoStart");
            }
        }
        public bool SearchAutoStart
        {
            get { return _search.AutoStart; }
            set
            {
                if (value == _search.AutoStart) return;

                _search.AutoStart = value;                
                base.OnPropertyChanged("SearchAutoStart");
            }
        }
        public bool IsRunning
        {
            get { return sList.IsRunning; }// _search.IsRunning; }
            set
            {
                if (value == _search.IsRunning) return;

                _search.IsRunning = value;
                base.OnPropertyChanged("IsRunning");
            }
        }
        public bool CanStart
        {
            get
            {
                // TODO: check if URL is valid, etc, should be here or @ URL?
                return (!IsRunning && (URL != string.Empty));
            }
        }
        public bool CanStop
        {
            get { return IsRunning && (URL != string.Empty); }
        }
        #endregion


        public SearchViewModel(Search search, SearchRepository repo, NotificationViewModel notif, ItemRepository itemRepo, LogsViewModel logs, Dispatcher dispatcher)
        {
            _search = search;
            URL = SearchURL;
            AutoStart = SearchAutoStart;
            Index = SearchIndex;

            _searchRepository = repo;

            _notifVM = notif;
            sList = new SearchEngine(itemRepo, _search, logs, dispatcher);

            _notifVM.RegisterSearchEvents(sList);
        }

        #region Presentation Layer
        public bool CanSave
        {
            get { return (URL != ""); }
        }
        public bool CanDelete
        {
            get { return (Index != -1); }
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => this.Save(),
                        param => this.CanSave
                        );
                }
                return _saveCommand;
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        param => this.Delete(),
                        param => this.CanDelete
                        );
                }
                return _deleteCommand;
            }
        }
        #endregion

        public void Save()
        {
            Search m_search = _search;

            if (URL != SearchURL)
            {
                m_search = Search.CreateSearch(URL, AutoStart);

                URL = SearchURL;
                AutoStart = SearchAutoStart;
            }
            else
            {                
                SearchAutoStart = AutoStart;
            }
            
            _searchRepository.UpdateSearch(m_search);
        }
        public void Delete()
        {
            _searchRepository.RemoveSearch(_search);
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            _notifVM.UnregisterSearchEvents(sList);
            sList.Dispose();
        }

        internal void Start()
        {
            sList.Start();
        }

        internal void Stop()
        {
            sList.Stop();
        }
    }
}
