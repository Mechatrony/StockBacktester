using Cysharp.Web;
using Kis.Dto;
using Kis.Enums;
using Kis.Utils;
using Kis.Packet;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Runtime.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kis;

public partial class KisClient {
  private static byte[] entropy = { 0xfd, 0xed, 0x10, 0x20, 0xe9, 0x75 };
  private static HttpClientHandler sharedHandler = new HttpClientHandler() { MaxConnectionsPerServer = 1 };
  private string baseUrl;
  private TokenPRequest tokenPRequest;
  private AccessToken? accessToken;

  private bool isVts = false;
  private bool isLegalPerson = false;
  private string legalPersonSecretKey;
  private string macAddress = "";
  private string legalPhoneNumber = "";
  private string legalPublicIP = "";
  private string hashKey = ""; // Not supported yet.
  private string legalUID = "";

  public Account Account { get; private set; }
  public bool IsDebugMode { get; set; } = false;

  public KisClient(bool isVts, string appKey, string secretKey, string account, bool isLegalPerson = false,
    string legalPersonSecretKey = "", string macAddress = "", string legalPhoneNumber = "",
    string legalPublicIP = "", string legalUID = "") {
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    this.isVts = isVts;
    this.isLegalPerson = isLegalPerson;
    this.legalPersonSecretKey = legalPersonSecretKey;
    this.macAddress = macAddress;
    this.legalPhoneNumber = legalPhoneNumber;
    this.legalPublicIP = legalPublicIP;
    this.legalUID = legalUID;

    baseUrl = this.isVts
      ? "https://openapivts.koreainvestment.com:29443"
      : "https://openapi.koreainvestment.com:9443";

    tokenPRequest = new TokenPRequest() {
      AppKey = appKey,
      SecretKey = secretKey
    };

    string planeAccount = account.Replace(" ", "").Replace("-", "");
    Account = new Account(planeAccount);
  }

  public HttpClient CreateHttpClient(string trId = "", string tr_cont = "") {
    HttpClient client = IsDebugMode
      ? new HttpClient(new LoggingHandler(sharedHandler), false)
      : new HttpClient(sharedHandler, false);

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    if (string.IsNullOrEmpty(trId) == false) {
      client.DefaultRequestHeaders.Add("authorization", $"{accessToken?.Type} {accessToken?.Value}");
      client.DefaultRequestHeaders.Add("appkey", tokenPRequest.AppKey);
      client.DefaultRequestHeaders.Add("appsecret", tokenPRequest.SecretKey);

      client.DefaultRequestHeaders.Add("tr_id", trId);

      if (string.IsNullOrEmpty(tr_cont) == false) {
        client.DefaultRequestHeaders.Add("tr_cont", "");
      }

      client.DefaultRequestHeaders.Add("custtype", isLegalPerson ? "B" : "P");

      if (string.IsNullOrEmpty(macAddress) == false) {
        client.DefaultRequestHeaders.Add("mac_addr", macAddress);
      }

      if (string.IsNullOrEmpty(hashKey) == false) {
        client.DefaultRequestHeaders.Add("hashkey", hashKey);
      }

      if (isLegalPerson) {
        client.DefaultRequestHeaders.Add("personalseckey", legalPersonSecretKey);
        client.DefaultRequestHeaders.Add("seq_no", "01");
        client.DefaultRequestHeaders.Add("phone_number", legalPhoneNumber);
        client.DefaultRequestHeaders.Add("ip_addr", legalPublicIP);
        client.DefaultRequestHeaders.Add("gt_uid", legalUID);
      }
    }

    return client;
  }

  public async Task<주식현재가시세DTO?> 주식현재가시세(string FID입력종목코드, string FID조건시장분류코드 = "J") {
    using var client = CreateHttpClient("FHKST01010100");

    주식현재가시세Query query = new() {
      FID_COND_MRKT_DIV_CODE = FID조건시장분류코드,
      FID_INPUT_ISCD = FID입력종목코드,
    };

    string url = "/uapi/domestic-stock/v1/quotations/inquire-price?" + WebSerializer.ToQueryString(query);

    var response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode) {
      var respBody = await response.Content.ReadFromJsonAsync<PacketResponse<주식현재가시세DTO>>();
      return respBody?.output;
    }

