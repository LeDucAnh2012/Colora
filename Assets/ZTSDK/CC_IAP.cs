using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using UnityEngine.Purchasing.Extension;
using System.Transactions;
using UnityEngine.Purchasing.Security;

public enum IAP_Product
{
    None,
    RemoveAds,
    RemoveAdsOld
}


[System.Serializable]
public class IdProduct
{
    public string idProd;
    public IAP_Product idAndroid;
    public IAP_Product idIOS;
    public ProductType type;
    public string price;
}


public class CC_IAP : MonoBehaviour, IDetailedStoreListener
{
    public static CC_IAP instance;

    public IdProduct[] idProductIAP;
    public static string productIDConsumable;
    public static string productIDNonConsumable;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    private string transactionID, purchaseToken, currencyCode, price;
    private bool m_PurchaseInProgress;
    private CrossPlatformValidator validator;
    private bool m_IsGooglePlayStoreSelected;
    private bool isBuyIAP = false;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (m_StoreController == null)
            SetupBuilder();
        LocalizedPrice();
    }


    #region setup and initialize
    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    void SetupBuilder()
    {
        if (IsInitialized())
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < idProductIAP.Length; i++)
        {
            if (ActionHelper.IsAndroid())
                builder.AddProduct(idProductIAP[i].idProd, idProductIAP[i].type);
            if (ActionHelper.IsIOS())
                builder.AddProduct(idProductIAP[i].idProd, idProductIAP[i].type);
        }

        UnityPurchasing.Initialize(this, builder);
    }
    #region Localize price
    public void LocalizedPrice()
    {
        if (m_StoreController != null)
        {
            for (int i = 0; i < idProductIAP.Length; i++)
                foreach (var product in m_StoreController.products.all)
                    if (product.definition.id == idProductIAP[i].idProd)
                        idProductIAP[i].price = product.metadata.localizedPriceString;
        }
    }
    public string Localized(IAP_Product idProduct)
    {
        string price = "hảo hảo chua cay";
        for (int i = 0; i < idProductIAP.Length; i++)
        {
            if (ActionHelper.IsAndroid())
                if (idProduct == idProductIAP[i].idAndroid)
                {
                    price = idProductIAP[i].price;
                    break;
                }
            if (ActionHelper.IsIOS())
                if (idProduct == idProductIAP[i].idIOS)
                {
                    price = idProductIAP[i].price;
                    break;
                }
        }
        return price;
    }
    #endregion

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    #endregion


    #region button clicks 
    public void BuyIAP(IAP_Product _idProductIAP)
    {
        isBuyIAP = true;
        for (int i = 0; i < idProductIAP.Length; i++)
        {
            if (ActionHelper.IsAndroid())
                if (_idProductIAP == idProductIAP[i].idAndroid)
                {
                    if (idProductIAP[i].type == ProductType.Consumable)
                    {
                        productIDConsumable = idProductIAP[i].idProd;
                        BuyProductID(productIDConsumable);
                    }
                    else
                    {
                        productIDNonConsumable = idProductIAP[i].idProd;
                        BuyProductID(productIDNonConsumable);
                    }
                }

            if (ActionHelper.IsIOS())
                if (_idProductIAP == idProductIAP[i].idIOS)
                {
                    if (idProductIAP[i].type == ProductType.Consumable)
                    {
                        productIDConsumable = idProductIAP[i].idProd;
                        BuyProductID(productIDConsumable);
                    }
                    else
                    {
                        productIDNonConsumable = idProductIAP[i].idProd;
                        BuyProductID(productIDNonConsumable);
                    }
                }
        }
    }
    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                PurchaseErrorCallBack();
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            PurchaseErrorCallBack();
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }
    void PurchaseErrorCallBack()
    {
        CC_Interface.instance.PurchaseCallback(false);
    }
    void PurchaseSuccessCallback(PurchaseEventArgs purchaseEvent)
    {
        string str = purchaseEvent.purchasedProduct.definition.id;
        Debug.Log("PurchaseSuccessCallback = " + str);

        for (int i = 0; i < idProductIAP.Length; i++)
        {
            if (str.Equals(idProductIAP[i].idProd))
            {
                var id = ActionHelper.IsAndroid() ? idProductIAP[i].idAndroid : idProductIAP[i].idIOS;
                Debug.Log("PurchaseSuccessCallback iap = " + id.ToString());
                CC_Interface.instance.RestorePurchase(id);
                if (isBuyIAP)
                {
                    double price = (double)m_StoreController.products.WithID(idProductIAP[i].idProd).metadata.localizedPrice;
                    string currency = m_StoreController.products.WithID(idProductIAP[i].idProd).metadata.isoCurrencyCode;
                    ActionHelper.TrackRevenueIAP(currency, price);
                }
            }
        }
        CC_Interface.instance.PurchaseCallback(true);
    }
    private void RestorePurchase(PurchaseEventArgs purchaseEvent)
    {
        string str = purchaseEvent.purchasedProduct.definition.id;
        Debug.Log("RestorePurchase = " + str);

        for (int i = 0; i < idProductIAP.Length; i++)
        {
            if (str.Equals(idProductIAP[i].idProd))
            {
                var id = ActionHelper.IsAndroid() ? idProductIAP[i].idAndroid : idProductIAP[i].idIOS;
                Debug.Log("restore iap = " + id.ToString());
                CC_Interface.instance.RestorePurchase(id);
            }
        }
    }

    #endregion


    #region main
    //processing purchase
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(purchaseEvent.purchasedProduct.definition.id, productIDConsumable, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            transactionID = purchaseEvent.purchasedProduct.transactionID;

            //var result = validator.Validate(e.purchasedProduct.receipt);

            //GooglePlayReceipt google = args.purchasedProduct.receipt as GooglePlayReceipt;
            PurchaseSuccessCallback(purchaseEvent);
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (String.Equals(purchaseEvent.purchasedProduct.definition.id, productIDNonConsumable, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase Nonconsume: PASS. Product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
            transactionID = purchaseEvent.purchasedProduct.transactionID;
            PurchaseSuccessCallback(purchaseEvent);

            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
            //restoreIAP(args.purchasedProduct.definition.id);
            RestorePurchase(purchaseEvent);
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        //throw new NotImplementedException();
        Debug.Log("OnPurchaseFailed");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        //throw new NotImplementedException();
        Debug.Log("OnPurchaseFailed");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        //throw new NotImplementedException();
        Debug.Log("OnPurchaseFailed");
    }

    #endregion

    /*
    #region IOS

    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
             Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {

                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            PurchaseErrorCallBack();
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + e.purchasedProduct.receipt);

        m_PurchaseInProgress = false;
        Debug.Log("m_isgoogle : " + m_IsGooglePlayStoreSelected);
        // Local validation is available for GooglePlay, and Apple stores
        if (m_IsGooglePlayStoreSelected ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.tvOS)
        {
            try
            {
                var result = validator.Validate(e.purchasedProduct.receipt);
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                    //transactionID = productReceipt.transactionID;
                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        Debug.Log(google.purchaseState);
                        Debug.Log(google.purchaseToken);
                        purchaseToken = google.purchaseToken;
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple)
                    {
                        Debug.Log(apple.originalTransactionIdentifier);
                        Debug.Log(apple.subscriptionExpirationDate);
                        Debug.Log(apple.cancellationDate);
                        Debug.Log(apple.quantity);
                    }

                    // For improved security, consider comparing the signed
                    // IPurchaseReceipt.productId, IPurchaseReceipt.transactionID, and other data
                    // embedded in the signed receipt objects to the data which the game is using
                    // to make this purchase.
                }
            }
            catch (IAPSecurityException ex)
            {
                Debug.Log("Invalid receipt, not unlocking content. " + ex);
                return PurchaseProcessingResult.Complete;
            }
        }


        // Unlock content from purchases here.

        if (e.purchasedProduct.definition.payouts != null)
        {
            Debug.Log("Purchase complete, paying out based on defined payouts");
            transactionID = e.purchasedProduct.transactionID;
            //purchaseToken = e.purchasedProduct.receipt.
            currencyCode = e.purchasedProduct.metadata.isoCurrencyCode;
            price = e.purchasedProduct.metadata.localizedPrice.ToString();
            Debug.Log("transactionID=" + transactionID);
            Debug.Log("purchaseToke=" + purchaseToken);
            PurchaseSuccessCallback();


            //foreach (var payout in e.purchasedProduct.definition.payouts) {

            //    Debug.Log(string.Format("Granting {0} {1} {2} {3}", payout.quantity, payout.typeString, payout.subtype, payout.data));
            //}
        }

        // Indicate if we have handled this purchase.
        //   PurchaseProcessingResult.Complete: ProcessPurchase will not be called
        //     with this product again, until next purchase.
        //   PurchaseProcessingResult.Pending: ProcessPurchase will be called
        //     again with this product at next app launch. Later, call
        //     m_Controller.ConfirmPendingPurchase(Product) to complete handling
        //     this purchase. Use to transactionally save purchases to a cloud
        //     game service.
        //#if DELAY_CONFIRMATION
        //        StartCoroutine(ConfirmPendingPurchaseAfterDelay(e.purchasedProduct));
        //        return PurchaseProcessingResult.Pending;
        //#else
        return PurchaseProcessingResult.Complete;
        //#endif
    }
    #endregion
    */
}




