using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
[System.Serializable]
public class MappingFont
{
    public int num;
    public Sprite sprNum;
}
public class ElementShape : MonoBehaviour
{
    [SerializeField] private TextMesh txtMesIdColor;
    [SerializeField] private SpriteRenderer sprRendererColor;
    [SerializeField] private SpriteRenderer sprRenderNumber;
    [SerializeField] private SpriteRenderer sprRenderNumber_1;
    [SerializeField] private Transform parentNumber;

    [SerializeField] private SortingGroup layerText;

    [SerializeField] private Sprite sprBG;
    [SerializeField] private Sprite sprBorder;

    [SerializeField] private List<MappingFont> listMappingFonts = new List<MappingFont>();

    public Color colorShape;
    public Color colorGray;

    public Color colorLerp;
    public Color colorText;

    public bool isDoneColor = false;
    public int numColor;

    public int row; // height
    public int collum; // width

    private LevelLoader levelLoader;
    private bool isUseNumMesh = false;
    private int countNumber = 0;
    public void LoadShape(int idColor, int row, int collum, TypePlayMode typePlayMode, TypeAlbum typeAlbum)
    {
        LoadShape(idColor, row, collum, typePlayMode);
        if (typePlayMode == TypePlayMode.DIY)
        {
            //txtMesIdColor.transform.localRotation = Quaternion.Euler(0, 0, -1 * (typeAlbum == TypeAlbum.Create ? -90 : 0));
            //parentNumber.transform.localRotation = Quaternion.Euler(0, 0, -1 * (typeAlbum == TypeAlbum.Create ? -90 : 0));
        }
    }
    public void LoadShape(int idColor, int row, int collum, TypePlayMode typePlayMode)
    {
        isDoneColor = false;
        this.numColor = idColor;
        txtMesIdColor.text = (numColor + 1).ToString();
        this.row = row;
        this.collum = collum;
        levelLoader = GameplayUIManager.instance.levelLoader;
        ConfigNum();
    }
    private void ConfigNum()
    {
        txtMesIdColor.gameObject.SetActive(false);
        parentNumber.gameObject.SetActive(true);
        sprRenderNumber.gameObject.SetActive(true);
        sprRenderNumber_1.gameObject.SetActive(true);


        isUseNumMesh = false;
        if (numColor < 9)
        {
            countNumber = 1;
            sprRenderNumber.sprite = listMappingFonts[numColor + 1].sprNum;
            sprRenderNumber.transform.localPosition = Vector2.zero;
            sprRenderNumber_1.gameObject.SetActive(false);
        }
        else
        {
            var list = ActionHelper.SplitNumberIntoDigits(numColor + 1);

            countNumber = list.Count;
            if (list.Count <= 2)
            {
                sprRenderNumber.sprite = listMappingFonts[list[0]].sprNum;
                sprRenderNumber.transform.localPosition = Vector2.right * -0.06f;

                sprRenderNumber_1.gameObject.SetActive(true);
                sprRenderNumber_1.sprite = listMappingFonts[list[1]].sprNum;
                sprRenderNumber_1.transform.localPosition = Vector2.right * 0.06f;
            }
            else
            {
                isUseNumMesh = true;
                txtMesIdColor.gameObject.SetActive(true);
                parentNumber.gameObject.SetActive(false);
                txtMesIdColor.text = (numColor + 1).ToString();
            }
        }
    }

