using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using RagialPriceChecker.DataAccess;
using System.Collections.ObjectModel;
using RagialPriceChecker.Models;
using System.Windows.Input;

namespace RagialPriceChecker.ViewModel
{
    public class AllSearchesViewModel : ViewModelBase
    {
        Dispatcher _dispatcher;
        readonly SearchRepository _searchRepo;
        SearchViewModel _active;
        LogsViewModel _logs;
        NotificationViewModel _notifVM;
        ItemRepository _itemRepo;

        public SearchViewModel ActiveSearch
        {
            get
            {
                if (_active == null)
                {
                    _active = new SearchViewModel(Search.CreateSearch("", false), _searchRepo, _notifVM, _itemRepo, _logs, _dispatcher);
                    // Any event?
                }

                return _active;
            }
            set
            {
                if (_active == value) return;

                _active = value;
                OnPropertyChanged("ActiveSearch");
            }
        }
        public ObservableCollection<SearchViewModel> Searches
        {
            get;
            set;
        }

        public AllSearchesViewModel(Dispatcher dispatcher, string dataFile, LogsViewModel logsVM, ItemRepository itemRepo)
        {
            _dispatcher = dispatcher;
            _searchRepo = new SearchRepository(dataFile);
            _itemRepo = itemRepo;
            _logs = logsVM;

            _notifVM = new NotificationViewModel(_dispatcher);

            base.DisplayName = "All Searches View Model";

            _searchRepo.SearchAdded += new EventHandler<ModelBaseAddedEventArgs>(OnSearchAddedToRepository);
            _searchRepo.SearchRemoved += new EventHandler<ModelBaseAddedEventArgs>(OnSearchRemovedFromRepository);

            CreateAllSearches();

            foreach (SearchViewModel search in Searches)
                if(search.AutoStart) Start(search);
        }        

        #region Commands
        ICommand _startAll;
        public ICommand StartAll
        {
            get 
            {
                if (_startAll == null)
                    _startAll = new RelayCommand(param => this.StartAllSearches());
                return _startAll;
            }
        }
        ICommand _stopAll;
        public ICommand StopAll
        {
            get
            {
                if (_stopAll == null)
                    _stopAll = new RelayCommand(param => this.StopAllSearches());
                return _stopAll;
            }
        }
        public bool CanStart
        {
            get
            {
                return ActiveSearch.CanStart;
            }
        }
        ICommand _start;
        public ICommand StartActive
        {
            get
            {
                if (_start == null)
                    _start = new RelayCommand(param => this.StartActiveSearch(),
                                         param => this.CanStart);
                return _start;
            }
        }
        public bool CanStop
        {
            get
            {
                return ActiveSearch.CanStop;
            }
        }
        ICommand _stop;
        public ICommand StopActive
        {
            get
            {
                if (_stop == null)
                    _stop = 
                        new RelayCommand(param => this.StopActiveSearch(),
                                         param => this.CanStop);
                return _stop;
            }
        }
        #endregion

        #region Searches
        void CreateAllSearches()
        {
            List<SearchViewModel> all =
               (from search in _searchRepo.GetSearches()
                select new SearchViewModel(search, _searchRepo, _notifVM, _itemRepo, _logs, _dispatcher)).ToList();

            this.Searches = new ObservableCollection<SearchViewModel>(all);            
        }

        void StartAllSearches()
        {
            foreach (SearchViewModel search in Searches)
                Start(search);
        }
        void StartActiveSearch()
        {
            Start(ActiveSearch);
        }
        void StopAllSearches()
        {
            foreach (SearchViewModel search in Searches)
                Stop(search);
        }
        private void StopActiveSearch()
        {
            Stop(ActiveSearch);
        }
        void Start(SearchViewModel search)
        {
            if (!search.IsRunning) search.Start();
        }
        void Stop(SearchViewModel search)
        {
            if (search.IsRunning) search.Stop();
        }
        #endregion

        #region Event Handling
        void OnSearchAddedToRepository(object sender, ModelBaseAddedEventArgs e)
        {
            var vm = new SearchViewModel((Search)e.NewObject, _searchRepo, _notifVM, _itemRepo, _logs, _dispatcher);
            this.Searches.Add(vm);

            ActiveSearch = vm;
        }
        void OnSearchRemovedFromRepository(object sender, ModelBaseAddedEventArgs e)
        {
            SearchViewModel vm = this.Searches.FirstOrDefault(v => v.URL == ((Search)e.NewObject).URL);
            if (vm != null) this.Searches.Remove(vm);

            ActiveSearch = null;
        }
        #endregion

        #region Overrides
        protected override void OnDispose()
        {
            foreach (SearchViewModel search in this.Searches)
                search.Dispose();

            this.Searches.Clear();
            _searchRepo.SearchAdded -= OnSearchAddedToRepository;
            _searchRepo.SearchRemoved -= OnSearchRemovedFromRepository;
            _notifVM.Dispose();
        }
        #endregion
    }
}
