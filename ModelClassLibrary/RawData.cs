using System.Text.Json;

namespace ModelClassLibrary
{
    public class RawData : RawDataBase
    {
        public FRaw Function { get; private set; }
        public double[] Nodes { get; private set; }
        public double[] Values { get; private set; }

        public RawData(double left, double right, int nodesCount, bool isUniform, FRaw function) : base(left, right, nodesCount, isUniform)
        {
            Function = function;
            Nodes = new double[nodesCount];
            Values = new double[nodesCount];
            Random random = new Random();
            for (int i = 0; i < nodesCount; i++)
            {
                Nodes[i] = Left + Step * i;
                if (isUniform == false && i != 0 && i != nodesCount - 1)
                {
                    Nodes[i] += (random.NextDouble() - 0.5d) * Step;
                }
                Values[i] = function(Nodes[i]);
            }
        }

        public RawData(string filename)
        {
            RawData rawData = Load(filename);
            Left = rawData.Left;
            Right = rawData.Right;
            NodesCount = rawData.NodesCount;
            IsUniform = rawData.IsUniform;
            
            Function = rawData.Function;

            Nodes = new double[NodesCount];
            Values = new double[NodesCount];
            
            rawData.Nodes.CopyTo(Nodes, 0);
            rawData.Values.CopyTo(Values, 0);
        }

        public void Save(string filename)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                IncludeFields = true,
                WriteIndented = true
            };

            StreamWriter? writer = null;
            try
            {
                writer = new StreamWriter(filename);
                writer.WriteLine(base.Serialize(options));
                double[] nodesValues = new double[2 * NodesCount];
                Nodes.CopyTo(nodesValues, 0);
                Values.CopyTo(nodesValues, NodesCount);
                writer.WriteLine(Function.Method.Name);
                writer.WriteLine(JsonSerializer.Serialize(nodesValues, options));
            }
            catch (Exception e)
            {
                throw new Exception($"RawData.Save: caught an exception: \"{e.Message}\"");
            }
            finally
            {
                writer?.Close();
            }
        }

        public static RawData Load(string filename)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                IncludeFields = true,
                WriteIndented = true
            };

            double left, right;
            int nodesCount;
            bool isUniform;
            FRaw? function = null;
            StreamReader? reader = null;
            double[]? nodesValues = null;
            try
            {
                reader = new StreamReader(filename);
                string? jsonLeft = reader.ReadLine();
                string? jsonRight = reader.ReadLine();
                string? jsonNodesCount = reader.ReadLine();
                string? jsonIsUniform = reader.ReadLine();
                string? jsonFunctionName = reader.ReadLine();
                string? jsonNodesValue = reader.ReadToEnd();
                List<string?> jsonArgs = new List<string?>() { jsonLeft, jsonRight, jsonNodesCount, jsonIsUniform, jsonFunctionName };
                if (jsonArgs.Any(s => s == null))
                {
                    throw new Exception($"Incorrect input in file \"{filename}\"");
                }
                left = JsonSerializer.Deserialize<double>(jsonLeft, options);
                right = JsonSerializer.Deserialize<double>(jsonRight, options);
                nodesCount = JsonSerializer.Deserialize<int>(jsonNodesCount, options);
                isUniform = JsonSerializer.Deserialize<bool>(jsonIsUniform, options);
                function = FRawFunctions.ToFunction(jsonFunctionName);
                nodesValues = JsonSerializer.Deserialize<double[]>(jsonNodesValue, options);
                if (nodesValues == null || nodesValues.Length != 2 * nodesCount)
                {
                    throw new Exception($"Incorrect input in file \"{filename}\"");
                }
            }
            catch(Exception e)
            {
                throw new Exception($"RawData.Load: caught an exception: \"{e.Message}\"");
            }
            finally
            {
                reader?.Close();
            }
            RawData rawData = new RawData(left, right, nodesCount, isUniform, function);
            nodesValues.Take(Range.EndAt(nodesCount)).ToArray().CopyTo(rawData.Nodes, 0);
            nodesValues.Take(Range.StartAt(nodesCount)).ToArray().CopyTo(rawData.Values, 0);
            return rawData;
        }
    }
}
