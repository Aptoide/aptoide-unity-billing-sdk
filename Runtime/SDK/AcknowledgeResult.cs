[System.Serializable]
public class AcknowledgeResult
{
    public BillingResult BillingResult; // Billing Result of the Acknowledge action
    public string PurchaseToken; // Purchase token of the Acknowledged Purchase
}
