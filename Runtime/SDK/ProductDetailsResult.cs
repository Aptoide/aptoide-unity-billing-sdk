[System.Serializable]
public class ProductDetailsResult
{
    public BillingResult BillingResult; // BillingResult of the Product Details request
    public QueryProductDetailsResult ProductDetailsResult; // Products details result containing the fetched and unfetched products
}
