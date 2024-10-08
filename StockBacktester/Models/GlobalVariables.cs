using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBacktester.Models;

public static class GlobalVariables {
  public static string KeyFilePath { get; } = "C:\\LicenseKeys";
  public static string DbRootPath { get; set; } = "D:\\DB";
}
