using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ReadWrite : MonoBehaviour
{
    // Đường dẫn đến file (trong thư mục PersistentDataPath)
    private string filePath;
    public List<Texture2D> listSprShpae = new List<Texture2D>();
    public List<Texture2D> listSprShpaeColor = new List<Texture2D>();

    public RawImage rawSpawn;
    public RectTransform parent;
    public DataShape dataShape;
    void Start()
    {
        // Đặt đường dẫn file vào thư mục PersistentDataPath
        filePath = Path.Combine(Application.persistentDataPath, "example.txt");

        // Đọc dữ liệu từ file
        string content = ReadFromFile();
        Debug.Log("File content: " + content);
        WriteToFile();
        SpawnRaw();

    }
    private void SpawnRaw()
    {
        string s = ReadFromFile();
        dataShape = JsonUtility.FromJson<DataShape>(s);

        for (int i = 0; i < dataShape.data.Count; i++)
        {
            var raw = Instantiate(rawSpawn, parent);
            raw.texture = StringToTexture(dataShape.data[i].txtTextureDefault);

            var raw1 = Instantiate(rawSpawn, parent);
            raw1.texture = StringToTexture(dataShape.data[i].txtTextureGray);
        }
    }

    string ReadFromFile()
    {
        // Kiểm tra xem file có tồn tại không trước khi đọc
        if (File.Exists(filePath))
        {
            // Sử dụng StreamReader để đọc dữ liệu từ file
            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
            return null;
        }
    }
    void WriteToFile()
    {
        var dataShape = new DataShape();

        for (int i = 0; i < listSprShpae.Count; i++)
        {
            var data = new Data();
            data.id = i;
            data.name = listSprShpae[i].name;
            data.txtTextureGray = TextureToString(listSprShpae[i]);
            data.txtTextureDefault = TextureToString(listSprShpaeColor[i]);
            dataShape.data.Add(data);

        }
        string content = JsonUtility.ToJson(dataShape);

        // Sử dụng StreamWriter để ghi dữ liệu vào file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(content);
        }

        Debug.Log("File written to: " + filePath);
    }
    public string TextureToString(Texture2D texture)
    {
        byte[] textureBytes = texture.EncodeToPNG();
        return System.Convert.ToBase64String(textureBytes);
    }

    public Texture2D StringToTexture(string base64String)
    {
        byte[] textureBytes = System.Convert.FromBase64String(base64String);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(textureBytes);
        return texture;
    }
}
[System.Serializable]
public class Data
{
    public int id;
    public string name;
    public TypeTopic typeTopic;
    public string txtTextureGray;
    public string txtTextureDefault;
}
[System.Serializable]
public class DataShape
{
    public List<Data> data = new List<Data>();
}