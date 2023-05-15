namespace ModelClassLibrary
{
    public class SplineDataItem
    {
        public double X { get; }
        public double Value { get; }
        public double FirstDerivative { get; }
        public double SecondDerivative { get; }

        public SplineDataItem(double x, double value, double firstDerivative, double secondDerivative)
        {
            X = x;
            Value = value;
            FirstDerivative = firstDerivative;
            SecondDerivative = secondDerivative;
        }

        public string ToString(string format)
        {
            return $"<{X.ToString(format)}; {Value.ToString(format)}; {FirstDerivative.ToString(format)}; {SecondDerivative.ToString(format)}>";
        }

        public override string ToString()
        {
            return ToString("0.000");
        }
    }
}
