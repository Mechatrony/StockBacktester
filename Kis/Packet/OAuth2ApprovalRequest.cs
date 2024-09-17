using System.Text.Json.Serialization;

namespace Kis.Packet;

public class OAuth2ApprovalRequest {
  [JsonPropertyName("grant_type")]
  public string GrantType { get; set; } = "client_credentials";
  [JsonPropertyName("appkey")]
  public string AppKey { get; set; } = "";
  [JsonPropertyName("secretkey")]
  public string SecretKey { get; set; } = "";
}
