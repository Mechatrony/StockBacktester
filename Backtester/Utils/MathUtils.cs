using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backtester.Utils;

public static class MathUtils
{
    //internal const double DBL_EPSILON = 2.2204460492503131e-016;
    internal const double DBL_EPSILON = 1e-12;

    public static bool InRange<T>(this T value, T lowerValue, T upperValue)
        where T : IComparable, IComparable<T>
    {
        return value.CompareTo(lowerValue) >= 0 && value.CompareTo(upperValue) <= 0;
    }

    public static double CalculateOrderableAmount(double fund, double price)
    {
        return fund / price;
    }

    public static double Floor(double value, int precision = 0)
    {
        if (value < double.NegativeZero)
        {
            return Math.Round(value + 0.5d * Math.Pow(0.1d, precision), precision);
        }
        else
        {
            return Math.Round(value - 0.5d * Math.Pow(0.1d, precision), precision);
        }
    }

    public static double Ceil(double value, int precision = 0)
    {
        if (value < double.NegativeZero)
        {
            return Math.Round(value - 0.5d * Math.Pow(0.1d, precision), precision);
        }
        else
        {
            return Math.Round(value + 0.5d * Math.Pow(0.1d, precision), precision);
        }
    }

    public static bool IsCloseTo(this double value1, double value2)
    {
        //in case they are Infinities (then epsilon check does not work)
        if (value1 == value2)
        {
            return true;
        }

        // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
        var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
        var delta = value1 - value2;
        return (-eps < delta) && (eps > delta);
    }

    public static bool IsZero(this double value)
    {
        return value.IsCloseTo(0);
    }

    public static bool IsGreaterThan(this double value, double other)
    {
        return value > other && !value.IsCloseTo(other);
    }

    public static bool IsLessThan(this double value, double other)
    {
        return value < other && !value.IsCloseTo(other);
    }

    public static bool IsSameOrGreaterThan(this double value, double other)
    {
        return !value.IsLessThan(other);
    }

    public static bool IsSameOrLessThan(this double value, double other)
    {
        return !value.IsGreaterThan(other);
    }

    public static DateTime Min(DateTime dateTime1, DateTime dateTime2)
    {
        return dateTime1 < dateTime2 ? dateTime1 : dateTime2;
    }

    public static double StandardDeviation(this IEnumerable<double> values)
    {
        double result = 0;
        if (values.Any())
        {
            double average = values.Average();
            double sum = values.Sum(value => Math.Pow(value - average, 2));
            result = Math.Sqrt(sum / values.Count());
        }
        return result;
    }

    public static int PyramidSum(int startValue, int endValue)
    {
        int sum = 0;
        for (int i = startValue; i <= endValue; i++)
        {
            sum += i;
        }
        return sum;
    }

    public static double CAGR(int days, double ror)
    {
        // ror: 0.xxx
        return Math.Pow(1d + ror, 365d / days) - 1;
    }

    public static double AverageIfValid(this IEnumerable<double> values)
    {
        return values.Any() ? values.Average() : double.NaN;
    }
}