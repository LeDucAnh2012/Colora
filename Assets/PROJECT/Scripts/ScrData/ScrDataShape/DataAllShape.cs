using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum TypeUnlock
{
    None,
    Free,
    Reward,
    DailyQuest
}
public enum TypeTopic
{
    None,       //0
    Trend,        //1
    Animal,       //2
    Cute,       //3
    Flower,       //4
    Food,     //5
    Game,   //6
    Monster,    //7
}
[Serializable]
public class DataBooster
{
    public TypeBooster typeBooster;
    public Sprite sprBooster;
}
[Serializable]
public class DataTopic
{
    public TypeTopic typeTopic;
    public Sprite sprTopic;
    public Sprite sprTopicGray;
}
[Serializable]
public class DataSave
{
    public List<DataCoordinates> listDataCoor = new List<DataCoordinates>();
}
[Serializable]
public class DataCoordinates
{
    public List<Coordinates> listCoor = new List<Coordinates>();
}
[Serializable]
public class Coordinates
{
    //  public int num;
    public bool isHasColor = false;
    //  public Color color;
}
[Serializable]
public class DataPaintingProgress
{
    public List<Vector2> listVecCoordinates = new List<Vector2>();
}

[Serializable]
public class ShapeInfo
{
    [HorizontalGroup("Split_0", Width = 95), HideLabel, PreviewField(120)] public Texture2D textureDefault;

    [Space]
    [HorizontalGroup("Split_0/1")]
    [BoxGroup("Split_0/1/Right")] public Texture2D textureGray;
    [Space]
    [HorizontalGroup("Split_0/1")]
    [BoxGroup("Split_0/1/Right")] public int indexShape;
    [BoxGroup("Split_0/1/Right")] public string nameShape;
    [Space][BoxGroup("Split_0/1/Right")] public TextAsset txtDataDefault;
    [BoxGroup("Split_0/1/Right")] public TextAsset txtDataGray;

    [Space][BoxGroup("Split_0/1/Right")] public int width;
    [BoxGroup("Split_0/1/Right")] public int height;

    [Space][BoxGroup("Split_0/1/Right")] public float scaleInHome;

    [BoxGroup("Split_0/1/Right")] public TypeUnlock typeUnlock;
    [BoxGroup("Split_0/1/Right")] public TypeTopic typeTopic;

    [Space] public List<int> listMap = new List<int>();
    public List<int> listMapNoColor = new List<int>();

    [Space] public List<Color> listColor = new List<Color>();
    public List<Color> listNoColor = new List<Color>();


    /// <summary>
    /// -1: Not In Progress
    /// 0: In Progress
    /// 1: Done
    /// </summary>
    public StateDone StateDone
    {
        get => (StateDone)PlayerPrefs.GetInt("Shape_" + nameShape + "_StateDone", -1);
        set => PlayerPrefs.SetInt("Shape_" + nameShape + "_StateDone", (int)value);
    }
    public bool IsUnlock
    {
        get => PlayerPrefs.GetInt("Shape_" + nameShape + "_IsUnlock") == 1;
        set => PlayerPrefs.SetInt("Shape_" + nameShape + "_IsUnlock", value ? 1 : 0);
    }
    public bool IsUnlockEffect
    {
        get => PlayerPrefs.GetInt("Shape_" + nameShape + "_IsUnlockEffect", 0) == 1;
        set => PlayerPrefs.SetInt("Shape_" + nameShape + "_IsUnlockEffect", value ? 1 : 0);
    }
    public bool IsOnEffect
    {
        get => PlayerPrefs.GetInt("Shape_" + nameShape + "_IsOnEffect", 0) == 1;
        set => PlayerPrefs.SetInt("Shape_" + nameShape + "_IsOnEffect", value ? 1 : 0);
    }
    private DataSave dataSave = null;
    public DataSave DataSave
    {
        get
        {
            string s = PlayerPrefs.GetString("Shape_" + nameShape + "_Data", "null");
            if (s.Equals("null"))
            {
                dataSave = null;
                return null;
            }
            else
                dataSave = JsonUtility.FromJson<DataSave>(s);
            //   dataSave = JsonConvert.DeserializeObject<DataSave>(s);

            return dataSave;
        }
        set
        {
            dataSave = value;
            string s = JsonUtility.ToJson(value);
            //  string s = JsonConvert.SerializeObject(value, Formatting.Indented);
            PlayerPrefs.SetString("Shape_" + nameShape + "_Data", s);
            PlayerPrefs.Save();
        }
    }


