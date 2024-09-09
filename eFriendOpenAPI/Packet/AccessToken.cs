using System.Text.Json.Serialization;

namespace Kis.Packet;

public class AccessToken {
  [JsonPropertyName("access_token")]
  public string Value { get; set; } = "";
  [JsonPropertyName("token_type")]
  public string Type { get; set; } = "";
  [JsonPropertyName("expires_in")]
  public decimal ExpiresIn { get; set; } = 0;

  public DateTime ExpiresAt { get; set; } = DateTime.MinValue;
}
