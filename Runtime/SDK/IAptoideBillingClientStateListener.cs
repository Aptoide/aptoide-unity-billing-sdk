public interface IAptoideBillingClientStateListener {
    void OnBillingSetupFinished(BillingResult billingResult);

    void OnBillingServiceDisconnected();
}
