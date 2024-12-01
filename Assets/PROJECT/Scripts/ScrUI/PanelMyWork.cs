using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMyWork : PanelBase
{
    [SerializeField] private ElementLevel eleSpawn;
    [SerializeField] private Transform parSpawnInprogress;
    [SerializeField] private Transform parSpawnFinish;

    [Space]
    [SerializeField] private Text txtInprogress;
    [SerializeField] private Text txtCompleted;

    [Space]
    [SerializeField] private Button btnInprogress;
    [SerializeField] private Button btnFinish;

    [Space]
    [SerializeField] private GameObject scrollInprogress;
    [SerializeField] private GameObject scrollFinished;

    [Space]
    [SerializeField] private Color colorText;
    [SerializeField] private Color colorTextSelect;
    [SerializeField] private Color colorShadow;
    [SerializeField] private Color colorShadowSelect;

    public List<ElementLevel> listInProgress = new List<ElementLevel>();
    public List<ElementLevel> listFinished = new List<ElementLevel>();

    private int countFinishDIY = 0;
    private int countProgressDIY = 0;
    private IEnumerator SET_PARENT = null;

    public override void Show()
    {
        base.Show();
        listInProgress.Clear();
        listFinished.Clear();

        if (SET_PARENT != null)
        {
            StopCoroutine(SET_PARENT);
            SET_PARENT = null;
        }
        SET_PARENT = SetParent();
        StartCoroutine(SET_PARENT);
    }

    private IEnumerator SetParent()
    {
        homeUIManager.panelLevel.SetParentShape(parSpawnInprogress, parSpawnFinish);
        yield return new WaitForEndOfFrame();
        SetText();
        ActiveInProgress();
        ActiveFinish();
    }
    private void SetText()
    {
        var valFinish = homeUIManager.panelLevel.CountFinish + countFinishDIY;
        var valProgress = homeUIManager.panelLevel.CountInProgress + countProgressDIY;

        txtCompleted.text = I2.Loc.ScriptLocalization.Completed.ToUpper() + "(" + valFinish + ")";
        txtInprogress.text = I2.Loc.ScriptLocalization.In_Progress.ToUpper() + "(" + valProgress + ")";
    }
    public void SpawnShapeDIY()
    {
        var list = DataDIY.metadataList.textures;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].stateDone == StateDone.Done)
            {
                var el1 = Instantiate(eleSpawn, parSpawnFinish);
                el1.LoadData(list[i], DataDIY.LoadTextureByID(list[i].textureID, false));
                countFinishDIY++;
            }
            else
            {
                var el1 = Instantiate(eleSpawn, parSpawnInprogress);
                el1.LoadData(list[i], DataDIY.LoadTextureByID(list[i].textureID, false));
                countProgressDIY++;
            }
        }

        SetText();
    }
    public override void Hide()
    {
        homeUIManager.panelLevel.ResetParentShape();
        base.Hide();
    }

    public void LoadData()
    {
        SpawnShapeDIY();
        ConfigUI(true);
    }

    public void OnClickInprogress()
    {
        SoundClickButton();
        ConfigUI(true);
    }

    public void OnClickFinished()
    {
        SoundClickButton();
        ConfigUI(false);
    }

    private void ConfigUI(bool isOn)
    {
        scrollInprogress.SetActive(isOn);
        scrollFinished.SetActive(!isOn);

        btnInprogress.image.color = new Color(1, 1, 1, isOn ? 1 : 0);
        btnInprogress.transform.GetChild(1).gameObject.SetActive(isOn);

        btnFinish.image.color = new Color(1, 1, 1, isOn ? 0 : 1);
        btnFinish.transform.GetChild(1).gameObject.SetActive(!isOn);

        SetColorText(txtInprogress, isOn);
        SetColorText(txtCompleted, !isOn);

    }
    private void SetColorText(Text txt, bool isOn)
    {
        txt.color = isOn ? colorTextSelect : colorText;
        var shadows = txt.GetComponents<Shadow>();
        foreach (var shadow in shadows)
        {
            shadow.effectColor = isOn ? colorShadowSelect : colorShadow;
        }
        txt.GetComponent<Outline>().effectColor = isOn ? colorShadowSelect : colorShadow;
    }
    public void ActiveInProgress()
    {
        foreach (var scr in listInProgress)
        {
            var check = scr.transform.position.y > 55 || scr.transform.position.y < (scr.isLoaded ? -44 : -116);
            scr.transform.GetChild(0).gameObject.SetActive(!check);
        }
    }
    public void ActiveFinish()
    {
        foreach (var scr in listFinished)
        {
            var check = scr.transform.position.y > 55 || scr.transform.position.y < (scr.isLoaded ? -44 : -116);
            scr.transform.GetChild(0).gameObject.SetActive(!check);
        }
    }
}

