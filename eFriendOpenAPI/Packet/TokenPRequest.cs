using System.Text.Json.Serialization;

namespace Kis.Packet;

public class TokenPRequest {
  [JsonPropertyName("grant_type")]
  public string GrantType { get; set; } = "client_credentials";
  [JsonPropertyName("appkey")]
  public string AppKey { get; set; } = "";
  [JsonPropertyName("appsecret")]
  public string SecretKey { get; set; } = "";

  public OAuth2ApprovalRequest AsApprovalRequest() {
    return new OAuth2ApprovalRequest { GrantType = GrantType, AppKey = AppKey, SecretKey = SecretKey };
  }
}