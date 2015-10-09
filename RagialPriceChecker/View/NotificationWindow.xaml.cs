using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace RagialPriceChecker.View
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public ObservableCollection<It> Items { get; set; }

        public NotificationWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            Items = new ObservableCollection<It>();
            Items.Add(new It() { Name = "Google", URL = "http://www.google.com" });

            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                //var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                //var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                //var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
                //this.Left = corner.X - this.ActualWidth - 100;
                //this.Top = corner.Y - this.ActualHeight;

                this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.ActualWidth;
                this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.ActualHeight - 32;
            }));

        }

        // Will need a timer to close this thing down.. among other things

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }        
    }

    public class It
    {
        public string URL { get; set; }
        public string Name { get; set; }

        ICommand _goToUrl;
        public ICommand GoToURLCommand
        {
            get
            {
                if (_goToUrl == null)
                {
                    _goToUrl = new RelayCommand(param => this.OpenURL(param));
                }

                return _goToUrl;
            }
        }

        public void OpenURL(object param)
        {
            System.Diagnostics.Process.Start((string)param);
        }
    }
}
