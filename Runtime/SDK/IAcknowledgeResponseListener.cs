public interface IAcknowledgeResponseListener
{
    void OnAcknowledgeResponse(BillingResult billingResult, string purchaseToken);
}
