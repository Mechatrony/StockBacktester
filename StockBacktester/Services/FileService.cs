using Newtonsoft.Json;
using StockBacktester.Crypto;
using System.Data;
using System.Text;

namespace StockBacktester.Services;

public class FileService {
  public T? ReadJson<T>(string folderPath, string fileName) {
    var path = Path.Combine(folderPath, fileName);
    if (File.Exists(path)) {
      var json = File.ReadAllText(path);
      return JsonConvert.DeserializeObject<T>(json);
    }

    return default;
  }

  public void SaveJson<T>(string folderPath, string fileName, T content) {
    if (!Directory.Exists(folderPath)) {
      Directory.CreateDirectory(folderPath);
    }

    var fileContent = JsonConvert.SerializeObject(content);
    File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
  }

  public void Delete(string folderPath, string fileName) {
    if (fileName != null && File.Exists(Path.Combine(folderPath, fileName))) {
      File.Delete(Path.Combine(folderPath, fileName));
    }
  }

  public void SaveCsv(string fileName, DataTable dataTable) {
    StringBuilder sb = new StringBuilder();

    foreach (DataColumn column in dataTable.Columns) {
      sb.Append(column.ColumnName);
      sb.Append(',');
    }
    sb.AppendLine();
    foreach (DataRow row in dataTable.Rows) {
      sb.AppendLine(string.Join(',', row.ItemArray));
    }

    File.WriteAllText(fileName, sb.ToString());
  }

  public List<Ohlcv> GetOhlcvs(string fileFullName) {
    List<Ohlcv> ohlcvs = new();
    string[] lines = File.ReadAllLines(fileFullName);

    int dateTimeColumn = 0;
    int openColumn = 0;
    int highColumn = 0;
    int lowColumn = 0;
    int closeColumn = 0;
    int volumeColumn = 0;
    int isFirstColumn = 0;

    // Match columns
    string[] firstRowItems = lines.First().Split(',');
    for (int column = 0; column < firstRowItems.Length; ++column) {
      string item = firstRowItems[column];
      switch (item) {
        case "DateTime":
          dateTimeColumn = column;
          break;
        case "Open":
          openColumn = column;
          break;
        case "High":
          highColumn = column;
          break;
        case "Low":
          lowColumn = column;
          break;
        case "Close":
          closeColumn = column;
          break;
        case "Volume":
          volumeColumn = column;
          break;
        case "IsFirst":
          isFirstColumn = column;
          break;
      }
    }

    // Convert to Ohlcv
    for (int rowIndex = 1; rowIndex < lines.Length; ++rowIndex) {
      string line = lines[rowIndex];
      string[] items = line.Split(',');
      DateTime dateTime = DateTime.Parse(items[dateTimeColumn]);

      Ohlcv ohlcv = new Ohlcv(
        dateTime,
        double.Parse(items[openColumn]),
        double.Parse(items[highColumn]),
        double.Parse(items[lowColumn]),
        double.Parse(items[closeColumn]),
        double.Parse(items[volumeColumn])
      );
      ohlcvs.Add(ohlcv);
    }

    if (ohlcvs.Count > 0) {
      ohlcvs.First().IsFirst = lines[1].Split(',')[isFirstColumn] == "True";
    }

    return ohlcvs;
  }

  public void SaveOhlcvs(string folderPath, string fileName, List<Ohlcv> ohlcvs) {
    StringBuilder sb = new StringBuilder();

    sb.Append("DateTime,Open,High,Low,Close,Volume,IsFirst");
    sb.AppendLine();
    foreach (var ohlcvPair in ohlcvs) {
      sb.AppendLine(string.Join(',', [
        ohlcvPair.DateTime.ToString("yyyy-MM-dd HH:mm"),
        ohlcvPair.OpenPrice.ToString("0.##########"),
        ohlcvPair.HighPrice.ToString("0.##########"),
        ohlcvPair.LowPrice.ToString("0.##########"),
        ohlcvPair.ClosePrice.ToString("0.##########"),
        ohlcvPair.Volume.ToString("0.##########"),
        ohlcvPair.IsFirst.ToString()
      ]));
    }

    Directory.CreateDirectory(folderPath);
    File.WriteAllText(Path.Combine(folderPath, fileName), sb.ToString());
  }
}
