namespace StockBacktester.Crypto;

public class CoinInfo(string name, int amountPrecision, int pricePrecision, DateTime listingDate) {
  public string Name { get; set; } = name;
  public int AmountPrecision { get; set; } = amountPrecision;
  public int PricePrecision { get; set; } = pricePrecision;
  public DateTime ListingDate { get; set; } = listingDate;
  public double CirculatingSupply { get; set; }
}
