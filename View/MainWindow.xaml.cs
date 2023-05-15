using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Windows;
using ViewModel;

namespace View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUIServices
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this);
        }

        public void ReportError(string message)
        {
            MessageBox.Show(message);
        }

        public string? GetSaveFilename()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() != null && dialog.FileName != "") { return dialog.FileName; }
            return null;
        }

        public string? GetLoadFileName()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != null && dialog.FileName != "") { return dialog.FileName; }
            return null;
        }

        public void ConfigurePlotModel(PlotModel plotModel)
        {
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "0.000",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid
            });
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                StringFormat = "0.000",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid
            });
            plotModel.Legends.Add(new Legend { LegendPosition = LegendPosition.TopLeft });
        }

        public void ConfigureScatterSeries(ScatterSeries series)
        {
            series.MarkerSize = 4;
            series.MarkerFill = OxyColors.OrangeRed;
            series.MarkerStroke = OxyColors.Red;
            series.MarkerType = MarkerType.Diamond;
            series.Title = "f(x)";
        }

        public void ConfigureLineSeries(LineSeries series)
        {
            series.Color = OxyColors.YellowGreen;
            series.MarkerSize = 1.5;
            series.MarkerFill = OxyColors.DarkGoldenrod;
            series.MarkerStroke = OxyColors.Black;
            series.MarkerType = MarkerType.Circle;
            series.MarkerStrokeThickness = 1;
            series.Title = "S(x)";
        }
    }
}
