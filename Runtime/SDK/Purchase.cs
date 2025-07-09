[System.Serializable]
public class Purchase
{
    // Purchase details
    public AccountIdentifiers AccountIdentifiers;
    public string DeveloperPayload;
    public string OrderId;
    public string OriginalJson;
    public string PackageName;
    public string[] Products;
    public int PurchaseState;
    public long PurchaseTime;
    public string PurchaseToken;
    public string Signature;
    public bool isAutoRenewing;
}
