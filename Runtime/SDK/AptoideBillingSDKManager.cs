using UnityEngine;
using System;
using System.Linq;

public class AptoideBillingSDKManager : MonoBehaviour
{
    private static AptoideBillingSDKManager instance;
    private static AndroidJavaObject aptoideBillingSDKUnityBridge;

    private static IAptoideBillingClientStateListener aptoideBillingClientStateListener;
    private static IConsumeResponseListener consumeResponseListener;
    private static IPurchasesUpdatedListener purchasesUpdatedListener;
    private static IPurchasesResponseListener purchasesResponseListener;
    private static IProductDetailsResponseListener productDetailsResponseListener;

    public static void InitializePlugin(IAptoideBillingClientStateListener _aptoideBillingClientStateListener,
    IConsumeResponseListener _consumeResponseListener,
    IPurchasesUpdatedListener _purchasesUpdatedListener,
    IProductDetailsResponseListener _productDetailsResponseListener,
    IPurchasesResponseListener _purchasesResponseListener,
    string publicKey,
    string className)
    {
        aptoideBillingClientStateListener = _aptoideBillingClientStateListener;
        consumeResponseListener = _consumeResponseListener;
        purchasesUpdatedListener = _purchasesUpdatedListener;
        purchasesResponseListener = _purchasesResponseListener;
        productDetailsResponseListener = _productDetailsResponseListener;

        if (Application.platform == RuntimePlatform.Android)
        {
            aptoideBillingSDKUnityBridge = new AndroidJavaObject("AptoideBillingSDKUnityBridge");
            Initialize(publicKey, className);
            StartConnection();
        }
    }

    // ---- SDK Methods ----

    public static void Initialize(string publicKey, string className)
    {
        aptoideBillingSDKUnityBridge?.CallStatic("initialize", className, publicKey);
    }

    public static void StartConnection()
    {
        aptoideBillingSDKUnityBridge?.CallStatic("startConnection");
    }

    public static void EndConnection()
    {
        aptoideBillingSDKUnityBridge?.CallStatic("endConnection");
    }

    public static bool IsReady()
    {
        bool isReady = aptoideBillingSDKUnityBridge?.CallStatic<bool>("isReady") ?? false;
        Debug.Log($"AptoideBillingSDKManager | IsReady: {isReady}");

        return isReady;
    }

    public static void QueryProductDetailsAsync(QueryProductDetailsParams queryProductDetailsParams)
    {
        using (AndroidJavaObject productsList = new AndroidJavaObject("java.util.ArrayList"))
        {
            string productType = null;
            foreach (QueryProductDetailsParams.Product productParams in queryProductDetailsParams.ProductList)
            {
                productType = productParams.ProductType;
                productsList.Call<bool>("add", productParams.ProductId);
            }
            aptoideBillingSDKUnityBridge?.CallStatic("queryProductDetailsAsync", productsList, productType);
        }
    }

    public static BillingResult LaunchBillingFlow(BillingFlowParams billingFlowParams)
    {
        string sku = billingFlowParams.Sku;
        string skuType = billingFlowParams.SkuType;
        string developerPayload = billingFlowParams.DeveloperPayload;
        string obfuscatedAccountId = billingFlowParams.ObfuscatedAccountId;
        bool freeTrial = billingFlowParams.FreeTrial;
        string billingResultJson = aptoideBillingSDKUnityBridge?.CallStatic<string>("launchBillingFlow", sku, skuType, developerPayload, obfuscatedAccountId, freeTrial) ?? "{}";

        BillingResult billingResult = JsonUtility.FromJson<BillingResult>(billingResultJson);

        Debug.Log($"AptoideBillingSDKManager | LaunchBillingFlow: responseCode: {billingResult.ResponseCode}, debugMessage: {billingResult.DebugMessage}");

        return billingResult;
    }

    public static void ConsumeAsync(ConsumeParams consumeParams)
    {
        aptoideBillingSDKUnityBridge?.CallStatic("consumeAsync", consumeParams.PurchaseToken);
    }

