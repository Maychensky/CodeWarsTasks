using System.Numerics;

namespace CodeWarsTasks
{
    internal class Kata001
    {
        private enum Sing { Plus, Minus, }
        public static string MixedFraction(string s)
        {
            
            var valuesFraction = GetValuesFraction(s);
            var integerPart = GetIntegerValue(valuesFraction);
            var properFraction = GetProperFraction(valuesFraction);
            LoadToIrreducibleProperFraction(ref properFraction);
            return GetStringFormat(valuesFraction.Item1 ,integerPart, properFraction.Item1, properFraction.Item2);
        }

        private static string GetStringFormat(Sing sing, BigInteger integerPart, BigInteger numerator, BigInteger denominator)
        {
            var space = string.Empty;
            var result = string.Empty;
            if (sing == Sing.Minus) result += "-";
            if (integerPart != 0)
            {
                result += integerPart;
                space = " ";
            }
            if (numerator != 0) result += string.Format("{0}{1}/{2}", space, numerator, denominator);
            return result.Equals(string.Empty) || result.Equals("-") ? "0" : result;
        }

        private static void LoadToIrreducibleProperFraction(ref (BigInteger, BigInteger) properFraction)
        {
            var commonDivision = GetCommonDivision(properFraction.Item1, properFraction.Item2);
            properFraction = (properFraction.Item1 / commonDivision, properFraction.Item2 / commonDivision);
        }

        private static BigInteger GetCommonDivision(BigInteger value1, BigInteger value2)
        {
            return value2 == 0 ? value1 : GetCommonDivision(value2, value1 % value2);
        }

        private static (BigInteger, BigInteger) GetProperFraction((Sing, BigInteger, BigInteger) valuesFraction)
        {
            var integerPart = GetIntegerValue(valuesFraction);
            return (valuesFraction.Item2 - (valuesFraction.Item3 * integerPart), valuesFraction.Item3);
        }

        private static BigInteger GetIntegerValue((Sing, BigInteger, BigInteger) valuesFraction)
        {
            return valuesFraction.Item2 / valuesFraction.Item3;
        }

        private static (Sing, BigInteger, BigInteger) GetValuesFraction(string s)
        {
            (Sing, BigInteger, BigInteger) result = (Sing.Plus,0, 0);
            var valuesFraction = s.Split('/');
            result.Item2 = int.Parse(valuesFraction[0]);
            result.Item3 = int.Parse(valuesFraction[1]);
            if ((result.Item2.IsNegative() && !result.Item3.IsNegative() || (!result.Item2.IsNegative() && result.Item3.IsNegative()))) result.Item1 = Sing.Minus;
            if (result.Item2 < 0) result.Item2 *= -1;
            if (result.Item3 < 0) result.Item3 *= -1;
            return result;
        }
    }

    public static class ExtendedInteger
    {
        public static bool IsNegative(this BigInteger value) => (value < 0);
    }
}