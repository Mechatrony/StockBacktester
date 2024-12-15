using StockBacktester.Crypto;
using StockBacktester.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace StockBacktester.Converters;

internal class TimeframeArrayToStringArrayConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string[]? result = null;
        if (value is IEnumerable<Timeframe> timeframes)
        {
            result = new string[timeframes.Count()];
            for (int index = 0; index < timeframes.Count(); ++index)
            {
                Timeframe timeframe = timeframes.ElementAt(index);
                result[index] = timeframe.ToStringExpression();
            }
        }

        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Timeframe[] result = null;
        if (value is IEnumerable<string> timeframeStrings)
        {
            result = new Timeframe[timeframeStrings.Count()];
            for (int index = 0; index < timeframeStrings.Count(); ++index)
            {
                string timeframeString = timeframeStrings.ElementAt(index);
                switch (timeframeString)
                {
                    case "1s":
                        result[index] = Timeframe.OneSecond;
                        break;
                    case "1m":
                        result[index] = Timeframe.OneMinute;
                        break;
                    case "3m":
                        result[index] = Timeframe.ThreeMinutes;
                        break;
                    case "5m":
                        result[index] = Timeframe.FiveMinutes;
                        break;
                    case "15m":
                        result[index] = Timeframe.FifteenMinutes;
                        break;
                    case "30m":
                        result[index] = Timeframe.ThirtyMinutes;
                        break;
                    case "1h":
                        result[index] = Timeframe.OneHour;
                        break;
                    case "2h":
                        result[index] = Timeframe.TwoHours;
                        break;
                    case "4h":
                        result[index] = Timeframe.FourHours;
                        break;
                    case "6h":
                        result[index] = Timeframe.SixHours;
                        break;
                    case "8h":
                        result[index] = Timeframe.EightHours;
                        break;
                    case "12h":
                        result[index] = Timeframe.TwelveHours;
                        break;
                    case "1d":
                        result[index] = Timeframe.OneDay;
                        break;
                    case "1w":
                        result[index] = Timeframe.OneWeek;
                        break;
                    case "1M":
                        result[index] = Timeframe.OneMonth;
                        break;
                }
            }
        }

        return result;
    }
}
