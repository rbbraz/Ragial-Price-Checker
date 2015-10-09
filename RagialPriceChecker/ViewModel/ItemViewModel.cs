using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RagialPriceChecker.Models;
using RagialPriceChecker.DataAccess;
using System.Windows.Input;

namespace RagialPriceChecker.ViewModel
{
    public class ItemViewModel : ViewModelBase
    {
        #region Fields
        readonly Item _item;
        readonly ItemRepository _itemRepository;
        RelayCommand _saveCommand;
        RelayCommand _deleteCommand;
        #endregion

        #region Properties
        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;

                _name = value;
                base.OnPropertyChanged("Name");
            }
        }
        public string ItemName
        {
            get { return _item.Name; }
            set
            {
                if (value == _item.Name) return;

                _item.Name = value;
                base.OnPropertyChanged("ItemName");
            }
        }
        int _targetPrice;
        public int TargetPrice
        {
            get { return _targetPrice; }
            set
            {
                if (value == _targetPrice) return;

                _targetPrice = value;
                base.OnPropertyChanged("TargetPrice");
            }
        }        
        public int ItemTargetPrice
        {
            get { return _item.TargetPrice; }
            set
            {
                if (value == _item.TargetPrice) return;

                _item.TargetPrice = value;
                base.OnPropertyChanged("ItemTargetPrice");
            }
        }
        #endregion

        public ItemViewModel(Item item, ItemRepository repo)
        {
            _item = item;
            _itemRepository = repo;
            Name = ItemName;
            TargetPrice = ItemTargetPrice;
        }

        #region Presentation Layer
        public bool CanSave
        {
            get { return (Name != ""); }
        }
        public bool CanDelete
        {
            get { return (Name != ""); }
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

        void Save()
        {
            Item m_item = _item;

            if (Name != ItemName)
            {
                m_item = Item.CreateItem(Name, TargetPrice);

                Name = ItemName;
                TargetPrice = ItemTargetPrice;
            }
            else
            {
                ItemTargetPrice = TargetPrice;
            }

            _itemRepository.UpdateItem(m_item);
        }
        void Delete()
        {
            _itemRepository.RemoveItem(_item);
        }
    }
}
