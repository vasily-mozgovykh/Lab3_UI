using ModelClassLibrary;
using FluentAssertions;

namespace ClassLibraryTests
{
    public class SplineDataTests
    {
        private const double MathPrecision = 1e-9d;

        [Fact]
        public void Construction()
        {
            RawData rawData = new RawData(-1.0d, 1.0d, 3, true, FRawFunctions.Cubic);
            SplineData splineData = new SplineData(rawData, new double[2] { 3.0d, 3.0d }, 5);
            splineData.Should().NotBeNull();

            splineData.RawData.Should().NotBeNull();
            
            splineData.BoundaryConditions.Should().NotBeNull();
            splineData.BoundaryConditions.Should().HaveCount(2);
            splineData.BoundaryConditions[0].Should().BeApproximately(3.0d, MathPrecision);
            splineData.BoundaryConditions[1].Should().BeApproximately(3.0d, MathPrecision);

            splineData.SplineNodesCount.Should().Be(5);

            splineData.Items.Should().BeEmpty();
        }

        [Fact]
        public void SplineStep()
        {
            RawData rawData = new RawData(-1.0d, 1.0d, 3, true, FRawFunctions.Cubic);
            SplineData splineData;
            
            splineData = new SplineData(rawData, new double[2] { 3.0d, 3.0d }, 5);
            splineData.SplineStep.Should().BeApproximately(0.5d, MathPrecision);

            splineData = new SplineData(rawData, new double[2] { 3.0d, 3.0d }, 1);
            double.IsFinite(splineData.SplineStep).Should().BeFalse();
        }

        [Fact]
        public void CalculateSplineCubic1()
        {
            RawData rawData = new RawData(-1.0d, 1.0d, 3, true, FRawFunctions.Cubic);
            SplineData splineData = new SplineData(rawData, new double[2] { 3.0d, 3.0d }, 5);
            splineData.CalculateSpline();

            splineData.Items.Should().NotBeNull();
            splineData.Items.Should().HaveCount(5);
            foreach (SplineDataItem item in splineData.Items)
            {
                item.X.Should().BeInRange(-1.0d, 1.0d);
                item.Value.Should().BeApproximately(Math.Pow(item.X, 3), MathPrecision);
                item.FirstDerivative.Should().BeApproximately(3.0d * Math.Pow(item.X, 2), MathPrecision);
                item.SecondDerivative.Should().BeApproximately(6.0d * item.X, MathPrecision);
            }
            splineData.Integral.Should().BeApproximately(0.0d, MathPrecision);
        }

        [Fact]
        public void CalculateSplineCubic2()
        {
            RawData rawData = new RawData(0.0d, 4.0d, 5, true, FRawFunctions.Cubic);
            SplineData splineData = new SplineData(rawData, new double[2] { 0.0d, 48.0d }, 9);
            splineData.CalculateSpline();

            splineData.Items.Should().NotBeNull();
            splineData.Items.Should().HaveCount(9);
            foreach (SplineDataItem item in splineData.Items)
            {
                item.X.Should().BeInRange(0.0d, 4.0d);
                item.Value.Should().BeApproximately(Math.Pow(item.X, 3), MathPrecision);
                item.FirstDerivative.Should().BeApproximately(3.0d * Math.Pow(item.X, 2), MathPrecision);
                item.SecondDerivative.Should().BeApproximately(6.0d * item.X, MathPrecision);
            }
            splineData.Integral.Should().BeApproximately(64.0d, MathPrecision);
        }

        [Fact]
        public void CalculateSplineLinear()
        {
            RawData rawData = new RawData(0.0d, 4.0d, 5, true, FRawFunctions.Linear);
            SplineData splineData = new SplineData(rawData, new double[2] { 1.0d, 1.0d }, 401);
            splineData.CalculateSpline();

            splineData.Items.Should().NotBeNull();
            splineData.Items.Should().HaveCount(401);
            foreach (SplineDataItem item in splineData.Items)
            {
                item.X.Should().BeInRange(0.0d, 4.0d);
                item.Value.Should().BeApproximately(item.X, MathPrecision);
                item.FirstDerivative.Should().BeApproximately(1.0d, MathPrecision);
                item.SecondDerivative.Should().BeApproximately(0.0d, MathPrecision);
            }
            splineData.Integral.Should().BeApproximately(8.0d, MathPrecision);
        }

        [Fact]
        public void CalculateSplinePseudoRandom()
        {
            RawData rawData = new RawData(0.0d, 4.0d, 5, true, FRawFunctions.PseudoRandom);
            SplineData splineData = new SplineData(rawData, new double[2] { 0.0d, 48.0d }, 157);
            splineData.CalculateSpline();

            splineData.Items.Should().NotBeNull();
            splineData.Items.Should().HaveCount(157);
            foreach (SplineDataItem item in splineData.Items)
            {
                item.X.Should().BeInRange(0.0d, 4.0d);
            }
        }

        [Fact]
        public void CalculateSplineIncorrectInput()
        {
            RawData rawData = new RawData(0.0d, 4.0d, 1, true, FRawFunctions.Cubic);
            SplineData splineData = new SplineData(rawData, new double[2] { 0.0d, 48.0d }, 157);
            Action code = () => { splineData.CalculateSpline(); };
            code.Should().Throw<Exception>();
        }
    }
}
