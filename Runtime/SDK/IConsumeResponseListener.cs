public interface IConsumeResponseListener
{
    void OnConsumeResponse(BillingResult billingResult, string purchaseToken);
}