using ModelClassLibrary;
using FluentAssertions;

namespace ClassLibraryTests
{
    public class FRawTests
    {
        private const double MathPrecision = 1e-9d;

        [Fact]
        public void LinearFunction()
        {
            FRawFunctions.Linear(0.0d).Should().BeApproximately(0.0d, MathPrecision);
            FRawFunctions.Linear(1.0d).Should().BeApproximately(1.0d, MathPrecision);
            FRawFunctions.Linear(0.2d).Should().BeApproximately(0.2d, MathPrecision);
            FRawFunctions.Linear(-2.5d).Should().BeApproximately(-2.5d, MathPrecision);
            FRawFunctions.Linear(30.0d).Should().BeApproximately(30.0d, MathPrecision);
        }

        [Fact]
        public void CubicFunction()
        {
            FRawFunctions.Cubic(0.0d).Should().BeApproximately(0.0d, MathPrecision);
            FRawFunctions.Cubic(1.0d).Should().BeApproximately(1.0d, MathPrecision);
            FRawFunctions.Cubic(0.2d).Should().BeApproximately(0.008d, MathPrecision);
            FRawFunctions.Cubic(-2.5d).Should().BeApproximately(-15.625d, MathPrecision);
            FRawFunctions.Cubic(30.0d).Should().BeApproximately(27000.0d, MathPrecision);
        }

        [Fact]
        public void PseudoRandomFunction()
        {
            List<double> args = new List<double> { 0.0d, 1.0d, 0.2d, -2.5d, 30.0d };
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                args.Add(Convert.ToDouble(i) + random.NextDouble());
            }
            double min = args.Min(x => FRawFunctions.PseudoRandom(x));
            double max = args.Max(x => FRawFunctions.PseudoRandom(x));
            min.Should().BeGreaterThanOrEqualTo(0.0d);
            max.Should().BeLessThanOrEqualTo(1.0d);
        }

        [Fact]
        public void StringToFunction()
        {
            string[] existingFunctions =
            {
                "Linear",
                "Cubic",
                "PseudoRandom"
            };
            string nonExistingFunction = "NonExistingFunction";

            FRaw function;
            string functionName;
            foreach (string existingFunction in  existingFunctions)
            {
                function = FRawFunctions.ToFunction(existingFunction);
                functionName = existingFunction;
                functionName.Should().Be(existingFunction);
            }

            Action code = () => FRawFunctions.ToFunction(nonExistingFunction);
            code.Should().Throw<Exception>();
        }

        [Fact]
        public void EnumToFunction()
        {
            (string, FRawEnum) [] existingFunctions =
            {
                ("Linear", FRawEnum.Linear),
                ("Cubic", FRawEnum.Cubic),
                ("PseudoRandom", FRawEnum.PseudoRandom)
            };

            FRaw function;
            string functionName;
            foreach ((string, FRawEnum) existingFunction in existingFunctions)
            {
                function = FRawFunctions.ToFunction(existingFunction.Item2);
                functionName = function.Method.Name;
                functionName.Should().Be(existingFunction.Item1);
            }

            Action code = () => FRawFunctions.ToFunction((FRawEnum) (-1));
            code.Should().Throw<Exception>();
        }

        [Fact]
        public void StringToEnum()
        {
            (string, FRawEnum)[] existingFunctions =
            {
                ("Linear", FRawEnum.Linear),
                ("Cubic", FRawEnum.Cubic),
                ("PseudoRandom", FRawEnum.PseudoRandom)
            };
            string nonExistingFunction = "NonExistingFunction";

            FRawEnum fRawEnum;
            foreach ((string, FRawEnum) existingFunction in existingFunctions)
            {
                fRawEnum = FRawFunctions.ToEnum(existingFunction.Item1);
                fRawEnum.Should().Be(existingFunction.Item2);
            }

            Action code = () => FRawFunctions.ToEnum(nonExistingFunction);
            code.Should().Throw<Exception>();
        }

        [Fact]
        public void FunctionToEnum()
        {
            (FRaw, FRawEnum)[] existingFunctions =
            {
                (FRawFunctions.Linear, FRawEnum.Linear),
                (FRawFunctions.Cubic, FRawEnum.Cubic),
                (FRawFunctions.PseudoRandom, FRawEnum.PseudoRandom)
            };

            FRawEnum fRawEnum;
            foreach ((FRaw, FRawEnum) existingFunction in existingFunctions)
            {
                fRawEnum = FRawFunctions.ToEnum(existingFunction.Item1);
                fRawEnum.Should().Be(existingFunction.Item2);
            }

            double nonExistingFunction(double x) { return x; }

            Action code = () => FRawFunctions.ToEnum(nonExistingFunction);
            code.Should().Throw<Exception>();
        }
    }
}