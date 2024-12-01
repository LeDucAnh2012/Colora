using System;
using UnityEngine;
using UnityEngine.UI;
public class ConvertTextureString : MonoBehaviour
{
    public Texture2D texture;
    public Image imageToPutTex;
    // Start is called before the first frame update
    void Start()
    {
        string json = ConvertTextureToJson(texture);
        Sprite outputSprite = ConvertTextureJsonToSprite(json);
        Debug.Log(json);
        imageToPutTex.sprite = outputSprite;
    }
    //Convert a textureGray to a string and then store it in Json
    private string ConvertTextureToJson(Texture2D tex)
    {
        string TextureArray = Convert.ToBase64String(tex.EncodeToPNG());
        string jsonOutput = JsonUtility.ToJson(new StoreJson(TextureArray));
        return jsonOutput;
    }
    //Convert a json string to Sprite
    private Sprite ConvertTextureJsonToSprite(string json)
    {
        StoreJson test = JsonUtility.FromJson<StoreJson>(json);
        byte[] b64_bytes = Convert.FromBase64String(test.imageFile);
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(b64_bytes);
        tex.Apply();
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.zero);
        return sprite;
    }
}
/// <summary>
/// Store your Image as a string in this class
/// </summary>
[Serializable]
public class StoreJson
{
    public string imageFile;
    public StoreJson(string imageFile)
    {
        this.imageFile = imageFile;
    }
}
