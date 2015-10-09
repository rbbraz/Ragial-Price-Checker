using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RagialPriceChecker.DataAccess;
using System.Collections.ObjectModel;
using RagialPriceChecker.Models;

namespace RagialPriceChecker.ViewModel
{
    public class AllItemsViewModel : ViewModelBase
    {
        readonly ItemRepository _itemRepo;
        ItemViewModel _active;

        public ItemRepository ItemRepository { get { return _itemRepo; } }
        public ItemViewModel ActiveItem
        {
            get
            {
                if (_active == null)
                    _active = new ItemViewModel(Item.CreateItem("", 0), _itemRepo);
                return _active;
            }
            set
            {
                if (_active == value) return;
                _active = value;
                OnPropertyChanged("ActiveItem");
            }
        }
        public ObservableCollection<ItemViewModel> Items
        {
            get;
            set;
        }

        public AllItemsViewModel(string dataFile)
        {
            _itemRepo = new ItemRepository(dataFile);

            base.DisplayName = "All Items View Model";

            _itemRepo.ItemAdded += new EventHandler<ModelBaseAddedEventArgs>(OnItemAddedToRepo);
            _itemRepo.ItemRemoved += OnItemRemovedFromRepository;

            CreateAllItems();
        }

        private void CreateAllItems()
        {
            List<ItemViewModel> all =
               (from item in _itemRepo.GetItems()
                select new ItemViewModel(item, _itemRepo)).ToList();

            //TODO: Do I need to watch for PropertyChanged on the searches? Unlikely
            this.Items = new ObservableCollection<ItemViewModel>(all);
            //this.Searches.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        #region Items
        /// <summary>
        /// This should be called as a command from the UI?
        /// </summary>
        /// <param name="name"></param>
        public void SetActiveItem(string name)
        {
            // Find item by name key

            // Set it as Active

            // The properties should be automagically loaded via binding on the ActiveSearch
        }
        #endregion

        #region Event Handling
        void OnItemAddedToRepo(object sender, ModelBaseAddedEventArgs e)
        {
            var vm = new ItemViewModel((Item)e.NewObject, _itemRepo);
            this.Items.Add(vm);

            ActiveItem = vm;
        }
        void OnItemRemovedFromRepository(object sender, ModelBaseAddedEventArgs e)
        {
            ItemViewModel vm = this.Items.FirstOrDefault(v => v.Name == ((Item)e.NewObject).Name);
            if (vm != null) this.Items.Remove(vm);

            ActiveItem = null;
        }
        #endregion

        #region Overrides
        protected override void OnDispose()
        {
            foreach (ItemViewModel item in this.Items)
                item.Dispose();

            this.Items.Clear();
            //this.Searches.CollectionChanged -= OnCollectionChanged;
            _itemRepo.ItemAdded -= OnItemAddedToRepo;
            _itemRepo.ItemRemoved -= OnItemRemovedFromRepository;
        }
        #endregion
    }
}
