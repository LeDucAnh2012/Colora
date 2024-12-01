using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class ImageColorExtractor : MonoBehaviour
{
    public GameObject blockPrefab; // Prefab của khối

    public Vector2 blockSize = new Vector2(1f, 1f); // Kích thước của mỗi khối
    public Vector2 spacing = new Vector2(0.1f, 0.1f); // Khoảng cách giữa các khối
    public float scale = 1f; // Tỉ lệ giảm

    [ShowInInspector] public Dictionary<Color, int> dicColor = new Dictionary<Color, int>();
    public List<Color> colors = new List<Color>();
    public bool isWriteData = false;

    public List<Texture2D> listTexture = new List<Texture2D>();
    public List<int> listLvUpdate = new List<int>();
    public bool isInstanShape = true;
    public GameObject obj;
    void Start()
    {
        //if (sourceImage == null || blockPrefab == null)
        //{
        //    Debug.LogError("Missing source image or block prefab!");
        //    return;
        //}
        //if (listLvUpdate.Count != 0)
        //{
        //    for (int i = 0; i < listLvUpdate.Count; i++)
        //    {
        //        GeneratePixelArt(listTexture[i * 2], listLvUpdate[i]);
        //        GeneratePixelArt(listTexture[i * 2 + 1], listLvUpdate[i]);
        //    }
        //}
        //else
        //    for (int i = 0; i < listTexture.Count / 2; i++)
        //    {
        //        GeneratePixelArt(listTexture[i * 2], i + 1);
        //        GeneratePixelArt(listTexture[i * 2 + 1], i + 1);
        //    }
        for (int i = 0; i < listTexture.Count; i++)
        {
            Debug.Log("name topic = " + GetNameTopic(listTexture[i]));
        }
    }
    [Button]
    void abc()
    {
        for (int i = 0; i < listTexture.Count; i++)
        {
            Debug.Log("name topic = " + GetNameTopic(listTexture[i]));
        }
    }
    [Button]
    void Render()
    {
        if (   blockPrefab == null)
        {
            Debug.LogError("Missing source image or block prefab!");
            return;
        }
        if (listLvUpdate.Count != 0)
        {
            for (int i = 0; i < listLvUpdate.Count; i++)
            {
                GeneratePixelArt(listTexture[i * 2], listLvUpdate[i]);
                GeneratePixelArt(listTexture[i * 2 + 1], listLvUpdate[i]);
            }
        }
        else
            for (int i = 0; i < listTexture.Count / 2; i++)
            {
                GeneratePixelArt(listTexture[i * 2], i + 1);
                GeneratePixelArt(listTexture[i * 2 + 1], i + 1);
            }
    }
    [Button]
    void Clear()
    {
        ActionHelper.Clear(transform);
    }
    private TypeTopic GetNameTopic(Texture2D texture)
    {
        var name = texture.name;
        var str = "";
        TypeTopic result = TypeTopic.None;
        for(int i = 0; i < name.Length; i++)
        {
            
            if (int.TryParse(name[i].ToString(),out _))
            {
                if( Enum.TryParse(str, true, out result))
                    return result;
            }
            else
            {
                str+= name[i].ToString();
            }
        }
        return result;
    }
    private void GeneratePixelArt(Texture2D sourceImage, int indexLevel)
    {
        int count = -1;
        Color[] pixels = sourceImage.GetPixels();
        int width = Mathf.FloorToInt(sourceImage.width * scale);
        int height = Mathf.FloorToInt(sourceImage.height * scale);

        int index = 0;
        colors = new List<Color>();
        dicColor = new Dictionary<Color, int>();

        string filePath;
            filePath = Application.dataPath + "/PROJECT/Resources/TextAssets/TexDataShape/" + sourceImage.name + ".txt";  // Đường dẫn tới file text trong thư mục persistent data của ứng dụng

        StreamWriter writer = null;// = new StreamWriter(filePathColor);

        if (isWriteData)
        {
            writer = new StreamWriter(filePath);
            writer.WriteLine(width + "," + height + ",");
        }

        GameObject par = gameObject;
        if (isInstanShape)
            par = Instantiate(obj, transform);

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
                    index += Mathf.FloorToInt(1 / scale); // Nhảy qua pixel theo tỷ lệ
                    continue;
                }
                if (isInstanShape)
                {
                    GameObject block = Instantiate(blockPrefab, new Vector3(x * (blockSize.x + spacing.x), y * (blockSize.y + spacing.y), 0), Quaternion.identity, par.transform);
                    block.transform.localScale = new Vector3(scale, scale, scale); // Đặt tỷ lệ của khối
                    block.GetComponent<SpriteRenderer>().color = pixelColor;
                }

                //var color = new Color(ClaimFloat(pixelColor.r), ClaimFloat(pixelColor.g), ClaimFloat(pixelColor.b), pixelColor.a);
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

                    //writer.Write(count + ",");

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

                    //  writer.Write(count + ",");
                }
                if (!colors.Contains(color))
                    colors.Add(color);

                index += Mathf.FloorToInt(1 / scale); // Nhảy qua pixel theo tỷ lệ
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


        Debug.Log("Done");
    }

    private float ClaimFloat(float val)
    {
        return (float)System.Math.Round(val, 1);
    }
    public int totalShape = 0;
    [Button]
    void SetListLV()
    {
        listLvUpdate.Clear();
        for (int i = 0; i < totalShape; i++)
        {
            listLvUpdate.Add(i + 1);
        }
    }
}
