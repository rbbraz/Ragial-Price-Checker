using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RagialPriceChecker.Models
{
    public class Item : ModelBase
    {
        #region Properties

        /// <summary>
        /// Item name.
        /// </summary>
        /// <remarks>Collection Key, needs to be unique</remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum price the user wishes to pay.
        /// </summary>
        public int TargetPrice
        {
            get;
            set;
        }        
        #endregion

        #region Creation
        protected Item()
        {
        }

        public static Item CreateItem(string name, int targetprice)
        {
            return new Item
                    {
                        Name = name,
                        TargetPrice = targetprice
                    };
        }
        #endregion

        // Are these needed? Might just be useless overhead
        ///// <summary>
        ///// Items' average price
        ///// </summary>
        //public int AveragePrice
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Ragial item link
        ///// </summary>
        //public string RagialLink
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// DB link
        ///// </summary>
        //public string DBLink
        //{
        //    get;
        //    set;
        //}
        //public void SetProperties(string ragial, string dblink, int avgprice)
        //{
        //    RagialLink = ragial;
        //    DBLink = dblink;
        //    AveragePrice = avgprice;
        //}
    }
}
