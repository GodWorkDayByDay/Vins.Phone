using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hdc.Collections.Generic;
using Hdc.Diagnostics;
using Hdc.Mv;
using Hdc.Mv.Halcon;
using Hdc.Mv.Inspection;
using Hdc.Mv.Inspection.Halcon;
using Hdc.Mv.Inspection.Halcon.SampleApp;
using Hdc.Mv.Inspection.Halcon.SampleApp.Annotations;
using Hdc.Mv.Inspection.Mil;
using Hdc.Reflection;
using Hdc.Serialization;
using Hdc.Windows.Media.Imaging;
using MeasurementTestApp;
using Microsoft.Win32;
using Line = Hdc.Mv.Line;
using Path = System.IO.Path;

namespace ODM.Inspectors.Halcon.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BitmapSource _bitmapSource;

        private bool isInitilized;
        private IGeneralInspector inspector;
        private double _circle1CenterX;
        private double _circle1CenterY;
        private double _circle2CenterX;
        private double _circle2CenterY;
        private double _distanceBetweenC1C2;
        private double _circle3CenterX;
        private double _circle3CenterY;
        private double _distanceBetweenC1C3;
        private double _xDistanceC1C3;
        private double _yDistanceC1C3;
        private double _circle3CenterXRelative;
        private double _circle3CenterYRelative;
        private double _circle2CenterXRelative;
        private double _circle2CenterYRelative;
        private string _text1Name;
        private string _text1Content;
        private string _text2Name;
        private string _text2Content;
