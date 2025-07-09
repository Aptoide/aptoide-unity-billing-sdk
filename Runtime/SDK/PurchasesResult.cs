[System.Serializable]
public class PurchasesResult
{
    public BillingResult BillingResult; // BillingResult of Purchases request
    public Purchase[] Purchases; // Array of purchases (if any)
}
