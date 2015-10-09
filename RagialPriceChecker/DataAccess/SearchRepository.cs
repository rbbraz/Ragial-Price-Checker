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
    public class SearchRepository : BaseRepository
    {
        #region Fields
        List<Search> _searches;        
        #endregion

        #region Constructor
        public SearchRepository(string dataFile)
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
            using (XmlReader xmlRdr = new XmlTextReader(_dataFile)) //(stream))
            {
                _searches =
                    (from search in XDocument.Load(xmlRdr).Element("searches").Elements("search")
                     select Search.CreateSearch
                         ((string)search.Attribute("url"),
                          (bool)search.Attribute("autoStart"),
                          (int)search.Attribute("index"))).ToList();
            }
        }

        private void SaveToXML()
        {
            using (XmlWriter xmlWt = new XmlTextWriter(_dataFile, Encoding.UTF8))
            {
                xmlWt.WriteStartElement("searches");

                foreach (Search s in _searches)
                {
                    xmlWt.WriteStartElement("search");
                    xmlWt.WriteAttributeString("url", s.URL);
                    xmlWt.WriteAttributeString("autoStart", s.AutoStart.ToString());
                    xmlWt.WriteAttributeString("index", s.Index.ToString());
                    xmlWt.WriteEndElement();
                }

                xmlWt.WriteEndElement();
            }
        }
        #endregion

        #region Public
        public event EventHandler<ModelBaseAddedEventArgs> SearchAdded;
        public event EventHandler<ModelBaseAddedEventArgs> SearchRemoved;

        public void RemoveSearch(Search search)
        {
            if (search == null) throw new ArgumentException("Null search");

            Search _search = _searches.FirstOrDefault(s => s.URL == search.URL);

            if (_search != null)
            {
                _searches.Remove(_search);

                if (this.SearchRemoved != null)
                    this.SearchRemoved(this, new ModelBaseAddedEventArgs(_search));

                SaveToXML();
            }
        }

        public void UpdateSearch(Search search)
        {
            if (search == null) throw new ArgumentException("Null search");

            Search _search = _searches.FirstOrDefault(s => s.URL == search.URL);            

            if (_search == null)
            {
                AddSearch(search);
            }
            //else
            //{
            //    _search.AutoStart = search.AutoStart;
            //    _search.IsRunning = search.IsRunning;
            //    // Can never update the URL after creation
            //}

            SaveToXML();
        }

        // Adds a new Search and raises an event to allow the UI to be updated
        private void AddSearch(Search search)
        {
            search.Index = (_searches.Count == 0) ? 0 : _searches.Max(s => s.Index) + 1;
            _searches.Add(search);

            if (this.SearchAdded != null)
                this.SearchAdded(this, new ModelBaseAddedEventArgs(search));
        }

        public List<Search> GetSearches()
        {
            return new List<Search>(_searches); // should we return a copy? -- probably better to avoid issues
        }
        #endregion
    }
}