//        private IRunner _runner;

        public ObservableCollection<RegionIndicatorViewModel> RegionIndicators { get; set; }
        public ObservableCollection<RectangleIndicatorViewModel> DefectIndicators { get; set; }
        public ObservableCollection<LineIndicatorViewModel> LineIndicators { get; set; }
        public ObservableCollection<CircleIndicatorViewModel> CircleIndicators { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            RegionIndicators = new ObservableCollection<RegionIndicatorViewModel>();
            DefectIndicators = new ObservableCollection<RectangleIndicatorViewModel>();
            LineIndicators = new ObservableCollection<LineIndicatorViewModel>();
            CircleIndicators = new ObservableCollection<CircleIndicatorViewModel>();

            this.DataContext = this;
            this.Closing += MainWindow_Closing;


            var inspectorFactory = new Func<string, IGeneralInspector>(
                name =>
                {
                    switch (name)
                    {
                        case "Sim":
                            {
                                var sim = new SimGeneralInspector();
                                return sim;
                            }
                            break;
//                        case "Mil":
//                            {
//                                var mi = new MilGeneralInspector();
//                                mi.Init(schema.ImagePixelWidth, schema.ImagePixelHeight);
//                                inspector = mi;
//
//                                return mi;
//                            }
//                            break;
                        case "Hal":
                            {
                                var hi = new HalconGeneralInspector();
                                return hi;
                            }
                            break;
                        default:
                            throw new NotSupportedException("InspectionSchema.InspectorName not be set!");
                    }
                });


            InspectionController = new InspectionController();
            InspectionController.SetInspectorFactory(inspectorFactory);

            //
            Refresh();
        }

        private InspectionController InspectionController;

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            InspectionController.Dispose();
        }

        private void Inspect(InspectionSchema schema)
        {
            RegionIndicators.Clear();
            LineIndicators.Clear();
            CircleIndicators.Clear();
            DefectIndicators.Clear();

            BitmapImage bi;

            try
            {
                bi = new BitmapImage(new Uri(schema.TestImageFilePath, UriKind.RelativeOrAbsolute));
            }
            catch (FileNotFoundException e)
            {
                throw new HalconInspectorException("Image file not exist", e);
            }

            BitmapSourceInfo bsi = bi.ToGray8BppBitmapSourceInfo();
            bsi.DpiX = 96;
            bsi.DpiY = 96;
            _bitmapSource = bsi.GetBitmapSource();
            IndicatorViewer.BitmapSource = _bitmapSource;

            //IndicatorViewer.Loaded += (sender, args) => IndicatorViewer.ZoomFit();
            //                        IndicatorViewer.Loaded += (sender, args) => IndicatorViewer.ZoomOut();

            var imageInfo = _bitmapSource.ToImageInfoWith8Bpp();

            _bitmapSource = imageInfo.ToBitmapSource();
            IndicatorViewer.BitmapSource = _bitmapSource;

            using (var sw = new NotifyStopwatch("AllRunner.InspectionController.Inspect()"))
            {
                InspectionController
                    .StartInspect()
                    .SetInspectionSchema()
                    .SetImageInfo(bi.ToHImage())
                    .CreateCoordinate()
                    .Inspect()
                    ;
            }

            Show_CircleSearchingDefinitions(
                InspectionController.InspectionResult.GetCoordinateCircleSearchingDefinitions());
            Show_CircleSearchingResults(InspectionController.InspectionResult.CoordinateCircles);

            Show_CircleSearchingDefinitions(
                InspectionController.InspectionResult.GetCircleSearchingDefinitions(), Brushes.DodgerBlue);
            Show_CircleSearchingResults(InspectionController.InspectionResult.Circles, Brushes.DodgerBlue);

            Show_EdgeSearchingDefinitions(InspectionController.InspectionResult.GetEdgeSearchingDefinitions());
            Show_EdgeSearchingResults(InspectionController.InspectionResult.Edges);
            Show_DistanceBetweenPointsResults(InspectionController.InspectionResult.DistanceBetweenPointsResults);
            Show_DefectResults(InspectionController.InspectionResult.DefectResults);
        }

        public void Show_DistanceBetweenLinesResults(
            IEnumerable<DistanceBetweenLinesResult> distanceBetweenLinesResults)
        {
            foreach (var result in distanceBetweenLinesResults)
            {
                var distanceLineIndicator = new LineIndicatorViewModel
                                            {
                                                StartPointX = result.FootPoint1.X,
                                                StartPointY = result.FootPoint1.Y,
                                                EndPointX = result.FootPoint2.X,
                                                EndPointY = result.FootPoint2.Y,
                                                Stroke = Brushes.Lime,
                                                StrokeThickness = 2,
                                                StrokeDashArray = new DoubleCollection() { 2, 2 },
                                            };
                LineIndicators.Add(distanceLineIndicator);
            }
        }
        public void Show_DistanceBetweenPointsResults(
            DistanceBetweenPointsResultCollection pointsResultCollection)
        {
            foreach (var result in pointsResultCollection)
            {
                var distanceLineIndicator = new LineIndicatorViewModel
                                            {
                                                StartPointX = result.Point1.X,
                                                StartPointY = result.Point1.Y,
                                                EndPointX = result.Point2.X,
                                                EndPointY = result.Point2.Y,
                                                Stroke = Brushes.Lime,
                                                StrokeThickness = 2,
                                                StrokeDashArray = new DoubleCollection() { 2, 2 },
                                            };
                LineIndicators.Add(distanceLineIndicator);
            }
        }

        public void Show_CircleSearchingResults(IList<CircleSearchingResult> CircleSearchingResults, Brush brush = null)
        {
            if (brush == null)
                brush = Brushes.Lime;

            foreach (var circleSearchingResult in CircleSearchingResults)
            {
                var ci = new CircleIndicatorViewModel()
                         {
                             CenterX = circleSearchingResult.Circle.CenterX,
                             CenterY = circleSearchingResult.Circle.CenterY,
                             Radius = circleSearchingResult.Circle.Radius,
                             Stroke = brush,
                             StrokeThickness = 2,
                             StrokeDashArray = null,
                         };
                if (circleSearchingResult.IsNotFound)
                    ci.Stroke = Brushes.Red;

                CircleIndicators.Add(ci);
            }

            if (CircleSearchingResults.Count >= 2)
            {
                var Circle1CenterX = CircleSearchingResults[0].Circle.CenterX;
                var Circle1CenterY = CircleSearchingResults[0].Circle.CenterY;
                var vector1 = new Vector(Circle1CenterX, Circle1CenterY);


                for (int i = 1; i < CircleSearchingResults.Count; i++)
                {
                    var Circle2CenterX = CircleSearchingResults[i].Circle.CenterX;
                    var Circle2CenterY = CircleSearchingResults[i].Circle.CenterY;

                    var vector2 = new Vector(Circle2CenterX, Circle2CenterY);

                    var diff = vector1 - vector2;
                    var distance = diff.Length;
                    var DistanceBetweenC1C2 = distance;


                    var distanceLineIndicator = new LineIndicatorViewModel
                                                {
                                                    StartPointX = Circle1CenterX,
                                                    StartPointY = Circle1CenterY,
                                                    EndPointX = Circle2CenterX,
                                                    EndPointY = Circle2CenterY,
                                                    Stroke = brush,
                                                    StrokeThickness = 2,
                                                    StrokeDashArray = new DoubleCollection() { 4, 4 },
                                                };
                    LineIndicators.Add(distanceLineIndicator);
                }
            }
        }

        public void Show_CircleSearchingDefinitions(IEnumerable<CircleSearchingDefinition> circleSearchingDefinitions,
                                                    Brush brush = null)
        {
            if (brush == null)
                brush = Brushes.Green;

            foreach (var circleSearchingDefinition in circleSearchingDefinitions)
            {
                var innerCircle = new CircleIndicatorViewModel()
                                  {
                                      CenterX = circleSearchingDefinition.CenterX,
                                      CenterY = circleSearchingDefinition.CenterY,
                                      Radius = circleSearchingDefinition.InnerRadius,
                                      Stroke = brush,
                                      StrokeThickness = 1,
                                      StrokeDashArray = new DoubleCollection() { 4, 4 },
                                  };
                CircleIndicators.Add(innerCircle);
                var outerCircle = new CircleIndicatorViewModel()
                                  {
                                      CenterX = circleSearchingDefinition.CenterX,
                                      CenterY = circleSearchingDefinition.CenterY,
                                      Radius = circleSearchingDefinition.OuterRadius,
                                      Stroke = brush,
                                      StrokeThickness = 1,
                                      StrokeDashArray = new DoubleCollection() { 4, 4 },
                                  };
                CircleIndicators.Add(outerCircle);
            }
        }

        public void Show_EdgeSearchingDefinitions(IEnumerable<EdgeSearchingDefinition> edgeSearchingDefinitions)
        {
            foreach (var ed in edgeSearchingDefinitions)
            {
                var regionIndicator = new RegionIndicatorViewModel
                                      {
                                          StartPointX = ed.StartX,
                                          StartPointY = ed.StartY,
                                          EndPointX = ed.EndX,
                                          EndPointY = ed.EndY,
                                          RegionWidth = ed.ROIWidth,
                                      };
                RegionIndicators.Add(regionIndicator);
            }
        }

        public void Show_EdgeSearchingResults(IEnumerable<EdgeSearchingResult> edgeSearchingResults)
        {
            foreach (var edgeSearchingResult in edgeSearchingResults)
            {
                if (edgeSearchingResult.HasError) continue;

                var lineIndicator = new LineIndicatorViewModel
                                    {
                                        StartPointX = edgeSearchingResult.EdgeLine.X1,
                                        StartPointY = edgeSearchingResult.EdgeLine.Y1,
                                        EndPointX = edgeSearchingResult.EdgeLine.X2,
                                        EndPointY = edgeSearchingResult.EdgeLine.Y2,
                                        Stroke = Brushes.Lime,
                                        StrokeThickness = 4,
                                    };
                LineIndicators.Add(lineIndicator);
            }
        }


        public void Show_DefectResults(IEnumerable<DefectResult> defectResults)
        {
            foreach (var dr in defectResults)
            {
                var regionIndicator = new RectangleIndicatorViewModel
                {
                    CenterX = dr.X,
                    CenterY = dr.Y,
                    Width = dr.Width,
                    Height = dr.Height,
                    IsShown = true,
                };
                DefectIndicators.Add(regionIndicator);


//                var regionIndicator2 = new RegionIndicatorViewModel
//                {
//                    StartPointX = dr.X - dr.Width/2,
//                    StartPointY = dr.Y,
//                    EndPointX = dr.X + dr.Width / 2,
//                    EndPointY = dr.Y,
//                    RegionWidth = dr.Height / 2.0,
//                };
//                RegionIndicators.Add(regionIndicator2);
            }

        }

        private void SaveImageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
                         {
                             DefaultExt = ".tif",
                             FileName = "ExportImage_" + DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss"),
                             Filter = ".tif|TIFF"
                         };
            var r = dialog.ShowDialog();
            if (r == true)
            {
                //                if (!dialog.FileName.Contains(".tif"))
                //                {
                //                    dialog.FileName += ".tif";
                //                }
                _bitmapSource.SaveToTiff(dialog.FileName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            IndicatorViewer.Refresh();

            Refresh();
        }

        private void Refresh()
        {
            //            var schema = "InspectionSchema.xaml".LoadFromAssemblyDir();
            var schema = InspectionController.GetInspectionSchema();

            Inspect(schema);
        }

        private void ZoomFitButton_OnClick(object sender, RoutedEventArgs e)
        {
            IndicatorViewer.ZoomFit();
        }

        private void ZoomActualButton_OnClick(object sender, RoutedEventArgs e)
        {
            IndicatorViewer.ZoomActual();
        }

        private void ZoomHalfButton_OnClick(object sender, RoutedEventArgs e)
        {
            IndicatorViewer.Scale = 0.5;
        }

        private void ZoomQuarterButton_OnClick(object sender, RoutedEventArgs e)
        {
            IndicatorViewer.Scale = 0.25;
        }

        private void ExportReportButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "InspectionReports_" + DateTime.Now.ToString("_yyyy-MM-dd_HH.mm.ss");
            dialog.Filter = "*.csv|CSV";
            dialog.DefaultExt = ".csv";
            var ok = dialog.ShowDialog();
            if (ok != true)
                return;

            var fileName = dialog.FileName;
        }
    }
}