    public static BillingResult IsFeatureSupported(int feature)
    {
        string billingResultJson = aptoideBillingSDKUnityBridge?.CallStatic<string>("isFeatureSupported", feature) ?? "{}";

        BillingResult billingResult = JsonUtility.FromJson<BillingResult>(billingResultJson);
        Debug.Log($"AptoideBillingSDKManager | IsFeatureSupported: responseCode: {billingResult.ResponseCode}, debugMessage: {billingResult.DebugMessage}");

        return billingResult;
    }

    public static void QueryPurchasesAsync(QueryPurchasesParams queryPurchasesParams)
    {
        aptoideBillingSDKUnityBridge?.CallStatic("queryPurchasesAsync", queryPurchasesParams.ProductType);
    }

    public static ReferralDeeplinkResult GetReferralDeeplink()
    {
        string referralDeeplinkJson = aptoideBillingSDKUnityBridge?.CallStatic<string>("getReferralDeeplink");
        Debug.Log($"AptoideBillingSDKManager | GetReferralDeeplink: {referralDeeplinkJson}");

        ReferralDeeplinkResult referralDeeplinkResult = JsonUtility.FromJson<ReferralDeeplinkResult>(referralDeeplinkJson);
        return referralDeeplinkResult;
    }

    public static bool IsAppUpdateAvailable()
    {
        bool isAppUpdateAvailable = aptoideBillingSDKUnityBridge?.CallStatic<bool>("isAppUpdateAvailable") ?? false;
        Debug.Log($"AptoideBillingSDKManager | IsAppUpdateAvailable: {isAppUpdateAvailable}");

        return isAppUpdateAvailable;
    }

    public static void LaunchAppUpdateDialog()
    {
        aptoideBillingSDKUnityBridge?.CallStatic("launchAppUpdateDialog");
    }

    public static void LaunchAppUpdateStore()
    {
        aptoideBillingSDKUnityBridge?.CallStatic("launchAppUpdateStore");
    }

    // ---- Callback Handlers from Java ----

    public void BillingSetupFinishedCallback(string billingResultJson)
    {
        Debug.Log($"Aptoide Billing Setup Finished: {billingResultJson}");

        BillingResult billingResult = JsonUtility.FromJson<BillingResult>(billingResultJson);

        aptoideBillingClientStateListener.OnBillingSetupFinished(billingResult);
    }

    public void BillingServiceDisconnectedCallback(string _)
    {
        Debug.LogWarning("AptoideBillingSDKManager | Aptoide Billing Service Disconnected");
        aptoideBillingClientStateListener.OnBillingServiceDisconnected();
    }

    public void PurchasesUpdatedCallback(string purchasesResultJson)
    {
        Debug.Log($"AptoideBillingSDKManager | Purchase Updated: {purchasesResultJson}");

        PurchasesResult purchasesResult = JsonUtility.FromJson<PurchasesResult>(purchasesResultJson);

        purchasesUpdatedListener.OnPurchasesUpdated(purchasesResult.BillingResult, purchasesResult.Purchases);
    }

    public void PurchasesResponseCallback(string purchasesResultJson)
    {
        Debug.Log($"AptoideBillingSDKManager | Purchases Response: {purchasesResultJson}");

        PurchasesResult purchasesResult = JsonUtility.FromJson<PurchasesResult>(purchasesResultJson);

        purchasesResponseListener.OnQueryPurchasesResponse(purchasesResult.BillingResult, purchasesResult.Purchases);
    }

    public void ProductDetailsResponseCallback(string productDetailsResultJson)
    {
        Debug.Log($"AptoideBillingSDKManager | Product Details Received: {productDetailsResultJson}");

        ProductDetailsRequestResult productDetailsResult = JsonUtility.FromJson<ProductDetailsRequestResult>(productDetailsResultJson);

        productDetailsResponseListener.OnProductDetailsResponse(productDetailsResult.BillingResult, productDetailsResult.ProductDetailsResult);
    }

    public void ConsumeResponseCallback(string consumeResultJson)
    {
        Debug.Log($"AptoideBillingSDKManager | Consume Response: {consumeResultJson}");

        ConsumeResult consumeResult = JsonUtility.FromJson<ConsumeResult>(consumeResultJson);

        consumeResponseListener.OnConsumeResponse(consumeResult.BillingResult, consumeResult.PurchaseToken);
    }
}
