using System;

public class AcknowledgeParams
{
    public string PurchaseToken { get; }

    private AcknowledgeParams(string purchaseToken)
    {
        PurchaseToken = purchaseToken;
    }

    public class Builder
    {
        private string purchaseToken;

        public Builder SetPurchaseToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Purchase token must not be empty.");
            }

            purchaseToken = token;
            return this;
        }

        public AcknowledgeParams Build()
        {
            if (string.IsNullOrEmpty(purchaseToken))
            {
                throw new InvalidOperationException("Purchase token must be provided.");
            }

            return new AcknowledgeParams(purchaseToken);
        }
    }

    public static Builder NewBuilder()
    {
        return new Builder();
    }
}
