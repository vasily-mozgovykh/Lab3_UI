using System.Runtime.InteropServices;

namespace ModelClassLibrary
{
    public class SplineData
    {
        public RawData RawData { get; private set; }
        public double[] BoundaryConditions { get; private set; }
        public int SplineNodesCount { get; private set; }
        public List<SplineDataItem> Items { get; private set; }
        public double Integral { get; private set; }
        public double SplineStep
        {
            get
            {
                return (RawData.Right - RawData.Left) / (SplineNodesCount - 1);
            }
        }

        public SplineData(RawData rawData, double[] boundaryConditions, int splineNodesCount)
        {
            RawData = rawData;
            BoundaryConditions = new double[2] { boundaryConditions[0], boundaryConditions[1] };
            SplineNodesCount = splineNodesCount;
            Items = new List<SplineDataItem>();
        }

        private const string SplinesCalculatorDllPath = "C:\\Users\\Vasily\\source\\repos\\Lab3_UI\\SplinesCalculator.dll";

        [DllImport(SplinesCalculatorDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void calculate_spline(
            double[] nodes,
            int nodes_count,
            bool is_uniform,
            double[] values,
            double[] boundary_conditions,
            int nsite,
            double[] site,
            double left_integration_limit,
            double right_integration_limit,
            double[] spline_values,
            ref double integral,
            ref int error
        );

        public void CalculateSpline()
        {
            double integral = 0.0d;
            int error = 0;
            double[] splineValues = new double[3 * SplineNodesCount];
            calculate_spline(
                RawData.IsUniform ? new double[2] { RawData.Left, RawData.Right } : RawData.Nodes,
                RawData.NodesCount,
                RawData.IsUniform,
                RawData.Values,
                BoundaryConditions,
                SplineNodesCount,
                new double[2] { RawData.Left, RawData.Right },
                RawData.Left,
                RawData.Right,
                splineValues,
                ref integral,
                ref error
            );
            if (error != 0)
            {
                throw new Exception($"SplineData.CalculateSpline: dll returned error code: [{error}]");
            }
            Integral = integral;
            Items.Clear();
            for (int i = 0; i < SplineNodesCount; i++)
            {
                double x = RawData.Left + SplineStep * i;
                Items.Add(new SplineDataItem(x, splineValues[3 * i], splineValues[3 * i + 1], splineValues[3 * i + 2]));
            }
        }
    }
}
