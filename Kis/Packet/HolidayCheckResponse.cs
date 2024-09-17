using System.Text.Json.Serialization;

namespace Kis.Packet;

public class HolidayCheckResponse<T> {
  [JsonPropertyName("ctx_area_nk")]
  public string ctx_area_nk { get; set; } = "";
  [JsonPropertyName("ctx_area_fk")]
  public string ctx_area_fk { get; set; } = "";

  public T[]? output { get; set; }
}
