using UnityEngine;
using UnityEngine.UI;

public class LocalizePrice : MonoBehaviour
{
    public IAP_Product idProduct;
    Text localizePrice;
    string localizeText;
    public bool isOnable = true;
    void Awake()
    {
        localizePrice = gameObject.GetComponent<Text>();

    }
    //#if LEADER
    void Start()
    {
        //Retrieve ();
    }
    void OnEnable()
    {
        if (isOnable)
        {
            CC_IAP.instance.LocalizedPrice();
            Retrieve();
        }
    }

    public void OnSetLocalize()
    {
        CC_IAP.instance.LocalizedPrice();
        Retrieve();
    }
    public void Retrieve()
    {
        if (localizePrice == null)
            localizePrice = gameObject.GetComponent<Text>();

        localizePrice.text = CC_IAP.instance.Localized(idProduct);
    }

}
