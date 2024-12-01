using UnityEngine;
using UnityEngine.Events;

public class PanelBase : MonoBehaviour
{
    public bool isShow;
    public GameplayController gameplayController => GameplayController.instance;
    public HomeUIManager homeUIManager => HomeUIManager.instance;
    public GameplayUIManager gameplayUIManager => GameplayUIManager.instance;
    public CanvasAllScene canvasAllScene => CanvasAllScene.instance;
    public InitDataGame initDataGame=> InitDataGame.instance;
    public virtual void Show()
    {
        OnOffObject(true);
    }
    public virtual void Hide()
    {
        OnOffObject(false);
    }
    private void OnOffObject(bool isShow)
    {
        this.isShow = isShow;
        gameObject.SetActive(isShow);
    }
    public void SoundClickButton()
    {
        SoundMusicManager.instance.SoundClickButton();
    }
    protected void DelayCallback(UnityAction callback, float timeDelay)
    {
        StartCoroutine(ActionHelper.StartAction(() =>
        {
            callback?.Invoke();
        }, timeDelay));
    }
    public void SoundShowPopup()
    {
        SoundMusicManager.instance.SoundShowPopup();
    }
}
