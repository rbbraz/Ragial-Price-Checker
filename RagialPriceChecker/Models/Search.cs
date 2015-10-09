using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RagialPriceChecker.Models
{
    public class Search : ModelBase
    {
        #region Properties
        public int Index { get; set; }
        /// <summary>
        /// Search query string
        /// </summary>
        /// <remarks>Collection Key, needs to be unique</remarks>
        public String URL
        {
            get;
            set;
        }
        /// <summary>
        /// Status flag that indicates the search thread is running
        /// Should not be stored in disk
        /// </summary>
        public bool IsRunning
        {
            get;
            set;
        }
        /// <summary>
        /// Flag that indicates if the search should start as the software starts
        /// </summary>
        public bool AutoStart
        {
            get;
            set;
        }
        #endregion

        #region Creation
        public static Search CreateSearch(string url, bool autostart)
        {
            return new Search
            {
                URL = url,
                AutoStart = autostart,
                Index = -1,
                IsRunning = false // never create automatically running
            };
        }
        public static Search CreateSearch(string url, bool autostart, int index)
        {
            return new Search
            {
                URL = url,
                AutoStart = autostart,
                Index = index,
                IsRunning = false // never create automatically running
            };
        }

        protected Search()
        {
        }
        #endregion
    }
}
