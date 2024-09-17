using System.Text.Json.Serialization;

namespace Kis.Packet;

public class OAuth2ApprovalResponse {
  [JsonPropertyName("approval_key")]
  public string ApprovalKey { get; set; } = "";
}
