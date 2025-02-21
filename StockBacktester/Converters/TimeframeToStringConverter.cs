using Backtester.Crypto;
using Backtester.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Backtester.Converters;

public class TimeframeToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Timeframe timeframe)
        {
            return timeframe.ToStringExpression();
        }
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string timeframeString)
        {
            switch (timeframeString)
            {
                case "1s":
                    return Timeframe.OneSecond;
                case "1m":
                    return Timeframe.OneMinute;
                case "3m":
                    return Timeframe.ThreeMinutes;
                case "5m":
                    return Timeframe.FiveMinutes;
                case "15m":
                    return Timeframe.FifteenMinutes;
                case "30m":
                    return Timeframe.ThirtyMinutes;
                case "1h":
                    return Timeframe.OneHour;
                case "2h":
                    return Timeframe.TwoHours;
                case "4h":
                    return Timeframe.FourHours;
                case "6h":
                    return Timeframe.SixHours;
                case "8h":
                    return Timeframe.EightHours;
                case "12h":
                    return Timeframe.TwelveHours;
                case "1d":
                    return Timeframe.OneDay;
                case "1w":
                    return Timeframe.OneWeek;
                case "1M":
                    return Timeframe.OneMonth;
            }
        }

        return null;
    }
}
