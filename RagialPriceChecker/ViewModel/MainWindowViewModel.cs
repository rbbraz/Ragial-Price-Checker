using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RagialPriceChecker.DataAccess;
using System.Collections.ObjectModel;

namespace RagialPriceChecker.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        AllSearchesViewModel _allSearchesViewModel;
        AllItemsViewModel _allItemsViewModel;
        LogsViewModel _logsVM;
        #endregion

        public AllSearchesViewModel AllSearchesVM
        { get { return _allSearchesViewModel; } }

        public AllItemsViewModel AllItemsVM
        { get { return _allItemsViewModel; } }

        public LogsViewModel LogsVM
        { get { return _logsVM; } }

        #region Constructor
        public MainWindowViewModel(string itemDataFile, string searchDataFile)
        {
            base.DisplayName = "Title";

            _logsVM = new LogsViewModel(System.Windows.Threading.Dispatcher.CurrentDispatcher);
            _allItemsViewModel = new AllItemsViewModel(itemDataFile);
            _allSearchesViewModel = new AllSearchesViewModel(System.Windows.Threading.Dispatcher.CurrentDispatcher, 
                                                                searchDataFile, _logsVM, _allItemsViewModel.ItemRepository);            
        }
        #endregion

        protected override void OnDispose()
        {
            base.OnDispose();

            _logsVM.Dispose();
            _allItemsViewModel.Dispose();
            _allSearchesViewModel.Dispose();
        }
    }
}
