using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBacktester.Utils;

public static class TechnicalIndicators
{
    public static Dictionary<DateTime, double> SMA(Dictionary<DateTime, double> source, int length)
    {

        Dictionary<DateTime, double> sma = new(source.Count);
        double sum = 0d;

        for (int index = 0; index < source.Count; ++index)
        {
            var kvp = source.ElementAt(index);
            DateTime dateTime = kvp.Key;

            sum += kvp.Value;
            if (index >= length)
            {
                sum -= source.ElementAt(index - length).Value;
            }

            if (index >= length)
            {
                sma[dateTime] = sum / length;
            }
            else
            {
                sma[dateTime] = double.NaN;
            }
        }

        return sma;
    }

    public static Dictionary<DateTime, double> WMA(Dictionary<DateTime, double> source, int length)
    {

        Dictionary<DateTime, double> wma = new(source.Count);
        double sum = 0d;
        int devider = MathUtils.PyramidSum(1, length);
        int firstValidIndex = -1;

        for (int index = 0; index < source.Count; ++index)
        {
            var kvp = source.ElementAt(index);
            if (double.IsNaN(kvp.Value))
            {
                wma[kvp.Key] = double.NaN;
                continue;
            }

            if (firstValidIndex == -1)
            {
                firstValidIndex = index;
            }
            sum += kvp.Value * length;

            for (int subIndex = index - 1; subIndex >= Math.Max(index - length, firstValidIndex); --subIndex)
            {
                sum -= source.ElementAtOrDefault(subIndex).Value.NaNToZero();
            }

            if (index >= firstValidIndex + length)
            {
                wma[kvp.Key] = sum / devider;
            }
            else
            {
                wma[kvp.Key] = double.NaN;
            }
        }

        return wma;
    }

    public static Dictionary<DateTime, double> HMA(Dictionary<DateTime, double> source, int length)
    {

        Dictionary<DateTime, double> wma1 = WMA(source, length / 2);
        Dictionary<DateTime, double> wma2 = WMA(source, length);
        Dictionary<DateTime, double> rawHma = new Dictionary<DateTime, double>(source.Count);

        foreach (var kvp in wma1)
        {
            if (kvp.Value == double.NaN)
            {
                rawHma[kvp.Key] = double.NaN;
            }
            else
            {
                rawHma[kvp.Key] = 2 * kvp.Value - wma2[kvp.Key];
            }
        }

        return WMA(rawHma, (int)Math.Sqrt(length));
    }

    public static bool Crossover(
        Dictionary<DateTime, double> series1,
        Dictionary<DateTime, double> series2,
        DateTime indexTime, DateTime prevIndexTime)
    {
        if (!series1.TryGetValue(indexTime, out double cur1)) return false;
        if (!series2.TryGetValue(indexTime, out double cur2)) return false;
        if (!series1.TryGetValue(prevIndexTime, out double prev1)) return false;
        if (!series2.TryGetValue(prevIndexTime, out double prev2)) return false;
        return cur1.IsGreaterThan(cur2) && prev1.IsSameOrLessThan(prev2);
    }

    public static bool Crossunder(
        Dictionary<DateTime, double> series1,
        Dictionary<DateTime, double> series2,
        DateTime indexTime, DateTime prevIndexTime)
    {
        if (!series1.TryGetValue(indexTime, out double cur1)) return false;
        if (!series2.TryGetValue(indexTime, out double cur2)) return false;
        if (!series1.TryGetValue(prevIndexTime, out double prev1)) return false;
        if (!series2.TryGetValue(prevIndexTime, out double prev2)) return false;
        return cur1.IsLessThan(cur2) && prev1.IsSameOrGreaterThan(prev2);
    }
}