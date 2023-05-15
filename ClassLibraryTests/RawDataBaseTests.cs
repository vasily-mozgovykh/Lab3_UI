using ModelClassLibrary;
using FluentAssertions;

namespace ClassLibraryTests
{
    public class RawDataBaseTests
    {
        private const double MathPrecision = 1e-9d;

        [Fact]
        public void Construction()
        {
            RawDataBase rawDataBase;

            rawDataBase = new RawDataBase();
            rawDataBase.Left.Should().BeApproximately(0.0d, MathPrecision);
            rawDataBase.Right.Should().BeApproximately(0.0d, MathPrecision);
            rawDataBase.NodesCount.Should().Be(0);
            rawDataBase.IsUniform.Should().BeFalse();

            rawDataBase = new RawDataBase(1.0d, 2.0d, 3, true);
            rawDataBase.Left.Should().BeApproximately(1.0d, MathPrecision);
            rawDataBase.Right.Should().BeApproximately(2.0d, MathPrecision);
            rawDataBase.NodesCount.Should().Be(3);
            rawDataBase.IsUniform.Should().BeTrue();
        }

        [Fact]
        public void Step()
        {
            RawDataBase rawDataBase;

            rawDataBase = new RawDataBase(1.0d, 2.0d, 3, true);
            rawDataBase.Step.Should().BeApproximately(0.5d, MathPrecision);

            rawDataBase = new RawDataBase(1.0d, 2.0d, 1, true);
            double.IsFinite(rawDataBase.Step).Should().BeFalse();
        }

        [Fact]
        public void Serialization()
        {
            RawDataBase rawDataBase = new RawDataBase(1.0d, 2.0d, 3, true);
            string[] args = rawDataBase.Serialize().Split('\n');
            args.Length.Should().Be(4);
            Convert.ToDouble(args[0]).Should().BeApproximately(1.0d, MathPrecision);
            Convert.ToDouble(args[1]).Should().BeApproximately(2.0d, MathPrecision);
            Convert.ToInt32(args[2]).Should().Be(3);
            Convert.ToBoolean(args[3]).Should().BeTrue();
        }
    }
}