    private DataPaintingProgress dataPainting;
    public DataPaintingProgress DataPainting
    {
        get
        {
            string s = PlayerPrefs.GetString("Painting_" + nameShape + "_Data", "null");
            if (s.Equals("null"))
            {
                dataPainting = null;
                return null;
            }
            else
                dataPainting = JsonUtility.FromJson<DataPaintingProgress>(s);
            //   dataPainting = JsonConvert.DeserializeObject<DataPaintingProgress>(s);

            return dataPainting;
        }
        set
        {
            dataPainting = value;
            string s = JsonUtility.ToJson(value);
            // string s = JsonConvert.SerializeObject(value, Formatting.Indented);
            PlayerPrefs.SetString("Painting_" + nameShape + "_Data", s);
            PlayerPrefs.Save();
        }
    }
    public string DataTexture
    {
        get => PlayerPrefs.GetString("Texture_" + nameShape + "_Data", "null");
        set
        {
            PlayerPrefs.SetString("Texture_" + nameShape + "_Data", value);
            PlayerPrefs.Save();
        }
    }
    public int IDFrame
    {
        get => PlayerPrefs.GetInt("IDFrame_" + nameShape + "_Data", -1);
        set
        {
            PlayerPrefs.SetInt("IDFrame_" + nameShape + "_Data", value);
            PlayerPrefs.Save();
        }
    }
    public int IDBackground
    {
        get => PlayerPrefs.GetInt("IDBackground_" + nameShape + "_Data", -1);
        set
        {
            PlayerPrefs.SetInt("IDBackground_" + nameShape + "_Data", value);
            PlayerPrefs.Save();
        }
    }
    public int CountCompleteGift
    {
        get => PlayerPrefs.GetInt("CountCompleteGift_" + nameShape, 0);
        set
        {
            PlayerPrefs.SetInt("CountCompleteGift_" + nameShape, value);
            PlayerPrefs.Save();
        }
    }


    #region Funct

    public void DeleteDataSave()
    {
        PlayerPrefs.DeleteKey("Shape_" + nameShape + "_Data");
        PlayerPrefs.DeleteKey("Painting_" + nameShape + "_Data");
        PlayerPrefs.DeleteKey("Texture_" + nameShape + "_Data");
        PlayerPrefs.DeleteKey("IDFrame_" + nameShape + "_Data");
        PlayerPrefs.DeleteKey("IDBackground_" + nameShape + "_Data");
        PlayerPrefs.DeleteKey("CountCompleteGift_" + nameShape);
        PlayerPrefs.DeleteKey("Shape_" + nameShape + "_StateDone");
        Debug.Log("delete data save");
    }

    #endregion
}

[Serializable]
public class TopicInfo
{
    public TypeTopic typeTopic;
    public List<ShapeInfo> listShapeInfo = new List<ShapeInfo>();
    public TopicInfo(TypeTopic typeTopic, List<ShapeInfo> listShapeInfo)
    {
        this.typeTopic = typeTopic;
        this.listShapeInfo = listShapeInfo;
    }

}
[CreateAssetMenu]
public class DataAllShape : ScriptableObject
{
    private static readonly ResourceAsset<DataAllShape> assets = new ResourceAsset<DataAllShape>("Data/DataAllShape");
    public List<ShapeInfo> listShapeInfo = new List<ShapeInfo>();

    public List<TopicInfo> listTopicInfo = new List<TopicInfo>();

    public List<int> listLevelEffectAds = new List<int>();
    public List<DataBooster> listDataBoosters = new List<DataBooster>();

    public List<Texture2D> listTextureColor2D = new List<Texture2D>();
    public List<Texture2D> listTexture2D = new List<Texture2D>();


