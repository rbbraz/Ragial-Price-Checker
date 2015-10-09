using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RagialPriceChecker.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace RagialPriceChecker.DataAccess
{
    public class ItemRepository : BaseRepository
    {
        #region Fields
        List<Item> _items;        
        #endregion

        #region Constructor
        public ItemRepository(string dataFile)
            : base(dataFile)
        {
            LoadFromXML();
        }
        #endregion

        #region XML methods
        private void LoadFromXML()
        {
            // _searches = new List<Search>();
            //_searches.DefaultIfEmpty(null);

            //using(Stream stream = GetResourceStream(_dataFile))
            using(XmlReader xmlRdr = new XmlTextReader(_dataFile)) //(stream))
            {
                _items =
                    (from search in XDocument.Load(xmlRdr).Element("items").Elements("item")
                     select Item.CreateItem
                         ((string)search.Attribute("name"),
                          (int)search.Attribute("targetPrice"))).ToList();
            }
        }

        private void SaveToXML()
        {
            using (XmlWriter xmlWt = new XmlTextWriter(_dataFile, Encoding.UTF8))
            {
                xmlWt.WriteStartElement("items");

                foreach (Item it in _items)
                {
                    xmlWt.WriteStartElement("item");
                    xmlWt.WriteAttributeString("name", it.Name);
                    xmlWt.WriteAttributeString("targetPrice", it.TargetPrice.ToString());
                    xmlWt.WriteEndElement();
                }

                xmlWt.WriteEndElement();
            }
        }
        #endregion

        #region Public
        public event EventHandler<ModelBaseAddedEventArgs> ItemAdded;
        public event EventHandler<ModelBaseAddedEventArgs> ItemRemoved;

        public void UpdateItem(Item item)
        {
            if (item == null) throw new ArgumentException("Null item");

            Item _item = _items.FirstOrDefault(it => it.Name == item.Name);

            if (_item == null)
            {
                AddItem(item);
            }
            //else
            //{
            //    _item.TargetPrice = item.TargetPrice;
            //    // Can never update the Name after creation
            //}

            SaveToXML();
        }

        public void RemoveItem(Item item)
        {
            if (item == null) throw new ArgumentException("Null search");

            Item _item = _items.FirstOrDefault(it => it.Name == item.Name);

            if (_item != null)
            {
                _items.Remove(_item);

                if (this.ItemRemoved != null)
                    this.ItemRemoved(this, new ModelBaseAddedEventArgs(_item));

                SaveToXML();
            }
        }

        // Adds a new Item and raises an event to allow the UI to be updated
        private void AddItem(Item item)
        {
            _items.Add(item);

            if (this.ItemAdded != null)
                this.ItemAdded(this, new ModelBaseAddedEventArgs(item));
        }

        public List<Item> GetItems()
        {
            return new List<Item>(_items); // should we return a copy? -- probably better to avoid issues
        }
        #endregion
    }
}
