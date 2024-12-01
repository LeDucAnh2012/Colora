using UnityEngine;
using System.IO;
using System.Collections.Generic;
public enum TypeAlbum
{
    Create,
    Album
}
[System.Serializable]
public class TextureMetadata
{
    public int textureID;
    public string fileName;
    public string filePathColor;
    public string filePath_NoColor;
    public StateDone stateDone;

    public int idBackground;
    public int idFrame;
    public int countGift;
    public bool isOnEffect;
    public TypeAlbum typeAlbum;
    public List<Color> listColor = new List<Color>();
    public List<Color> listNoColor = new List<Color>();
    public List<int> listMap = new List<int>();
    public List<int> listMapNoColor = new List<int>();
    public DataPaintingProgress dataPaintingProgress;
    public DataSave dataSave;
}

[System.Serializable]
public class TextureMetadataList
{
    public List<TextureMetadata> textures = new List<TextureMetadata>();
}

public static class DataDIY
{
    public static TextureMetadataList metadataList = new TextureMetadataList();
    public static TextureMetadata metadataCurrent;

    public static int Count
    {
        get => PlayerPrefs.GetInt("DataDIY_Count_", 0);
        set
        {
            PlayerPrefs.SetInt("DataDIY_Count_", value);
            PlayerPrefs.Save();
        }
    }
    public static int TextureCurrent
    {
        get => PlayerPrefs.GetInt("DataDIY_TextureCurrent_", 0);
        set
        {
            PlayerPrefs.SetInt("DataDIY_TextureCurrent_", value);
            PlayerPrefs.Save();
        }
    }
    public static TextureMetadataList LoadMetadataList()
    {
        string metadataPath = Path.Combine(Application.persistentDataPath, "metadata.json");
        Debug.Log("meta data = " + metadataPath);

        if (File.Exists(metadataPath))
        {
            string metadataJson = File.ReadAllText(metadataPath);
            metadataList = JsonUtility.FromJson<TextureMetadataList>(metadataJson);
            Count = metadataList.textures.Count;
            return metadataList;
        }
        else
        {
            Debug.Log("file not exists");
        }
        Count = 0;
        return new TextureMetadataList();
    }
    public static void CreateNewTexture(Texture2D texture, Texture2D textureGray, int textureID, TypeAlbum typeAlbum, List<Color> listColor, List<Color> listNoColor, List<int> listMap, List<int> listMapNoColor)
    {
        // Đặt tên file và đường dẫn
        string fileNameColor = $"texture_{textureID}.png";
        string filePathColor = Path.Combine(Application.persistentDataPath, fileNameColor);

        // Lưu textureGray dưới dạng PNG
        byte[] bytesColor = texture.EncodeToPNG();
        File.WriteAllBytes(filePathColor, bytesColor);

        string fileNameNoColor = $"texture_{textureID}_NoColor.png";
        string filePathNoColor = Path.Combine(Application.persistentDataPath, fileNameNoColor);
        byte[] bytesNoColor = textureGray.EncodeToPNG();
        File.WriteAllBytes(filePathNoColor, bytesNoColor);

        Debug.Log("path = " + filePathColor);
        // Tạo metadata mới và thêm vào danh sách
        TextureMetadata metadata = new TextureMetadata
        {
            textureID = textureID,
            fileName = fileNameColor,
            filePathColor = filePathColor,
            filePath_NoColor = filePathNoColor,

            stateDone = StateDone.InProgress,
            idBackground = -1,
            idFrame = -1,
            countGift = 0,
            isOnEffect = false,
            typeAlbum = typeAlbum,
            listColor = listColor,
            listNoColor = listNoColor,
            listMap = listMap,
            listMapNoColor = listMapNoColor,
            dataPaintingProgress = new DataPaintingProgress(),
            dataSave = new DataSave(),
        };

        metadataList.textures.Add(metadata);
        metadataCurrent = metadata;
        // Ghi lại metadata vào file JSON
        string metadataJson = JsonUtility.ToJson(metadataList, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "metadata.json"), metadataJson);
    }
    // Phương thức để lưu textureGray đã chỉnh sửa dưới dạng PNG
    public static void SaveEditedTexture(Texture2D texture, int textureID)
    {
        // Tìm metadata của textureGray cũ
        TextureMetadata metadata = metadataList.textures.Find(t => t.textureID == textureID);
        if (metadata != null)
        {
            // Đặt đường dẫn file cũ
            string filePath = metadata.filePath_NoColor;

            // Lưu textureGray đã chỉnh sửa dưới dạng PNG
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Edited textureGray saved to {filePath}");

            // Cập nhật idBackground và ghi lại metadata vào file JSON
            string metadataJson = JsonUtility.ToJson(metadataList, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "metadata.json"), metadataJson);
        }
        else
        {
            Debug.LogWarning($"Texture with ID {textureID} not found.");
        }
    }
    public static void SaveMetadata(int textureID, int idBackground, int idFrame)
    {

        // Tạo hoặc cập nhật metadata
        TextureMetadata metadata = metadataList.textures.Find(t => t.textureID == textureID);
        metadataCurrent = metadata;
        if (metadata == null)
        {
            Debug.LogError("meta data null");
        }
        else
        {
            metadata.idBackground = idBackground;
            metadata.idFrame = idFrame;
        }

        // Ghi lại metadata vào file JSON
        string metadataJson = JsonUtility.ToJson(metadataList, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "metadata.json"), metadataJson);
        Debug.Log($"Metadata saved for textureGray {textureID}");
    }
    public static void SaveMetadata(int textureID, StateDone stateDone, int countGift, DataPaintingProgress dataPaintingProgress,DataSave dataSave)
    {

        // Tạo hoặc cập nhật metadata
        TextureMetadata metadata = metadataList.textures.Find(t => t.textureID == textureID);
        metadataCurrent = metadata;
        if (metadata == null)
        {
            Debug.LogError("meta data null");
        }
        else
        {
            metadata.stateDone = stateDone;
            // metadata.countGift = countGift;
            metadata.dataPaintingProgress = dataPaintingProgress;
            metadata.dataSave = dataSave;
        }

        // Ghi lại metadata vào file JSON
        string metadataJson = JsonUtility.ToJson(metadataList, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "metadata.json"), metadataJson);
        Debug.Log($"Metadata saved for textureGray {textureID}");
    }

    public static Texture2D LoadTextureByID(int textureID, bool isColor)
    {
        TextureMetadata metadata = metadataList.textures.Find(t => t.textureID == textureID);
        if (metadata != null)
        {
            string filePath = "";
            if (isColor)
                filePath = metadata.filePathColor;
            else
                filePath = metadata.filePath_NoColor;

            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(2, 2); // Tạo một Texture2D trống
                texture.LoadImage(fileData); // Load dữ liệu hình ảnh từ file PNG
                Debug.Log("load textureGray");
                return texture;
            }
            else
            {
                Debug.LogWarning($"File for textureGray ID {textureID} not found at path {filePath}");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"Metadata for textureGray ID {textureID} not found.");
            return null;
        }
    }
    public static void SetMetaDataCurrent(int textureID)
    {
        metadataCurrent = metadataList.textures.Find(t => t.textureID == textureID);
    }
    #region Funct Set
    private static void Save()
    {
        string metadataJson = JsonUtility.ToJson(metadataList, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "metadata.json"), metadataJson);
    }

    public static void SetCountGift(int textureID, int countGift)
    {
        metadataCurrent.countGift = countGift;
        Save();
    }
    public static void SetStateDone(StateDone stateDone)
    {
        metadataCurrent.stateDone = stateDone;
        Save();
    }
    public static void SetIsOnEffect(bool isOn)
    {
        metadataCurrent.isOnEffect = isOn;
        Save();
    }
    #endregion

}
