namespace Kis.Utils;

public static class StringExtension {
  public static decimal ToDecimal(this string value) {
    if (string.IsNullOrEmpty(value)) {
      return 0;
    }

    return decimal.Parse(value);
  }
}
