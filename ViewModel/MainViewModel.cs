using ModelClassLibrary;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ViewModel
{
    public record RawDataNode(double X, double Value);

    public interface IUIServices
    {
        void ReportError(string message);
        string? GetSaveFilename();
        string? GetLoadFileName();
        void ConfigurePlotModel(PlotModel plotModel);
        void ConfigureScatterSeries(ScatterSeries series);
        void ConfigureLineSeries(LineSeries series);
    }

    public class MainViewModel : ViewModelBase
    {
        #region FIELDS
        private readonly IUIServices UIServices;
        private double[] LimitsValue { get; set; }
        private int NodesCountValue { get; set; }
        private bool IsUniformValue { get; set; }
        private FRawEnum FunctionNameValue { get; set; }
        private int SplineNodesCountValue { get; set; }
        private double LeftFirstDerivativeValue { get; set; }
        private double RightFirstDerivativeValue { get; set; }
        private RawData RawData { get; set; }
        private SplineData SplineData { get; set; }
        #endregion

        #region PUBLIC PROPERTIES
        public double[] Limits
        {
            get => LimitsValue;
            set
            {
                if (value != null && (value[0] != LimitsValue[0] || value[1] != LimitsValue[1]))
                {
                    LimitsValue[0] = value[0]; LimitsValue[1] = value[1];
                    RaisePropertyChanged(nameof(Limits));
                }
            }
        }
        public int NodesCount
        {
            get => NodesCountValue;
            set
            {
                if (value != NodesCountValue)
                {
                    NodesCountValue = value;
                    RaisePropertyChanged(nameof(NodesCount));
                }
            }
        }
        public bool IsUniform
        {
            get => IsUniformValue;
            set
            {
                if (value != IsUniformValue)
                {
                    IsUniformValue = value;
                    RaisePropertyChanged(nameof(IsUniform));
                    RaisePropertyChanged(nameof(IsNonUniform));
                }
            }
        }
        public bool IsNonUniform
        {
            get => !IsUniformValue;
            set
            {
                if (value != !IsUniformValue)
                {
                    IsUniformValue = !value;
                    RaisePropertyChanged(nameof(IsUniform));
                    RaisePropertyChanged(nameof(IsNonUniform));
                }
            }
        }
        public FRawEnum FunctionName
        {
            get => FunctionNameValue;
            set
            {
                if (value != FunctionNameValue)
                {
                    FunctionNameValue = value;
                    RaisePropertyChanged(nameof(FunctionName));
                }
            }
        }
        public int SplineNodesCount
        {
            get => SplineNodesCountValue;
            set
            {
                if (value != SplineNodesCountValue)
                {
                    SplineNodesCountValue = value;
                    RaisePropertyChanged(nameof(SplineNodesCount));
                }
            }
        }
        public double LeftFirstDerivative
        {
            get => LeftFirstDerivativeValue;
            set
            {
                if (value != LeftFirstDerivativeValue)
                {
                    LeftFirstDerivativeValue = value;
                    RaisePropertyChanged(nameof(LeftFirstDerivative));
                }
            }
        }
        public double RightFirstDerivative
        {
            get => RightFirstDerivativeValue;
            set
            {
                if (value != RightFirstDerivativeValue)
                {
                    RightFirstDerivativeValue = value;
                    RaisePropertyChanged(nameof(RightFirstDerivative));
                }
            }
        }
        public double? Integral { get; private set; } = null;
        public List<FRawEnum> FunctionNames { get; private set; }
        public List<RawDataNode> RawDataNodes { get; private set; }
        public List<SplineDataItem> SplineDataItems { get; private set; }
        public PlotModel ChartData { get; private set; }
        #endregion
        
        #region VALIDATION
        public override string Error => base.Error;
        public override string this[string propertyName]
        {
            get
            {
                string msg = null;
                switch (propertyName)
                {
                    case "NodesCount":
                        if (NodesCount < 2) { msg = "Nodes count should be strictly greater than 2"; }
                        break;
                    case "SplineNodesCount":
                        if (SplineNodesCount < 2) { msg = "Spline nodes count should be strictly greater than 2"; }
                        break;
                    case "Limits":
                        if (Limits[0] >= Limits[1])
                            msg = "Left limit should be strictly less than right limit";
                        break;
                    default:
                        break;
                }
                return msg;
            }
        }
        #endregion

        private void FillTableData()
        {
            RawDataNodes = new List<RawDataNode>();
            for (int i = 0; i < RawData.NodesCount; i++)
                RawDataNodes.Add(new RawDataNode(RawData.Nodes[i], RawData.Values[i]));
            RaisePropertyChanged(nameof(RawDataNodes));

            SplineDataItems = SplineData.Items;
            RaisePropertyChanged(nameof(SplineDataItems));

            Integral = SplineData.Integral;
            RaisePropertyChanged(nameof(Integral));
        }

        private void FillChartData()
        {
            ChartData = new PlotModel() { Title = "" };

            LineSeries lineSeries = new LineSeries();
            foreach (SplineDataItem splineDataItem in SplineData.Items)
                lineSeries.Points.Add(new DataPoint(splineDataItem.X, splineDataItem.Value));
            UIServices.ConfigureLineSeries(lineSeries);
            ChartData.Series.Add(lineSeries);

            ScatterSeries scatterSeries = new ScatterSeries();
            for (int i = 0; i < RawData.NodesCount; i++)
                scatterSeries.Points.Add(new ScatterPoint(RawData.Nodes[i], RawData.Values[i]));
            UIServices.ConfigureScatterSeries(scatterSeries);
            ChartData.Series.Add(scatterSeries);

            UIServices.ConfigurePlotModel(ChartData);
            RaisePropertyChanged(nameof(ChartData));
        }

        #region COMMANDS
        private void OnSave(object arg)
        {
            try
            {
                string? saveFileName = UIServices.GetSaveFilename();
                if (saveFileName == null) { return; }
                double a = Limits[0], b = Limits[1];
                if (RawData == null)
                    RawData = new RawData(a, b, NodesCount, IsUniform, FRawFunctions.ToFunction(FunctionName));
                RawData.Save(saveFileName);
            }
            catch (Exception e)
            {
                UIServices.ReportError(e.Message);
            }
        }
        private void OnLoadFromControls(object arg)
        {
            try
            {
                double a = Limits[0], b = Limits[1];
                double[] boundaryConditions = new double[2] { LeftFirstDerivative, RightFirstDerivative };
                
                RawData = new RawData(a, b, NodesCount, IsUniform, FRawFunctions.ToFunction(FunctionName));
                SplineData = new SplineData(RawData, boundaryConditions, SplineNodesCount);
                SplineData.CalculateSpline();
                FillTableData();
                FillChartData();
            }
            catch (Exception e)
            {
                UIServices.ReportError(e.Message);
            }
        }
        private void OnLoadFromFile(object arg)
        {
            try
            {
                string? loadFileName = UIServices.GetLoadFileName();
                if (loadFileName == null) { return; }
                double[] boundaryConditions = new double[2] { LeftFirstDerivative, RightFirstDerivative };
                
                RawData = RawData.Load(loadFileName);
                SplineData = new SplineData(RawData, boundaryConditions, SplineNodesCount);
                SplineData.CalculateSpline();

                Limits = new double[2] { RawData.Left, RawData.Right };
                NodesCount = RawData.NodesCount;
                IsUniform = RawData.IsUniform;
                FunctionName = FRawFunctions.ToEnum(RawData.Function);
                FillTableData();
                FillChartData();
            }
            catch (Exception e)
            {
                UIServices.ReportError(e.Message);
            }
        }
        public ICommand SaveCommand { get; private set; }
        public ICommand LoadFromControlsCommand { get; private set; }
        public ICommand LoadFromFileCommand { get; private set; }
        #endregion

        public MainViewModel(IUIServices uIServices)
        {
            UIServices = uIServices;

            LimitsValue = new double[2] { -1.0d, 1.0d };
            NodesCountValue = 3;
            IsUniformValue = true;
            FunctionNameValue = FRawEnum.Linear;

            SplineNodesCountValue = 5;
            LeftFirstDerivativeValue = 1.0d;
            RightFirstDerivativeValue = 1.0d;

            FunctionNames = new List<FRawEnum>();
            foreach (FRawEnum functionName in Enum.GetValues(typeof(FRawEnum)))
            {
                FunctionNames.Add(functionName);
            }
            RawDataNodes = new List<RawDataNode>();
            SplineDataItems = new List<SplineDataItem>();

            SaveCommand = new RelayCommand(OnSave,
                _ => RawData != null && Limits[0] < Limits[1] && NodesCount >= 2);
            LoadFromControlsCommand = new RelayCommand(OnLoadFromControls,
                _ => Limits[0] < Limits[1] && NodesCount >= 2 && SplineNodesCount >= 2);
            LoadFromFileCommand = new RelayCommand(OnLoadFromFile,
                _ => SplineNodesCount >= 2);
        }
    }
}
