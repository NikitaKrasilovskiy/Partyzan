using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !STEAM
using UnityEngine.Purchasing;

public class iOsRestore :MonoBehaviour, IStoreListener
{
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        extensions.GetExtension<IAppleExtensions> ().RestoreTransactions (result => { if (result)
            {
            // This does not mean anything was restored,
            // merely that the restoration process succeeded.
            }
        else
        {
            // Restoration failed.
        }
    });
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    { }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    { }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    { return new PurchaseProcessingResult(); }

    void Start ()
    {
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		string receipt = builder.Configure<IAppleConfiguration>().appReceipt;
	}
}
#endif