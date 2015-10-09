using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Resources;
using System.Windows;
using RagialPriceChecker.Models;

namespace RagialPriceChecker.DataAccess
{
    public abstract class BaseRepository
    {
        protected string _dataFile;

        protected BaseRepository() { }
        protected BaseRepository(string dataFile)
        {
            _dataFile = dataFile;
        }

        protected static Stream GetResourceStream(string resourceFile)
        {
            Uri uri = new Uri(resourceFile, UriKind.RelativeOrAbsolute);

            StreamResourceInfo info = Application.GetResourceStream(uri);
            if (info == null || info.Stream == null)
                throw new ApplicationException("Missing resource file: " + resourceFile);

            return info.Stream;
        }
    }
}