    public void SetPos(Vector2 localPos, Vector2 scale)
    {
        transform.localPosition = localPos;
        transform.localScale = scale;
    }
    private void SetSprite(bool isDone)
    {
        sprRendererColor.sprite = isDone ? sprBG : sprBorder;
    }
    private void SetLayer(int layer)
    {
        sprRendererColor.sortingOrder = layer;
        layerText.sortingOrder = layer + 1;
        sprRenderNumber.sortingOrder = layer + 1;
        sprRenderNumber_1.sortingOrder = layer + 1;
    }
    public void SetColor(Color color, bool isDone, bool isSaveData, bool isInit, bool isSound = false)
    {
        if (isDoneColor)
        {
            transform.localScale = Vector2.one;

            txtMesIdColor.gameObject.SetActive(false);
            parentNumber.gameObject.SetActive(false);

            sprRendererColor.color = colorShape;
            return;
        }
        if (!isInit)
        {
            GameplayUIManager.instance.boosterManager.SetStartCountTime();
            if (isSound)
            {
                SoundMusicManager.instance.SoundTapPaintColor();
                levelLoader.SetVibration();
            }
        }

        isDoneColor = isDone;
        sprRendererColor.color = color;

        if (isUseNumMesh)
            txtMesIdColor.gameObject.SetActive(true);
        else
            parentNumber.gameObject.SetActive(true);

        if (isDone)
        {
            if (!isInit)
                levelLoader.SpawnEffectPaintColor(transform.position, color);
            SetSprite(isDone);
            SetLayer(4);
            txtMesIdColor.gameObject.SetActive(false);
            parentNumber.gameObject.SetActive(false);
            levelLoader.SetDoneColor(numColor, isSaveData, isInit, row, collum);
            transform.localScale = Vector2.one;
        }
        else
        {
            SetLayer(8);
        }
    }
    public bool isSetVisualColor = false;
    public void SetColorDone()
    {
        var color = colorShape;
        sprRendererColor.color = new Color(color.r, color.g, color.b, 1);
        SetSprite(true);
        SetLayer(4);

        txtMesIdColor.gameObject.SetActive(false);
        parentNumber.gameObject.SetActive(false);
    }
    public void SetColorDefault(Color color, Color colorText, bool isShowText)
    {
        if (isDoneColor)
        {
            SetSprite(true);
            txtMesIdColor.gameObject.SetActive(false);
            parentNumber.gameObject.SetActive(false);
            sprRendererColor.color = colorShape;
            return;
        }
        SetSprite(false);
        sprRendererColor.color = color;


        if (isUseNumMesh)
        {
            txtMesIdColor.color = colorText;
            txtMesIdColor.gameObject.SetActive(isShowText);
            txtMesIdColor.gameObject.SetActive(true);
        }
        else
        {
            sprRenderNumber.color = colorText;
            sprRenderNumber.gameObject.SetActive(isShowText);

            sprRenderNumber_1.color = colorText;
            if (countNumber == 2)
                sprRenderNumber_1.gameObject.SetActive(isShowText);
            else
                sprRenderNumber_1.gameObject.SetActive(false);

            parentNumber.gameObject.SetActive(true);
        }
    }
    public void SetColorText()
    {
        if (isUseNumMesh)
        {
            txtMesIdColor.color = colorText;
            txtMesIdColor.gameObject.SetActive(true);
        }
        else
        {
            sprRenderNumber.color = colorText;
            sprRenderNumber_1.color = colorText;

            sprRenderNumber.gameObject.SetActive(true);

            if (countNumber == 2)
                sprRenderNumber_1.gameObject.SetActive(true);
            else
                sprRenderNumber_1.gameObject.SetActive(false);

            parentNumber.gameObject.SetActive(true);
        }
    }
    public void SetNone()
    {
        SetSprite(false);
        sprRendererColor.color = new Color(1, 1, 1, 0);
        SetLayer(8);
        txtMesIdColor.gameObject.SetActive(false);
        parentNumber.gameObject.SetActive(false);
    }
    public void SetColorDefault(Color color)
    {
        if (isDoneColor)
        {
            SetSprite(true);
            txtMesIdColor.gameObject.SetActive(false);
            parentNumber.gameObject.SetActive(false);
            sprRendererColor.color = colorShape;
            return;
        }
        SetSprite(false);
        sprRendererColor.color = color;
    }
    public void ResetShape()
    {
        SetSprite(false);
        sprRendererColor.color = Color.white;
        SetLayer(8);
        sprRenderNumber_1.gameObject.SetActive(false);
        sprRenderNumber.gameObject.SetActive(false);
        parentNumber.gameObject.SetActive(false);
        txtMesIdColor.gameObject.SetActive(false);
    }
}