    #region Funct
    public static int Count => GetListShapeInfo().Count;
    public static List<ShapeInfo> GetListShapeInfo()
    {
        return assets.Value.listShapeInfo;
    }
    public static ShapeInfo GetShapeInfo(int idLevel)
    {
        foreach (var shapeInfo in GetListShapeInfo())
            if (shapeInfo.indexShape == idLevel)
                return shapeInfo;
        return null;
    }
    public static List<ShapeInfo> GetListShapeInfo(StateDone StateDone)
    {
        var list = new List<ShapeInfo>();
        foreach (var shape in GetListShapeInfo())
            if (shape.StateDone == StateDone)
                list.Add(shape);
        return list;
    }
    public static List<ShapeInfo> GetListShapeNotDone()
    {
        var list = new List<ShapeInfo>();
        foreach (var shape in GetListShapeInfo())
            if (shape.StateDone != StateDone.Done)
            {
                if (shape.typeUnlock != TypeUnlock.DailyQuest)
                    list.Add(shape);
                else
                {
                    if (shape.IsUnlock)
                        list.Add(shape);
                }
            }
        return list;
    }
    public static List<ShapeInfo> GetListShapeInfo(TypeTopic typeTopic)
    {
        var list = new List<ShapeInfo>();
        foreach (var shape in GetListShapeInfo())
            if (shape.typeTopic == typeTopic)
                list.Add(shape);
        return list;
    }
    public static List<ShapeInfo> GetListShapeInfo(TypeUnlock typeUnlock)
    {
        var list = new List<ShapeInfo>();
        foreach (var shape in GetListShapeInfo())
            if (shape.typeUnlock == typeUnlock)
                list.Add(shape);
        return list;
    }
    public static List<ShapeInfo> GetListShapeInfo(TypeUnlock typeUnlock, bool isUnlock)
    {
        var list = new List<ShapeInfo>();
        foreach (var shape in GetListShapeInfo())
            if (shape.typeUnlock == typeUnlock && shape.IsUnlock == isUnlock)
                list.Add(shape);
        return list;
    }
    public static int CountTopic => GetListTopicInfo().Count;
    public static List<TopicInfo> GetListTopicInfo()
    {
        return assets.Value.listTopicInfo;
    }
    public static void SetHasEffect()
    {
        if (PlayerPrefs.GetInt("ConfigSetEffect") == 1) return;
        PlayerPrefs.SetInt("ConfigSetEffect", 1);

        float per = 0.3f;
        var list = new List<ShapeInfo>();

        foreach (var shapeInfo in GetListShapeInfo())
        {
            if (!assets.Value.listLevelEffectAds.Contains(shapeInfo.indexShape))
            {
                shapeInfo.IsUnlockEffect = true;
                list.Add(shapeInfo);
            }
        }

        var count = (int)(list.Count * per);

        while (count > 0)
        {
            var shape = list[UnityEngine.Random.Range(0, list.Count)];
            shape.IsOnEffect = true;
            list.Remove(shape);
            count--;
        }
    }

    public static int GetCountPicDone()
    {
        var count = 0;
        foreach (var shapeInfo in GetListShapeInfo())
            if (shapeInfo.StateDone == StateDone.Done)
                count++;
        return count;
    }

    public static DataBooster GetDataBooster(TypeBooster type)
    {
        foreach (var topic in assets.Value.listDataBoosters)
            if (topic.typeBooster == type)
                return topic;
        return null;
    }
    public static bool CheckShowTab(TypeUnlock typeUnlock, TypeTopic typeTopic)
    {
        var list = GetListShapeInfo(typeTopic);

        foreach (var shape in list)
            if (shape.typeUnlock == typeUnlock && shape.IsUnlock)
                return true;

        return false;
    }
    public static void UnlockAll()
    {
        foreach (var shape in GetListShapeInfo())
        {
            shape.IsUnlock = true;
            shape.IsUnlockEffect = true;
        }
    }
    public static bool CheckUnlockAllShapeDailyQuest()
    {
        return GetListShapeInfo(TypeUnlock.DailyQuest, false).Count == 0;
    }
    #endregion

    //=============================================================================

    [Button]
    private void SetDataTopicInfo()
    {
        listTopicInfo.Clear();
        for (int i = 0; i < Count; i++)
        {
            var shape = GetListShapeInfo()[i];
            var count = 0;

            foreach (var type in listTopicInfo)
            {
                if (type.typeTopic == shape.typeTopic)
                {
                    type.listShapeInfo.Add(shape);
                    break;
                }
                else
                {
                    count++;


                }
            }
            if (count == listTopicInfo.Count)
            {
                var typeInfo = new TopicInfo(shape.typeTopic, new List<ShapeInfo>() { shape });
                listTopicInfo.Add(typeInfo);
            }
        }
    }
    public string level;
    //  public TypeUnlock typeUnlock;
    public TypeTopic typeTopic;

