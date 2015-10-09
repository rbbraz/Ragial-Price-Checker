/*
 * Created by SharpDevelop.
 * User: a039292
 * Date: 02/21/2013
 * Time: 09:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace RagialPriceChecker.View
{
	/// <summary>
	/// Interaction logic for NotificationWnd.xaml
	/// </summary>
	public partial class NotificationWnd : Window
	{
		public NotificationWnd()
		{
			InitializeComponent();
			
			this.SizeChanged += onSizeChanged;
		}
		
		void Window_Loaded(object sender, RoutedEventArgs e)
		{
			onSizeChanged(this, null);
		}
		
		// Repositions window --- seems to be working fine
		void onSizeChanged(object sender, EventArgs e)
		{		
			var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
    		this.Left = desktopWorkingArea.Right - this.ActualWidth - 5;
    		this.Top = desktopWorkingArea.Bottom - this.ActualHeight - 5;
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}
		
		void ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			onSizeChanged(this, null);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			//timer.Stop();
		}		
	}
}