namespace Kis;

public class Account {
  public string CANO { get; private set; }
  public string ACNT_PRDT_CD { get; private set; }

  public Account(string account) {
    if (string.IsNullOrEmpty(account) || account.Length < 10) {
      throw new ArgumentException(null, $"{nameof(account)}");
    }

    CANO = account[0..8];
    ACNT_PRDT_CD = account[8..];
  }

  public override string ToString() {
    return $"{CANO}-{ACNT_PRDT_CD}";
  }
}
