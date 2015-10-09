using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RagialPriceChecker.ViewModel;
using RagialPriceChecker.DataAccess;
using System.Windows.Threading;
using RagialPriceChecker.Models;

namespace RagialPriceChecker.SearchHelpers
{
    // Where to store this?
    // Dictionary on ASVM?
    // Or store this in the SVM?
    public class SearchWorker
    {
        private static System.Timers.Timer sTimer;
        private Search _search;
        private LogsViewModel _logs;
        private ItemRepository _itemRepo;
        private Dispatcher _dispatcher;

        // We will also need some sort of VM for the results
        public SearchWorker(Search search, 
                            ItemRepository itemRepo,
                            Dispatcher dispatcher,
                            LogsViewModel logs)
        {
            _search = search;
            _itemRepo = itemRepo;
            _dispatcher = dispatcher;
            _logs = logs;

            sTimer = new System.Timers.Timer(60000); // 60s?
            sTimer.Elapsed += new System.Timers.ElapsedEventHandler(sTimer_Elapsed);

            sTimer.Start();
        }
        public void Start()
        {
            sTimer.Start();
        }
        public void Stop()
        {
            sTimer.Stop();
        }

        void sTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Searches, updates whatever needs to be updated, etc
            throw new NotImplementedException();
        }


        // The dispatcher needs to be used to update:
        // -- screen
        // -- observable collections
        // * aka AllItemsView, if needed, and also the ItemVend/VendItem/etc/ObsColls
        void DispatcherExample()
        {
            Action del = () => DispatchedMethod("txt");

            _dispatcher.Invoke(del);
        }
        void DispatchedMethod(object var)
        {
        }
    }
}
