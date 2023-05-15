namespace ModelClassLibrary
{
    public delegate double FRaw(double x);

    public enum FRawEnum
    {
        Linear,
        Cubic,
        PseudoRandom
    }

    public static class FRawFunctions
    {
        public static double Linear(double x)
        {
            return x;
        }

        public static double Cubic(double x)
        {
            return Math.Pow(x, 3);
        }

        public static double PseudoRandom(double x)
        {
            Random random = new Random();
            return random.NextDouble();
        }

        public static FRaw ToFunction(string str)
        {
            switch (str)
            {
                case "Linear":
                    return Linear;
                case "Cubic":
                    return Cubic;
                case "PseudoRandom":
                    return PseudoRandom;
                default: throw new Exception($"FrawFunctions.FRawFromString: there is no FRaw named \"{str}\"");
            }
        }

        public static FRaw ToFunction(FRawEnum fRawEnum)
        {
            switch (fRawEnum)
            {
                case FRawEnum.Linear:
                    return Linear;
                case FRawEnum.Cubic:
                    return Cubic;
                case FRawEnum.PseudoRandom:
                    return PseudoRandom;
                default: throw new Exception($"FrawFunctions.FRawFromString: there is no matching FRaw");
            }
        }

        public static FRawEnum ToEnum(string str)
        {
            switch (str)
            {
                case "Linear":
                    return FRawEnum.Linear;
                case "Cubic":
                    return FRawEnum.Cubic;
                case "PseudoRandom":
                    return FRawEnum.PseudoRandom;
                default: throw new Exception($"FrawFunctions.FRawFromString: there is no FRawEnum named \"{str}\"");
            }
        }

        public static FRawEnum ToEnum(FRaw function)
        {
            return ToEnum(function.Method.Name);
        }
    }
}