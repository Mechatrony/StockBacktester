using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;

namespace Kis;

public partial class KisClient {
  [AllowNull]
  private static Dictionary<string, KospiCode> kospiCodes;

  [AllowNull]
  private static Dictionary<string, KosdaqCode> kosdaqCodes;

  public static bool CodeExists(string code)
    => kospiCodes.ContainsKey(code) || kosdaqCodes.ContainsKey(code);

  // 코스닥주식종목코드(kosdaq_code.mst) 정제 파이썬 파일
  // https://github.com/koreainvestment/open-trading-api/blob/main/stocks_info/kis_kosdaq_code_mst.py
  public async Task<Dictionary<string, KosdaqCode>> LoadKosdaqMasterCode(string baseDirectory) {
    Dictionary<string, KosdaqCode> codes = new();

    string codeFileName = "kosdaq_code.mst";
    if (await CacheDownloadAsync(baseDirectory, codeFileName, TimeSpan.FromHours(23)) == false) {
      return codes;
    }

    // "%TEMP%\eFriendOpenAPI\kosdaq_code.mst"
    string mstFile = Path.Combine(baseDirectory, codeFileName);

    foreach (string line in File.ReadAllLines(mstFile, Encoding.GetEncoding("ks_c_5601-1987"))) {
      var item = KosdaqCode.ReadFromMSTFile(line);
      codes[item.단축코드] = item;
    }

    kosdaqCodes = codes;
    return codes;
  }

  // 코스피주식종목코드(kospi_code.mst) 정제 파이썬 파일
  // https://github.com/koreainvestment/open-trading-api/blob/main/stocks_info/kis_kospi_code_mst.py
  public async Task<Dictionary<string, KospiCode>> LoadKospiMasterCode(string baseDirectory) {
    Dictionary<string, KospiCode> codes = new();

    string codeFileName = "kospi_code.mst";
    if (await CacheDownloadAsync(baseDirectory, codeFileName, TimeSpan.FromHours(23)) == false) {
      return codes;
    }

    // "%TEMP%\eFriendOpenAPI\kospi_code.mst"
    string mstFile = Path.Combine(baseDirectory, codeFileName);

    foreach (string line in File.ReadAllLines(mstFile, Encoding.GetEncoding("ks_c_5601-1987"))) {
      var item = KospiCode.ReadFromMstFile(line);
      codes[item.단축코드] = item;
    }

    kospiCodes = codes;
    return codes;
  }

  private async Task<bool> CacheDownloadAsync(string baseDirectory, string codeFileName, TimeSpan expire) {
    string codeFilePath = Path.Combine(baseDirectory, codeFileName);
    if (File.Exists(codeFilePath)) {
      FileInfo fileInfo = new FileInfo(codeFilePath);
      if (fileInfo.CreationTimeUtc.Add(expire) < DateTime.UtcNow) {
        File.Delete(codeFilePath);
      } else {
        return true;
      }
    }

    HttpClient httpClient = new HttpClient();

    CancellationTokenSource cts = new CancellationTokenSource();
    cts.CancelAfter(TimeSpan.FromSeconds(10));

    string masterFile = $"https://new.real.download.dws.co.kr/common/master/{codeFileName}.zip";
    Stream? stream = await httpClient.GetStreamAsync(masterFile, cts.Token);

    if (stream == null) {
      return false;
    }

    using (stream)
    using (ZipArchive zipArchive = new ZipArchive(stream)) {
      zipArchive.ExtractToDirectory(baseDirectory);
    }

    return true;
  }
}