    [Button]
    void SetTypeTopic()
    {
        var list = new List<int>();
        string s = "";
        for (int i = 0; i < level.Length; i++)
        {

            if (level[i] == ',')
            {
                var val = int.Parse(s);
                list.Add(val);
                s = "";
            }
            else
            {
                s += level[i];
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            GetShapeInfo(list[i]).typeTopic = typeTopic;
            Debug.Log("level " + list[i]);
        }
        Debug.Log("DONE");
    }


  //  [Button]
    void LoadListMap()
    {
        int id = 0;
        var listText = Resources.LoadAll<TextAsset>("TextAssets/TextDataShape/").ToList();
        Debug.Log("count= " + listText.Count);
        foreach (var shapeinfo in listText)
        {
            //shapeInfo.textureGray = listtexture2d[id];
            //shapeInfo.idshape = id + 1;

            //shapeInfo.listmap.clear();
            //shapeInfo.listmapnocolor.clear();

            //shapeInfo.listcolor.clear();
            //shapeInfo.listnocolor.clear();

            //shapeInfo.txtdata = resources.load<textasset>("textassets/texdatashape/texcolor/lv_" + getlistshapeinfo()[id].idshape + "_color");
            //shapeInfo.txtdatanocolor = resources.load<textasset>("textassets/texdatashape/texcolor/lv_" + getlistshapeinfo()[id].idshape + "_nocolor");

            //loaddatacolorandshape(shapeInfo.txtdata.tostring().tochararray(), ref shapeInfo.listcolor, ref shapeInfo.listmap);
            //loaddatacolorandshape(shapeInfo.txtdatanocolor.tostring().tochararray(), ref shapeInfo.listnocolor, ref shapeInfo.listmapnocolor);

            //shapeInfo.width = shapeInfo.listmap[0];
            //shapeInfo.height = shapeInfo.listmap[1];

            //shapeInfo.listmap.removeat(0);
            //shapeInfo.listmap.removeat(0);

            //shapeInfo.listmapnocolor.removeat(0);
            //shapeInfo.listmapnocolor.removeat(0);
            //id++;
        }
        Debug.Log("done");
    }

    private int row = 0;
    private int collum = 0;

    private int countRow = 0;
    private int countCollum = 0;
    private string str = "";
    private void LoadDataColorAndShape(char[] arrChar, ref List<Color> color, ref List<int> listMap)
    {
        row = 0;
        collum = 0;

        countRow = 0;
        countCollum = 0;
        str = "";

        color = new List<Color>();
        char key = '\n';
        var listTMP = new List<float>();
        for (int k = 0; k < arrChar.Length; k++)
        {
            if (arrChar[k] == ',')
            {
                if (listMap.Count < 2)
                {
                    int val = int.Parse(str);
                    listMap.Add(val);
                    str = "";

                    if (collum != 0)
                        row = val;

                    if (collum == 0)
                        collum = val;
                }
                else
                {
                    countCollum++;
                    if (countRow == row)
                    {
                        float number = 0;
                        var style = System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowCurrencySymbol;
                        var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

                        if (Single.TryParse(str, style, culture, out number))
                            listTMP.Add(number);
                    }
                    else
                        listMap.Add(int.Parse(str));
                    str = "";
                }
            }
            else
            {
                str += arrChar[k];
                if (countRow == row)
                {
                    if ((arrChar[k] == key && countCollum == 2) || k == arrChar.Length - 1)
                    {
                        float number = 0;
                        var style = System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowCurrencySymbol;
                        var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

                        //if (Single.TryParse(str, style, culture, out number))
                        //    listTMP.Add(number);

                        number = float.Parse(str);
                        listTMP.Add(number);

                        if (listTMP.Count == 3)
                        {
                            var col = new Color(listTMP[0], listTMP[1], listTMP[2], 1);
                            color.Add(col);
                            listTMP.Clear();
                        }
                        countCollum = 0;
                        str = "";
                    }
                }
                else if (arrChar[k] == key && countCollum == collum - 1)
                {
                    int val = int.Parse(str);
                    listMap.Add(val);
                    str = "";
                    countCollum = 0;
                    countRow++;
                }
            }
        }
    }


    [Button]
    void Render()
    {
        isDefault = true;
        shapeInfo = null;
        GetListShapeInfo().Clear();
        for (int i = 0; i < assets.Value.listTextureColor2D.Count / 2; i++)
        {
            var txt = assets.Value.listTextureColor2D;
            GeneratePixelArt(txt[i * 2]);
            GeneratePixelArt(txt[i * 2 + 1]);
        }

    }
    public bool isWriteData = true;
    private void GeneratePixelArt(Texture2D sourceImage)
    {
        int count = -1;
        Color[] pixels = sourceImage.GetPixels();
        int width = Mathf.FloorToInt(sourceImage.width);
        int height = Mathf.FloorToInt(sourceImage.height);

        int index = 0;
        List<Color> colors = new List<Color>();
        colors = new List<Color>();
        Dictionary<Color, int> dicColor = new Dictionary<Color, int>();
        dicColor = new Dictionary<Color, int>();

        string filePath;
        filePath = Application.dataPath + "/PROJECT/Resources/TextAssets/TextDataShape/" + sourceImage.name + ".txt";  // Đường dẫn tới file text trong thư mục persistent data của ứng dụng

        StreamWriter writer = null;// = new StreamWriter(filePathColor);

        if (isWriteData)
        {
            writer = new StreamWriter(filePath);
            writer.WriteLine(width + "," + height + ",");
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[index];

                if (pixelColor.a < 1)
                {
                    if (isWriteData)
                    {
                        if (x == width - 1)
                            writer.Write("-1");
                        else
                            writer.Write("-1,");
                    }
                    //  writer.Write("-1,");
                    index += 1; // Nhảy qua pixel theo tỷ lệ
                    continue;
                }

                var color = pixelColor;// new Color(ClaimFloat(pixelColor.r), ClaimFloat(pixelColor.g), ClaimFloat(pixelColor.b), pixelColor.a);
                if (!dicColor.ContainsKey(color))
                {
                    count++;
                    dicColor.Add(color, count);
                    if (isWriteData)
                    {

                        if (x == width - 1)
                            writer.Write(count);
                        else
                            writer.Write(count + ",");
                    }
                }
                else
                {
                    if (isWriteData)
                    {

                        if (x == width - 1)
                            writer.Write(dicColor[color]);
                        else
                            writer.Write(dicColor[color] + ",");
                    }
                }
                if (!colors.Contains(color))
                    colors.Add(color);

                index += 1; // Nhảy qua pixel theo tỷ lệ
            }
            if (isWriteData)
                writer.Write('\n');
        }

        foreach (var keys in dicColor.Keys)
        {
            var s = keys.r + "," + keys.g + "," + keys.b;
            if (isWriteData)
                writer.Write(s + '\n');
        }
        if (isWriteData)
            writer.Close();


        Debug.Log("Done List map " + sourceImage.name);


    }
    [Button]
    private void AddData()
    {
        GetListShapeInfo().Clear();
        foreach (var sourceImage in listTextureColor2D)
        {

            countShape = GetListShapeInfo().Count;
            if (isDefault)
            {
                isDefault = false;
                shapeInfo = new ShapeInfo();
                shapeInfo.textureDefault = sourceImage;
                shapeInfo.indexShape = countShape++;
                shapeInfo.nameShape = sourceImage.name;

                shapeInfo.listColor = new List<Color>();
                shapeInfo.listMap = new List<int>();
       


                shapeInfo.typeTopic = GetNameTopic(sourceImage);
                shapeInfo.txtDataDefault = Resources.Load<TextAsset>("TextAssets/TextDataShape/" + sourceImage.name);
                LoadDataColorAndShape(shapeInfo.txtDataDefault.ToString().ToCharArray(), ref shapeInfo.listColor, ref shapeInfo.listMap);

                shapeInfo.width = shapeInfo.listMap[0];
                shapeInfo.height = shapeInfo.listMap[1];

                shapeInfo.listMap.RemoveAt(0);
                shapeInfo.listMap.RemoveAt(0);
                shapeInfo.typeUnlock = TypeUnlock.Free;
                GetListShapeInfo().Add(shapeInfo);
                Debug.Log("Done data Color " + sourceImage.name);
            }
            else
            {
                isDefault = true;
                shapeInfo.textureGray = sourceImage;
                shapeInfo.txtDataGray = Resources.Load<TextAsset>("TextAssets/TextDataShape/" + sourceImage.name);


                shapeInfo.listNoColor = new List<Color>();
                shapeInfo.listMapNoColor = new List<int>();

                if (shapeInfo == null)
                {
                    Debug.Log("VCL");
                }
                LoadDataColorAndShape(shapeInfo.txtDataGray.ToString().ToCharArray(), ref shapeInfo.listNoColor, ref shapeInfo.listMapNoColor);
                shapeInfo.listMapNoColor.RemoveAt(0);
                shapeInfo.listMapNoColor.RemoveAt(0);
                Debug.Log("Done data Gray " + sourceImage.name);
            }
        }
    }
    private ShapeInfo shapeInfo = new ShapeInfo();
    private bool isDefault = true;
    public int countShape = 0;

    private bool CheckHasTypeTopic(TypeTopic type)
    {
        foreach (var top in assets.Value.listTopicInfo)
        {
            if (top.typeTopic == type)
            {
                return true;
            }
        }
        return false;
    }
    private TypeTopic GetNameTopic(Texture2D texture)
    {
        var name = texture.name;
        var str = "";
        TypeTopic result = TypeTopic.None;
        for (int i = 0; i < name.Length; i++)
        {

            if (int.TryParse(name[i].ToString(), out _))
            {
                if (Enum.TryParse(str, true, out result))
                    return result;
            }
            else
            {
                str += name[i].ToString();
            }
        }
        Debug.Log("TypeTopic = NONE");
        return result;
    }

}
