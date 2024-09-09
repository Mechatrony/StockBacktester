using System.Net.Http.Headers;

namespace KisApi;

public class KisClient {
  private static byte[] _entropy = { 0xfd, 0xed, 0x10, 0x20, 0xe9, 0x75 };
  private static HttpClientHandler _sharedHandler = new() { MaxConnectionsPerServer = 1 };
  private string _baseUrl;
  //private TokenPRequest _tokenPRequest;
  //private AccessToken? _accessToken;
  //private 계좌번호 _account;

  private bool _isVTS = false;
  private bool _isLegalPerson = false;
  private string _legalPersonSecretKey;
  private string _macAddress = "";
  private string _legalPhoneNumber = "";
  private string _legalPublicIP = "";
  private string _hashKey = ""; // Not supported yet.
  private string _legalUID = "";

  //public 계좌번호 Account { get => _account; }
  public bool DebugMode { get; set; } = false;

  public HttpClient NewHttp(string trId = "", string tr_cont = "") {
    HttpClient client;

    if (this.DebugMode) {
      client = new HttpClient(new LoggingHandler(_sharedHandler), false);
    } else {
      client = new HttpClient(_sharedHandler, false);
    }

    client.BaseAddress = new Uri(_baseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    if (string.IsNullOrEmpty(trId) == false) {
      client.DefaultRequestHeaders.Add("authorization", $"{_accessToken?.Type} {_accessToken?.Value}");
      client.DefaultRequestHeaders.Add("appkey", _tokenPRequest.AppKey);
      client.DefaultRequestHeaders.Add("appsecret", _tokenPRequest.SecretKey);

      client.DefaultRequestHeaders.Add("tr_id", trId);

      if (string.IsNullOrEmpty(tr_cont) == false) {
        client.DefaultRequestHeaders.Add("tr_cont", "");
      }

      client.DefaultRequestHeaders.Add("custtype", _isLegalPerson ? "B" : "P");

      if (string.IsNullOrEmpty(_macAddress) == false) {
        client.DefaultRequestHeaders.Add("mac_addr", _macAddress);
      }

      if (string.IsNullOrEmpty(_hashKey) == false) {
        client.DefaultRequestHeaders.Add("hashkey", _hashKey);
      }

      if (_isLegalPerson) {
        client.DefaultRequestHeaders.Add("personalseckey", _legalPersonSecretKey);
        client.DefaultRequestHeaders.Add("seq_no", "01");
        client.DefaultRequestHeaders.Add("phone_number", _legalPhoneNumber);
        client.DefaultRequestHeaders.Add("ip_addr", _legalPublicIP);
        client.DefaultRequestHeaders.Add("gt_uid", _legalUID);
      }
    }

    return client; ;
  }
}
