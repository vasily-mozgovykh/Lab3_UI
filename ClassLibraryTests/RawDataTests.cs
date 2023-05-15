using ModelClassLibrary;
using FluentAssertions;

namespace ClassLibraryTests
{
    public class RawDataTests
    {
        private const double MathPrecision = 1e-9d;

        [Fact]
        public void ConstructionLinear()
        {
            RawData rawData = new RawData(1.0d, 2.0d, 3, true, FRawFunctions.Linear);
            rawData.Function.Method.Name.Should().Be("Linear");

            rawData.Left.Should().BeApproximately(1.0d, MathPrecision);
            rawData.Right.Should().BeApproximately(2.0d, MathPrecision);
            rawData.NodesCount.Should().Be(3);
            rawData.IsUniform.Should().BeTrue();

            rawData.Nodes.Should().NotBeNull();
            rawData.Nodes.Should().HaveCount(3);

            rawData.Values.Should().NotBeNull();
            rawData.Values.Should().HaveCount(3);

            double[] args = new double[3] { 1.0d, 1.5d, 2.0d };
            for (int i = 0; i < 3; i++)
            {
                rawData.Nodes[i].Should().BeApproximately(args[i], MathPrecision);
                rawData.Values[i].Should().BeApproximately(args[i], MathPrecision);
            }
        }

        [Fact]
        public void ConstructionCubic()
        {
            RawData rawData = new RawData(1.0d, 6.0d, 6, true, FRawFunctions.Cubic);
            rawData.Function.Method.Name.Should().Be("Cubic");

            rawData.Left.Should().BeApproximately(1.0d, MathPrecision);
            rawData.Right.Should().BeApproximately(6.0d, MathPrecision);
            rawData.NodesCount.Should().Be(6);
            rawData.IsUniform.Should().BeTrue();

            rawData.Nodes.Should().NotBeNull();
            rawData.Nodes.Should().HaveCount(6);

            rawData.Values.Should().NotBeNull();
            rawData.Values.Should().HaveCount(6);

            double[] args = new double[6] { 1.0d, 2.0d, 3.0d, 4.0d, 5.0d, 6.0d };
            for (int i = 0; i < 6; i++)
            {
                rawData.Nodes[i].Should().BeApproximately(args[i], MathPrecision);
                rawData.Values[i].Should().BeApproximately(Math.Pow(args[i], 3), MathPrecision);
            }
        }

        [Fact]
        public void ConstructionRandomNonUniform()
        {
            RawData rawData = new RawData(1.0d, 6.0d, 2000, false, FRawFunctions.PseudoRandom);
            rawData.Function.Method.Name.Should().Be("PseudoRandom");

            rawData.Left.Should().BeApproximately(1.0d, MathPrecision);
            rawData.Right.Should().BeApproximately(6.0d, MathPrecision);
            rawData.NodesCount.Should().Be(2000);
            rawData.IsUniform.Should().BeFalse();

            rawData.Nodes.Should().NotBeNull();
            rawData.Nodes.Should().HaveCount(2000);

            rawData.Values.Should().NotBeNull();
            rawData.Values.Should().HaveCount(2000);

            rawData.Nodes.Should().BeInAscendingOrder();
        }

        [Fact]
        public void SaveLoadCubic()
        {
            string saveLoadFileName = "saveLoadFileName";

            RawData rawData = new RawData(1.0d, 2.0d, 5, true, FRawFunctions.Cubic);
            rawData.Save(saveLoadFileName);

            rawData = RawData.Load(saveLoadFileName);
            rawData.Should().NotBeNull();

            rawData.Left.Should().BeApproximately(1.0d, MathPrecision);
            rawData.Right.Should().BeApproximately(2.0d, MathPrecision);
            rawData.NodesCount.Should().Be(5);
            rawData.IsUniform.Should().BeTrue();

            rawData.Function.Method.Name.Should().Be("Cubic");

            rawData.Nodes.Should().NotBeNull();
            rawData.Nodes.Should().HaveCount(5);

            rawData.Values.Should().NotBeNull();
            rawData.Values.Should().HaveCount(5);

            double[] args = { 1.0d, 1.25d, 1.5d, 1.75d, 2.0d };
            for (int i = 0; i < 5; i++)
            {
                rawData.Nodes[i].Should().BeApproximately(args[i], MathPrecision);
                rawData.Values[i].Should().BeApproximately(Math.Pow(args[i], 3), MathPrecision);
            }
        }

        [Fact]
        public void SaveLoadRandom()
        {
            string saveLoadFileName = "saveLoadFileName";

            RawData rawData = new RawData(1.0d, 2.0d, 11, true, FRawFunctions.PseudoRandom);
            double[] nodes = new double[11];
            rawData.Nodes.CopyTo(nodes, 0);
            double[] values = new double[11];
            rawData.Values.CopyTo(values, 0);
            rawData.Save(saveLoadFileName);

            rawData = RawData.Load(saveLoadFileName);
            rawData.Should().NotBeNull();

            rawData.Left.Should().BeApproximately(1.0d, MathPrecision);
            rawData.Right.Should().BeApproximately(2.0d, MathPrecision);
            rawData.NodesCount.Should().Be(11);
            rawData.IsUniform.Should().BeTrue();

            rawData.Function.Method.Name.Should().Be("PseudoRandom");

            rawData.Nodes.Should().NotBeNull();
            rawData.Nodes.Should().HaveCount(11);

            rawData.Values.Should().NotBeNull();
            rawData.Values.Should().HaveCount(11);

            for (int i = 0; i < 11; i++)
            {
                rawData.Nodes[i].Should().BeApproximately(nodes[i], MathPrecision);
                rawData.Values[i].Should().BeApproximately(values[i], MathPrecision);
            }
        }

        [Fact]
        public void LoadNonExistingFile()
        {
            Action code = () => RawData.Load("nonExistingFileName");
            code.Should().Throw<Exception>();
        }

        [Fact]
        public void ConstructionFromFile()
        {
            string saveLoadFileName = "saveLoadFileName";

            RawData rawData = new RawData(1.0d, 2.0d, 5, true, FRawFunctions.Cubic);
            rawData.Save(saveLoadFileName);

            rawData = new RawData(saveLoadFileName);
            rawData.Should().NotBeNull();

            rawData.Left.Should().BeApproximately(1.0d, MathPrecision);
            rawData.Right.Should().BeApproximately(2.0d, MathPrecision);
            rawData.NodesCount.Should().Be(5);
            rawData.IsUniform.Should().BeTrue();

            rawData.Function.Method.Name.Should().Be("Cubic");

            rawData.Nodes.Should().NotBeNull();
            rawData.Nodes.Should().HaveCount(5);

            rawData.Values.Should().NotBeNull();
            rawData.Values.Should().HaveCount(5);

            double[] args = { 1.0d, 1.25d, 1.5d, 1.75d, 2.0d };
            for (int i = 0; i < 5; i++)
            {
                rawData.Nodes[i].Should().BeApproximately(args[i], MathPrecision);
                rawData.Values[i].Should().BeApproximately(Math.Pow(args[i], 3), MathPrecision);
            }

            Action code = () => { rawData = new RawData("nonExistingFileName"); };
            code.Should().Throw<Exception>();
        }
    }
}
