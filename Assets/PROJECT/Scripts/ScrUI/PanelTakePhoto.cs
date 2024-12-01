using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelTakePhoto : PanelBase
{
    [Space]
    [SerializeField] private Button btnBack;
    [SerializeField] private RawImage m_image;
    [SerializeField] private Slider m_qualitySlider;
    [SerializeField] private CameraPlugin m_cameraPlugin;

    [Space]
    [SerializeField] private GameObject objTake;
    [SerializeField] private GameObject objTakeDone;

    [Space]
    [SerializeField] private Material malGray;
    [SerializeField] private Material malDefault;

    private Texture2D m_fullTexture;

    private Rect m_rect = new Rect(0f, 0f, 1f, 1f);

    private Rect m_lastRect;

    private float m_maxZoom = 5f;

    private float m_currentImageZoom = 1f;

    private bool m_subscribed;

    private int m_maxSize = 1024;

    private TypeAlbum typeAlbum;

    private List<Color> colorList = new List<Color>();
    private List<Color> colorListGray = new List<Color>();
    private List<int> listMap = new List<int>();
    private List<int> listMapNoColor = new List<int>();
    private Dictionary<Color, int> dicColor = new Dictionary<Color, int>();

    public override void Show()
    {
        base.Show();
    }
    private void Start()
    {
        // if (!ActionHelper.IsEditor())
        ShowObjectTake(true);
        StartCoroutine(Init());
        m_fullTexture = (Texture2D)m_image.texture;
    }
    private Action action;
    public IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        action = (Action)Delegate.Combine(action, (Action)delegate
        {
            CameraPlugin cameraPlugin = this.m_cameraPlugin;
            cameraPlugin.OnUpdateTexture = (Action<Texture2D, float>)Delegate.Combine(cameraPlugin.OnUpdateTexture, new Action<Texture2D, float>(this.OnUpdateTextureHandler));
            this.m_cameraPlugin.Initilized();
            this.m_cameraPlugin.Quality = (this.m_qualitySlider.minValue + this.m_qualitySlider.maxValue) / 2f;
            this.m_qualitySlider.value = this.m_cameraPlugin.Quality;
        });
        action.SafeInvoke();

        //CameraPlugin cameraPlugin = this.m_cameraPlugin;
        //cameraPlugin.OnUpdateTexture = (Action<Texture2D, float>)Delegate.Combine(cameraPlugin.OnUpdateTexture, new Action<Texture2D, float>(this.OnUpdateTextureHandler));
        //this.m_cameraPlugin.Initilized();
        //this.m_cameraPlugin.Quality = (this.m_qualitySlider.minValue + this.m_qualitySlider.maxValue) / 2f;
        //this.m_qualitySlider.value = this.m_cameraPlugin.Quality;

        this.m_qualitySlider.value = (this.m_qualitySlider.minValue + this.m_qualitySlider.maxValue) / 2f;
        this.m_image.texture = null;
    }
    private void ShowObjectTake(bool isShow)
    {
        if (!ActionHelper.IsEditor())
        {
            objTake.SetActive(isShow);
            objTakeDone.SetActive(!isShow);
        }
    }

    private void CamScaleTexture(Texture2D texture, int height, float ratio, float quality)
    {
        TextureScale.Point(texture, (int)(height * ratio * quality), (int)(height * quality));
    }
    private void OnUpdateTextureHandler(Texture2D tex, float angle)
    {
        CamScaleTexture(tex, 40, 1f, this.m_qualitySlider.value);
        this.m_image.rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        this.m_image.texture = tex;
    }
    public void QualitySliderValueChanged(float value)
    {
        this.m_cameraPlugin.Quality = value;

        if (objTakeDone.activeSelf)
            this.UpdateTexture(true);
    }

    private void UpdateTexture(bool force = false)
    {
        try
        {
            if (!(this.m_rect != this.m_lastRect) && !force)
            {
                return;
            }

            m_lastRect = m_rect;
            Texture2D texture2D = null;
            if (this.m_rect == new Rect(0f, 0f, 1f, 1f))
            {
                Texture2D texture2D2 = new Texture2D(m_fullTexture.width, m_fullTexture.height, TextureFormat.RGB24, false);
                texture2D2.filterMode = FilterMode.Point;
                texture2D = texture2D2;
                texture2D.SetPixels(m_fullTexture.GetPixels());
            }
            else
            {
                int x = (int)(m_rect.xMin * m_fullTexture.width);
                int y = (int)(m_rect.yMin * m_fullTexture.height);
                int a = (int)(m_rect.width * m_fullTexture.width);
                int b = (int)(m_rect.height * m_fullTexture.height);

                int num = Mathf.Min(a, b);
                if (num >= m_fullTexture.width || num >= m_fullTexture.height)
                {

                }
                Color[] pixels = m_fullTexture.GetPixels(x, y, num, num);

                Texture2D texture2D2 = new Texture2D(num, num, TextureFormat.RGB24, false);
                texture2D2.filterMode = FilterMode.Point;
                texture2D = texture2D2;
                texture2D.SetPixels(pixels);
                texture2D.Apply();
            }
            this.m_image.texture = texture2D;
            CamScaleTexture((Texture2D)m_image.texture, 40, 1f, m_qualitySlider.value);
        }
        catch (Exception e)
        {
            // Debug.LogError($"Exception caught: {e.Message}\n{e.StackTrace}");
            Debug.Log("Keo tu tu thoi");
        }
    }

    public void OnClickChangeCamera()
    {
        SoundClickButton();
        this.m_cameraPlugin.ChangeCamera();
    }


    public void OnClickTakePhoto()
    {
        SoundClickButton();
        try
        {
            Texture2D texture2D = this.m_cameraPlugin.TakeSnapshot();
            if (texture2D.width > this.m_maxSize || texture2D.height > this.m_maxSize)
            {
                float num = (float)texture2D.width / (float)texture2D.height;
                int height = (int)Mathf.Min((float)this.m_maxSize, (float)this.m_maxSize / num);
                CamScaleTexture(texture2D, height, num, 1f);
            }
            this.m_fullTexture = texture2D;
            this.m_currentImageZoom = 1f;
            this.m_rect = new Rect(0f, 0f, 1f, 1f);
            this.UpdateTextureScale();
            this.UpdateTexture(true);
            this.m_cameraPlugin.Stop();
            this.m_image.rectTransform.localEulerAngles = Vector3.zero;

            typeAlbum = TypeAlbum.Create;
            ShowObjectTake(false);
            ShowLoadingDelay();
        }
        catch (Exception ex)
        {
            canvasAllScene.ShowNoti(I2.Loc.ScriptLocalization.Taking_photos_failed.ToUpper());
        }
    }
    private void UpdateTextureScale()
    {
        try
        {
            this.m_currentImageZoom = Mathf.Max(1f, this.m_currentImageZoom);
            this.m_currentImageZoom = Mathf.Min(this.m_currentImageZoom, this.m_maxZoom);
            Vector2 center = this.m_rect.center;
            if (this.m_fullTexture.width > this.m_fullTexture.height)
            {
                float num = (float)this.m_fullTexture.height / (float)this.m_fullTexture.width;
                this.m_rect = new Rect((1f - num) / 2f, 0f, num / this.m_currentImageZoom, 1f / this.m_currentImageZoom);
            }
            else
            {
                float num2 = (float)this.m_fullTexture.width / (float)this.m_fullTexture.height;
                this.m_rect = new Rect(0f, (1f - num2) / 2f, 1f / this.m_currentImageZoom, num2 / this.m_currentImageZoom);
            }
            this.m_rect.center = center;
            this.CheckBorders();
        }
        catch
        {

        }
    }
    private void CheckBorders()
    {
        if (m_rect.min.x < 0f)
        {
            m_rect.center = new Vector2(m_rect.center.x - m_rect.min.x, m_rect.center.y);
        }
        if (m_rect.max.x > 1f)
        {
            m_rect.center = new Vector2(this.m_rect.center.x + 1f - this.m_rect.max.x, this.m_rect.center.y);
        }
        if (m_rect.min.y < 0f)
        {
            m_rect.center = new Vector2(this.m_rect.center.x, this.m_rect.center.y - this.m_rect.min.y);
        }
        if (m_rect.max.y > 1f)
        {
            m_rect.center = new Vector2(this.m_rect.center.x, this.m_rect.center.y + 1f - this.m_rect.max.y);
        }
    }

    public void OnClickGallery()
    {
        SoundClickButton();
        CC_Interface.instance.IsShowingAd = true;
        if (!UnitySingleton<PickerController>.Instance.IsInit())
        {
            PickerController instance = UnitySingleton<PickerController>.Instance;
            instance.InitComplete = (Action<bool>)Delegate.Combine(instance.InitComplete, new Action<bool>(this.AfterInitPicker));
            UnitySingleton<PickerController>.Instance.Subscribe(new Action<Texture2D>(this.GetImageFromGallery), null, null, null);
            UnitySingleton<PickerController>.Instance.Initilized();
        }
        else
        {
            UnitySingleton<PickerController>.Instance.Subscribe(new Action<Texture2D>(this.GetImageFromGallery), null, null, null);
            UnitySingleton<PickerController>.Instance.OpenGallery();
        }
        return;
        NativeGallery.GetImageFromGallery(new NativeGallery.MediaPickCallback((path) =>
        {
            GetImageCompleteWrapper(path);
        }), "Select an image");
        //NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        //{
        //    if (path != null)
        //    {
        //        // Tải ảnh từ đường dẫn và hiển thị lên RawImage
        //        LoadImages(path);
        //    }
        //}, "Select an image", "image/*");

    }
    private void AfterInitPicker(bool isInit)
    {
        PickerController instance = UnitySingleton<PickerController>.Instance;
        instance.InitComplete = (Action<bool>)Delegate.Remove(instance.InitComplete, new Action<bool>(this.AfterInitPicker));
        if (isInit)
        {
            UnitySingleton<PickerController>.Instance.OpenGallery();
        }
    }
    private void GetImageFromGallery(Texture2D texture)
    {
        UnitySingleton<PickerController>.Instance.UnSubscribe(new Action<Texture2D>(this.GetImageFromGallery), null, null, null);
        btnBack.gameObject.SetActive(false);
        m_image.texture = texture;
        m_fullTexture = texture;
        m_image.texture = texture;
        m_image.rectTransform.localEulerAngles = Vector3.zero;
        m_currentImageZoom = 1f;
        UpdateTextureScale();
        UpdateTexture(true);
        m_cameraPlugin.Stop();
        typeAlbum = TypeAlbum.Album;
        ShowObjectTake(false);
        CC_Interface.instance.IsShowingAd = false;
        ShowLoadingDelay();
    }
    private void LoadImages(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // Tự động điều chỉnh kích thước của textureGray.

        m_image.texture = texture;
        m_fullTexture = texture;
        m_image.texture = texture;
        m_image.rectTransform.localEulerAngles = Vector3.zero;
        m_currentImageZoom = 1f;
        //return;
        UpdateTextureScale();
        UpdateTexture(true);
        m_cameraPlugin.Stop();
        typeAlbum = TypeAlbum.Album;
        ShowObjectTake(false);

        CC_Interface.instance.IsShowingAd = false;
    }
    private void GetImageCompleteWrapper(string path)
    {
        StartCoroutine(LoadImage(path));
    }
    private IEnumerator LoadImage(string path)
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("Loading image from gallery " + path);

        var url = path;
        Debug.Log("pathf = " + path);

        if (ActionHelper.IsAndroid())
        {
            if (url == null)
                StopCoroutine(LoadImage(path));
            else
                if (!url.StartsWith("file:"))
                url = "file://" + url;
        }

        var www = new WWW(url);
        yield return www;

        try
        {
            var texture = www.texture;
            if (texture == null)
            {
                Debug.Log("Failed to load textureGray url:" + url);
                this.m_cameraPlugin.Initilized();
            }
            else
            {
                btnBack.gameObject.SetActive(false);
                m_image.texture = texture;
                m_fullTexture = texture;
                m_image.texture = texture;
                m_image.rectTransform.localEulerAngles = Vector3.zero;
                m_currentImageZoom = 1f;
                UpdateTextureScale();
                UpdateTexture(true);
                m_cameraPlugin.Stop();
                typeAlbum = TypeAlbum.Album;
                ShowObjectTake(false);
                CC_Interface.instance.IsShowingAd = false;
                ShowLoadingDelay();
            }
        }
        catch (Exception ex)
        {
            canvasAllScene.ShowNoti(I2.Loc.ScriptLocalization.Image_loading_failed.ToUpper());
        }
    }
    private void ShowLoadingDelay()
    {
        canvasAllScene.objLoading.Show();
        StartCoroutine(ActionHelper.StartAction(() =>
        {
            canvasAllScene.objLoading.Hide();
        }, 0.5f));
    }

    public void OnClickBack()
    {
        SoundClickButton();
        // VariableSystem.IsBackFromDIY = true;
        InitDataGame.instance.LoadData();
        SceneManager.LoadScene(TypeSceneCurrent.HomeScene.ToString());
        base.Hide();
    }

    public void OnClickReTake()
    {
        SoundClickButton();
        btnBack.gameObject.SetActive(true);
        ShowObjectTake(true);
        StartCoroutine(Init());
    }
    public void OnClickUse()
    {
        SoundClickButton();
        Debug.Log("Use shape");
        try
        {
            Texture2D texture2D = (Texture2D)this.m_image.texture;
            int num = (int)Mathf.Lerp(15f, 50f, (this.m_qualitySlider.value - this.m_qualitySlider.minValue) / (this.m_qualitySlider.maxValue - this.m_qualitySlider.minValue));
            Debug.Log("num = " + num);
            if (TextureColoring.CheckToNeedConverColor(texture2D, num))
            {
                //texture2D = TextureColorsReducer.Process(texture2D, num);
                texture2D = ActionHelper.Process(texture2D, num);
            }

            colorList = new List<Color>();
            // Lấy mã màu từ textureGray và lọc ra các màu trùng nhau
            HashSet<Color32> colorSet = new HashSet<Color32>();
            List<int> colorIndexList = new List<int>();

            Color32[] updatedPixels = texture2D.GetPixels32();
            for (int i = 0; i < updatedPixels.Length; i++)
            {
                if (colorSet.Add(updatedPixels[i]))
                    colorList.Add(updatedPixels[i]);
                colorIndexList.Add(colorList.IndexOf(updatedPixels[i]));
            }
            GeneratePixelArt(texture2D, ref listMap);

            // Chuyển textureGray sang màu xám
            Texture2D grayTexture = new Texture2D(texture2D.width, texture2D.height, texture2D.format, false);
            Color32[] originalPixels = texture2D.GetPixels32();
            Color32[] grayPixels = new Color32[originalPixels.Length];

            for (int i = 0; i < originalPixels.Length; i++)
            {
                Color32 pixel = originalPixels[i];
                byte gray = (byte)(0.299f * pixel.r + 0.587f * pixel.g + 0.114f * pixel.b);
                grayPixels[i] = new Color32(gray, gray, gray, pixel.a);
            }

            grayTexture.SetPixels32(grayPixels);
            grayTexture.Apply();
            this.m_image.texture = grayTexture;

            // Lấy mã màu từ textureGray xám và lọc ra các màu trùng nhau
            colorSet = new HashSet<Color32>();
            colorIndexList = new List<int>();

            for (int i = 0; i < grayPixels.Length; i++)
            {
                if (colorSet.Add(grayPixels[i]))
                    colorListGray.Add(grayPixels[i]);
                colorIndexList.Add(colorListGray.IndexOf(grayPixels[i]));
            }

            GeneratePixelArt(grayTexture, ref listMapNoColor);
            DataDIY.CreateNewTexture(texture2D, grayTexture, DataDIY.Count, typeAlbum, colorList, colorListGray, listMap, listMapNoColor);

            DataDIY.Count++;
            //ActionHelper.ShowRewardAds("Rw_DIY_" + DataDIY.Count, Callback);
            Callback(true);
        }
        catch (Exception ex)
        {

        }
    }

    private void Callback(bool isComplete)
    {
        if (!isComplete) return;

        DataDIY.LoadMetadataList();
        DataDIY.SetMetaDataCurrent(DataDIY.Count - 1);
        Texture2D texture2DGray = DataDIY.LoadTextureByID(DataDIY.Count - 1, false);

        gameplayController.texture = texture2DGray;
        gameplayController.typePlayMode = TypePlayMode.DIY;

        if (DataDIY.metadataCurrent.dataSave.listDataCoor.Count == 0)
        {
            int count = 0;
            DataDIY.metadataCurrent.dataSave = new DataSave();
            var listDataCoor = new List<DataCoordinates>();
            for (int y = 0; y < texture2DGray.height; y++)
            {
                var data = new DataCoordinates();

                for (int x = 0; x < texture2DGray.width; x++)
                {
                    var cor = new Coordinates();
                    //   cor.num = -1;
                    cor.isHasColor = false;

                    count++;


                    data.listCoor.Add(cor);
                }
                listDataCoor.Add(data);
            }
            DataDIY.metadataCurrent.dataSave.listDataCoor = listDataCoor;
        }
        gameplayController.LoadLevel(DataDIY.metadataCurrent, texture2DGray);
    }

    void GeneratePixelArt(Texture2D sourceImage, ref List<int> list)
    {
        bool isWriteData = true;
        bool isColor = true;
        float scale = 1f;
        int count = -1;
        Color[] pixels = sourceImage.GetPixels();
        int width = Mathf.FloorToInt(sourceImage.width * scale);
        int height = Mathf.FloorToInt(sourceImage.height * scale);

        int index = 0;
        dicColor = new Dictionary<Color, int>();

        GameObject par = gameObject;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[index];

                if (pixelColor.a < 1)
                {
                    index += Mathf.FloorToInt(1 / 1);
                    continue;
                }

                var color = pixelColor;
                if (!dicColor.ContainsKey(color))
                {
                    count++;
                    dicColor.Add(color, count);
                    if (isWriteData)
                        list.Add(count);
                }
                else
                {
                    if (isWriteData)
                        list.Add(dicColor[color]);
                }
                index += Mathf.FloorToInt(1 / scale);
            }
        }

        isColor = !isColor;
        Debug.Log("Done");
    }
}
