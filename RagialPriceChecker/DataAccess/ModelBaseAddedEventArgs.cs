using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RagialPriceChecker.Models;

namespace RagialPriceChecker.DataAccess
{
    public class ModelBaseAddedEventArgs : EventArgs
    {
        public ModelBaseAddedEventArgs(ModelBase newObject)
        {
            this.NewObject = newObject;
        }

        public ModelBase NewObject { get; private set; }
    }
}
