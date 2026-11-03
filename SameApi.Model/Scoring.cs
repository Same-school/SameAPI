namespace SameApi.Model
{
    public static class Scoring
    {
        private const double Z = 1.96;

        public static double WilsonLowerBound(long up, long down)
        {
            long n = up + down;

            if (n == 0) return 0;

            double p = up / n;
            double z2 = Z * Z;

            double squareRoot = Math.Sqrt((p * (1 - p) / n + z2 / (4 * (n * n))));

            var numerator = p + z2/(2*n) - Z * squareRoot;
            var denominator = 1 + z2 / n;

            var result  = numerator / denominator;
            
            return result;
        }

        public static double Decay(DateTime createdAtUtc, DateTime nowUtc)
        {
            double ageHours = Math.Max(0, (nowUtc - createdAtUtc).TotalHours);
            var formula = 1 / (Math.Pow(ageHours + 2, 1.5));

            return 1 / formula;
        }
    }
}