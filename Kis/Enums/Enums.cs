using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kis.Enums;

public enum Timeframe {
  //[Description("1m")]OneMinute,
  //[Description("3m")]ThreeMinutes,
  //[Description("5m")]FiveMinutes,
  //[Description("15m")]FifteenMinutes,
  //[Description("30m")]ThirtyMinutes,
  //[Description("1h")]OneHour,
  //[Description("2h")]TwoHours,
  //[Description("4h")]FourHours,
  //[Description("6h")]SixHours,
  //[Description("8h")]EightHours,
  //[Description("12h")]TwelveHours,
  [Description("D")]OneDay,
  [Description("W")]OneWeek,
  [Description("M")]OneMonth,
  [Description("Y")]OneYear,
}