using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ODM.Controls.MultiIndicatorDemos
{
	/// <summary>
	/// Interaction logic for MultiIndicatorDemoView.xaml
	/// </summary>
	public partial class MultiIndicatorDemoView : UserControl
	{
		public MultiIndicatorDemoView()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.

		    var bs = new BitmapImage(new Uri(@"D:\@\Vins.Phone\Vins.Phone_3.x(git)\Base2\NO.10_ResizeY.bmp", UriKind.RelativeOrAbsolute));
		    ZoomImageViewer.BitmapSource = bs;
		}
	}
}