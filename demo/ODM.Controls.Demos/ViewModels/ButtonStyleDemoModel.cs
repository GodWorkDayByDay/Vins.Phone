using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

namespace ODM.Controls.Demos
{
	public class ButtonStyleDemoModel : INotifyPropertyChanged
	{
		public ButtonStyleDemoModel()
		{
			
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
		#endregion
	}
}