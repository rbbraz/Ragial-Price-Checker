using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace RagialPriceChecker.ViewModel
{
    public class Log
    {
        private static int _cnt = 0;

        string _text;
        int _index;
        public int Index 
        { 
            get { return _index; }           
        }
        public string Text { get { return _text; } }

        public Log(string text)
        {
            _index = _cnt++;
            _text = DateTime.Now.ToString() + " - " + text;
        }
    }

    public class LogsViewModel : ViewModelBase
    {
        ObservableCollection<Log> _logs;
        Dispatcher _dispatcher;

        public ObservableCollection<Log> Logs
        {
            get
            {
                return _logs; // (ObservableCollection<Log>)_logs.OrderBy(i => i.Index);
            }
            set
            {
                _logs = value;
            }
        }

        public LogsViewModel(Dispatcher dispatcher)
        {
            _logs = new ObservableCollection<Log>();
            _dispatcher = dispatcher;
        }

        public void Add(string text)
        {
            Action del = () => DispatchedAdd(text);
            _dispatcher.Invoke(del);
        }

        void DispatchedAdd(string text)
        {
            Logs.Insert(0, (new Log(text)));
        }
    }
}
