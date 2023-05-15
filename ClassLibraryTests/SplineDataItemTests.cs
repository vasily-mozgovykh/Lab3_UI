using ModelClassLibrary;
using FluentAssertions;

namespace ClassLibraryTests
{
    public class SplineDataItemTests
    {
        private const double MathPrecision = 1e-9d;

        [Fact]
        public void Construction()
        {
            SplineDataItem splineDataItem = new SplineDataItem(1.5d, 2.25d, 3.0d, 2.0d);
            splineDataItem.X.Should().BeApproximately(1.5d, MathPrecision);
            splineDataItem.Value.Should().BeApproximately(2.25d, MathPrecision);
            splineDataItem.FirstDerivative.Should().BeApproximately(3.0d, MathPrecision);
            splineDataItem.SecondDerivative.Should().BeApproximately(2.0d, MathPrecision);
        }

        [Fact]
        public void ToStringFormat()
        {
            string format = "0.00";
            SplineDataItem splineDataItem = new SplineDataItem(1.5d, 2.25d, 3.0d, 2.0d);
            
            string str = splineDataItem.ToString(format);

            string[] args = str.Replace("<", "").Replace(">", "").Replace(" ", "").Split(';');
            args.Should().HaveCount(4);
            args[0].IndexOf(1.5d.ToString(format)).Should().NotBe(-1);
            args[1].IndexOf(2.25d.ToString(format)).Should().NotBe(-1);
            args[2].IndexOf(3.0d.ToString(format)).Should().NotBe(-1);
            args[3].IndexOf(2.0d.ToString(format)).Should().NotBe(-1);
        }

        [Fact]
        public void ToStringOverloaded()
        {
            string format = "0.000";
            SplineDataItem splineDataItem = new SplineDataItem(1.5d, 2.25d, 3.0d, 2.0d);
            
            string str = splineDataItem.ToString();

            string[] args = str.Replace("<", "").Replace(">", "").Replace(" ", "").Split(';');
            args.Should().HaveCount(4);
            args[0].IndexOf(1.5d.ToString(format)).Should().NotBe(-1);
            args[1].IndexOf(2.25d.ToString(format)).Should().NotBe(-1);
            args[2].IndexOf(3.0d.ToString(format)).Should().NotBe(-1);
            args[3].IndexOf(2.0d.ToString(format)).Should().NotBe(-1);
        }
    }
}
