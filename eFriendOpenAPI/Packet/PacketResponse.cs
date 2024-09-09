﻿using System.Text.Json.Serialization;

namespace Kis.Packet;

public class PacketResponse<T> {
  [JsonPropertyName("rt_cd")]
  public string rt_cd { get; set; } = "";
  [JsonPropertyName("msg_cd")]
  public string msg_cd { get; set; } = "";
  [JsonPropertyName("msg1")]
  public string msg1 { get; set; } = "";

  public T? output { get; set; }
}