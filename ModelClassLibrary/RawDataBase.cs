using System.Text.Json;

namespace ModelClassLibrary
{
    public class RawDataBase
    {
        public double Left { get; protected set; }
        public double Right { get; protected set; }
        public int NodesCount { get; protected set; }
        public bool IsUniform { get; protected set; }
        public double Step
        {
            get
            {
                return (Right - Left) / (NodesCount - 1);
            }
        }

        public RawDataBase() { }

        public RawDataBase(double left, double right, int nodesCount, bool isUniform)
        {
            Left = left;
            Right = right;
            NodesCount = nodesCount;
            IsUniform = isUniform;
        }

        public string Serialize(JsonSerializerOptions? options = null)
        {
            if (options == null)
            {
                options = new JsonSerializerOptions()
                {
                    IncludeFields = true,
                    WriteIndented = true
                };
            }
            string result = "";
            result += JsonSerializer.Serialize(Left, options) + "\n";
            result += JsonSerializer.Serialize(Right, options) + "\n";
            result += JsonSerializer.Serialize(NodesCount, options) + "\n";
            result += JsonSerializer.Serialize(IsUniform, options);
            return result;
        }
    }
}
