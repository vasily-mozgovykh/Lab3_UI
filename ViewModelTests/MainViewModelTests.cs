using ViewModel;
using ModelClassLibrary;
using FluentAssertions;
using Moq;
using OxyPlot;
using OxyPlot.Series;

namespace ViewModelTests
{
    public class MainViewModelTests
    {
        private const double MathPrecision = 1e-9;

        private class DataPointComparer : IComparer<DataPoint>
        {
            public int Compare(DataPoint p1, DataPoint p2) => Math.Sign(p1.X - p2.X);
        }

        private class ScatterPointComparer : IComparer<ScatterPoint>
        {
            public int Compare(ScatterPoint p1, ScatterPoint p2) => Math.Sign(p1.X - p2.X);
        }

        [Fact]
        public void ErrorScenario()
        {
            Mock<IUIServices> er = new Mock<IUIServices>();
            MainViewModel mvm = new MainViewModel(er.Object);

            mvm.NodesCount = 0;
            mvm.LoadFromControlsCommand.CanExecute(null).Should().BeFalse();
            mvm.LoadFromControlsCommand.Execute(null);

            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void BasicControlsLinearScenario()
        {
            Mock<IUIServices> er = new Mock<IUIServices>();
            MainViewModel mvm = new MainViewModel(er.Object);

            mvm.Limits = new double[2] { -2.0d, 2.0d };
            mvm.NodesCount = 11;
            mvm.IsNonUniform = true;
            mvm.FunctionName = FRawEnum.Linear;
            mvm.SplineNodesCount = 21;
            mvm.LeftFirstDerivative = 1.0d;
            mvm.RightFirstDerivative = 1.0d;

            mvm.LoadFromControlsCommand.CanExecute(null).Should().BeTrue();
            mvm.LoadFromControlsCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Never);

            mvm.Integral.Should().BeApproximately(0.0d, MathPrecision);
            
            mvm.RawDataNodes.Should().HaveCount(11);
            mvm.RawDataNodes.Should().OnlyContain(p => Math.Abs(p.X - p.Value) < MathPrecision);

            mvm.SplineDataItems.Should().HaveCount(21);
            mvm.SplineDataItems.Should().OnlyContain(p => Math.Abs(p.X - p.Value) < MathPrecision);

            LineSeries lineSeries = (LineSeries)mvm.ChartData.Series[0];
            lineSeries.Points.Should().HaveCount(21);
            lineSeries.Points.Should().BeInAscendingOrder(new DataPointComparer());
            lineSeries.Points.Should().OnlyContain(p => Math.Abs(p.X - p.Y) < MathPrecision);

            ScatterSeries scatterSeries = (ScatterSeries)mvm.ChartData.Series[1];
            scatterSeries.Points.Should().HaveCount(11);
            scatterSeries.Points.Should().BeInAscendingOrder(new ScatterPointComparer());
            scatterSeries.Points.Should().OnlyContain(p => Math.Abs(p.X - p.Y) < MathPrecision);

            er.Verify(r => r.ConfigurePlotModel(It.IsAny<PlotModel>()), Times.Once);
            er.Verify(r => r.ConfigureScatterSeries(It.IsAny<ScatterSeries>()), Times.Once);
            er.Verify(r => r.ConfigureLineSeries(It.IsAny<LineSeries>()), Times.Once);
        }

        [Fact]
        public void BasicControlsCubicScenario()
        {
            Mock<IUIServices> er = new Mock<IUIServices>();
            MainViewModel mvm = new MainViewModel(er.Object);

            mvm.Limits = new double[2] { 0.0d, 4.0d };
            mvm.NodesCount = 3;
            mvm.IsUniform = false;
            mvm.FunctionName = FRawEnum.Cubic;
            mvm.SplineNodesCount = 51;
            mvm.LeftFirstDerivative = 0.0d;
            mvm.RightFirstDerivative = 48.0d;

            mvm.LoadFromControlsCommand.CanExecute(null).Should().BeTrue();
            mvm.LoadFromControlsCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Never);

            mvm.Integral.Should().BeApproximately(64.0d, MathPrecision);