    return null;
  }

  // 국내주식기간별시세
  public async Task<Ohlcv[]?> FetchOhlcvAsync(
    string stockCode, DateTime startDate, DateTime endDate, Timeframe timeframe = Timeframe.OneDay) {
    using var client = CreateHttpClient("FHKST03010100");

    bool complete = false;
    Ohlcv[] combinedOhlcvs = [];
    DateTime queryStartDate = startDate;
    DateTime queryEndDate = endDate;

    while (!complete) {
      var query = new Dictionary<string, string>() {
        { "FID_COND_MRKT_DIV_CODE", "J" },
        { "FID_INPUT_ISCD", stockCode },
        { "FID_INPUT_DATE_1", queryStartDate.ToString("yyyyMMdd") },
        { "FID_INPUT_DATE_2", queryEndDate.ToString("yyyyMMdd") },
        { "FID_PERIOD_DIV_CODE", timeframe.ToDescription() },
        { "FID_ORG_ADJ_PRC", "0" },
      };

      string url
        = "/uapi/domestic-stock/v1/quotations/inquire-daily-itemchartprice?"
        + WebSerializer.ToQueryString(query);

      var response = await client.GetAsync(url);

      if (!response.IsSuccessStatusCode) return null;

      JsonObject? responseBody = await response.Content.ReadFromJsonAsync<JsonObject>();
      JsonArray jsonArray = responseBody!["output2"]!.AsArray();
      Ohlcv[] chart1 = new Ohlcv[jsonArray.Count];
      int index = chart1.Length - 1;

      foreach (var data in jsonArray) {
        chart1[index] = new Ohlcv(
          DateTime.ParseExact(data!["stck_bsop_date"]!.ToString(), "yyyyMMdd", null),
          double.Parse(data!["stck_oprc"]!.ToString()),
          double.Parse(data!["stck_hgpr"]!.ToString()),
          double.Parse(data!["stck_lwpr"]!.ToString()),
          double.Parse(data!["stck_clpr"]!.ToString()),
          UInt64.Parse(data!["acml_vol"]!.ToString()),
          UInt64.Parse(data!["acml_tr_pbmn"]!.ToString())
        );
        --index;
      }

      // TODO: Ensure complete condition
      if (chart1.FirstOrDefault()?.DateTime <= startDate) {
        chart1 = chart1.Where(ohlcv => ohlcv.DateTime >= startDate).ToArray();
        complete = true;
      }

      Ohlcv[] chart2 = combinedOhlcvs;
      combinedOhlcvs = new Ohlcv[chart1.Length + chart2.Length];
      chart1.CopyTo(combinedOhlcvs, 0);
      chart2.CopyTo(combinedOhlcvs, chart1.Length);

      queryEndDate = chart1.First().DateTime - TimeSpan.FromDays(1);
      // TODO: Modify incorrect end date
    }

    return combinedOhlcvs;
  }

  public async Task<bool> IsOpenDay() {
    return await IsOpenDay(DateTime.Now);
  }

  public async Task<bool> IsOpenDay(DateTime dateTime) {
    using var client = CreateHttpClient("CTCA0903R");

    var query = new Dictionary<string, string>() {
      { "BASS_DT", dateTime.ToString("yyyyMMdd") },
      { "CTX_AREA_NK", "" },
      { "CTX_AREA_FK", "" },
    };

    string url = "/uapi/domestic-stock/v1/quotations/chk-holiday?" + WebSerializer.ToQueryString(query);

    var response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode) {
      var respBody = await response.Content.ReadFromJsonAsync<JsonObject>();
      if (respBody?["output"]?["opnd_yn"]?.ToString() == "Y") {
        return true;
      }
    }

    return false;
  }

  public async Task<string> GetApprovalKey() {
    using var client = CreateHttpClient();
    var response = await client.PostAsJsonAsync("/oauth2/Approval", tokenPRequest.AsApprovalRequest());
    if (response.IsSuccessStatusCode) {
      var respBody = await response.Content.ReadFromJsonAsync<OAuth2ApprovalResponse>();
      return respBody?.ApprovalKey ?? "";
    }

    return "";
  }

  public async Task<bool> CheckAccessToken() {
    AccessToken? preToken = ReadTokenFromDiskCache();
    if (preToken != null) {
      if (DateTime.Now.AddHours(1) < preToken.ExpiresAt) {
        accessToken = preToken;
        return true;
      }
    }

    using HttpClient client = CreateHttpClient();
    var response = await client.PostAsJsonAsync("/oauth2/tokenP", tokenPRequest);

    if (response.IsSuccessStatusCode) {
      var responseBody = await response.Content.ReadFromJsonAsync<AccessToken>();
      if (responseBody != null) {
        accessToken = responseBody;
        accessToken.ExpiresAt = DateTime.Now.AddSeconds((double)responseBody.ExpiresIn);
        WriteTokenToDiskCache(responseBody);

        if (IsDebugMode) {
          Console.WriteLine($"Token: {responseBody.Value}");
          Console.WriteLine($"Type: {responseBody.Type}");
          Console.WriteLine($"ExpiresIn: {responseBody.ExpiresIn} ({responseBody.ExpiresAt})");
        }
      }

      return responseBody != null;
    }

    return false;
  }

  private AccessToken? ReadTokenFromDiskCache() {
    string keyFilePath = GetKeyFilePath();
    if (File.Exists(keyFilePath) == false) {
      return null;
    }

    byte[] encrypted = File.ReadAllBytes(keyFilePath);
    byte[] decrypted = encrypted;

    try {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
        decrypted = ProtectedData.Unprotect(encrypted, entropy, DataProtectionScope.CurrentUser);
      }
    } catch (CryptographicException) {
      return null;
    }

    string userData = Encoding.UTF8.GetString(decrypted);

    try {
      return JsonSerializer.Deserialize<AccessToken>(userData);
    } catch (JsonException) {
      return null;
    }
  }

  private void WriteTokenToDiskCache(AccessToken accessToken) {
    string jsonString;

    try {
      jsonString = JsonSerializer.Serialize(accessToken);
    } catch (JsonException) {
      return;
    }

    byte[] userData = Encoding.UTF8.GetBytes(jsonString);
    byte[] encrypted = userData;

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
      encrypted = ProtectedData.Protect(userData, entropy, DataProtectionScope.CurrentUser);
    }

    File.WriteAllBytes(GetKeyFilePath(), encrypted);
  }

  private string GetKeyFilePath() {
    string tempDir = Path.GetTempPath();
    string fileName = $@"hts.token.{CreateSHA512(tokenPRequest.AppKey)[0..8]}";
    string tempFilePath = Path.Combine(tempDir, fileName);

    return tempFilePath;
  }

  public static string CreateSHA512(string strData) {
    byte[] message = Encoding.UTF8.GetBytes(strData);
    byte[] hashValue = SHA512.HashData(message);
    return Convert.ToHexString(hashValue);
  }
}