            mvm.RawDataNodes.Should().HaveCount(3);
            mvm.RawDataNodes.Should().OnlyContain(p => Math.Abs(Math.Pow(p.X, 3) - p.Value) < MathPrecision);

            mvm.SplineDataItems.Should().HaveCount(51);
            mvm.SplineDataItems.Should().OnlyContain(p => Math.Abs(Math.Pow(p.X, 3) - p.Value) < MathPrecision);

            LineSeries lineSeries = (LineSeries)mvm.ChartData.Series[0];
            lineSeries.Points.Should().HaveCount(51);
            lineSeries.Points.Should().BeInAscendingOrder(new DataPointComparer());
            lineSeries.Points.Should().OnlyContain(p => Math.Abs(Math.Pow(p.X, 3) - p.Y) < MathPrecision);

            ScatterSeries scatterSeries = (ScatterSeries)mvm.ChartData.Series[1];
            scatterSeries.Points.Should().HaveCount(3);
            scatterSeries.Points.Should().BeInAscendingOrder(new ScatterPointComparer());
            scatterSeries.Points.Should().OnlyContain(p => Math.Abs(Math.Pow(p.X, 3) - p.Y) < MathPrecision);
            
            er.Verify(r => r.ConfigurePlotModel(It.IsAny<PlotModel>()), Times.Once);
            er.Verify(r => r.ConfigureScatterSeries(It.IsAny<ScatterSeries>()), Times.Once);
            er.Verify(r => r.ConfigureLineSeries(It.IsAny<LineSeries>()), Times.Once);
        }

        [Fact]
        public void FunctionNamesCheck()
        {
            Mock<IUIServices> er = new Mock<IUIServices>();
            MainViewModel mvm = new MainViewModel(er.Object);
            foreach (FRawEnum functionName in Enum.GetValues(typeof(FRawEnum)))
                mvm.FunctionNames.Should().Contain(functionName);
        }

        [Fact]
        public void SaveLoadPseudoRandom()
        {
            string filename = "saveLoadFileName";
            Mock<IUIServices> er = new Mock<IUIServices>();
            MainViewModel mvm = new MainViewModel(er.Object);
            er.SetReturnsDefault(filename);

            mvm.Limits = new double[2] { 5.0d, 6.0d };
            mvm.NodesCount = 9;
            mvm.IsUniform = false;
            mvm.FunctionName = FRawEnum.PseudoRandom;
            mvm.LoadFromControlsCommand.Execute(null);

            mvm.SaveCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Never);
            er.Verify(r => r.GetSaveFilename(), Times.Once);

            RawDataNode[] nodesValues = new RawDataNode[9];
            mvm.RawDataNodes.CopyTo(nodesValues);

            mvm.Limits = new double[2] { -4.0d, -2.0d };
            mvm.NodesCount = 4;
            mvm.IsUniform = true;
            mvm.FunctionName = FRawEnum.Linear;
            mvm.LoadFromControlsCommand.Execute(null);

            mvm.LoadFromFileCommand.Execute(null);
            er.Verify(r => r.ReportError(It.IsAny<string>()), Times.Never);
            er.Verify(r => r.GetLoadFileName(), Times.Once);

            mvm.Limits.Should().HaveCount(2);
            mvm.Limits[0].Should().BeApproximately(5.0d, MathPrecision);
            mvm.Limits[1].Should().BeApproximately(6.0d, MathPrecision);
            mvm.NodesCount.Should().Be(9);
            mvm.IsUniform.Should().BeFalse();
            mvm.FunctionName.Should().Be(FRawEnum.PseudoRandom);

            mvm.RawDataNodes.Should().HaveCount(9);
            for (int i = 0; i < 9; i++)
                mvm.RawDataNodes[i].Should().Be(nodesValues[i]);

            er.Verify(r => r.ConfigurePlotModel(It.IsAny<PlotModel>()), Times.Exactly(3));
            er.Verify(r => r.ConfigureScatterSeries(It.IsAny<ScatterSeries>()), Times.Exactly(3));
            er.Verify(r => r.ConfigureLineSeries(It.IsAny<LineSeries>()), Times.Exactly(3));
        }
    }
}