using UnityEngine;
using System.Collections;
using System.Security.Policy;
using UnityEngine.SceneManagement;

public class PixelArtConverter : MonoBehaviour
{
    public GameObject blockPrefab; // Prefab của khối
    public GameObject obj;
    public Vector2 blockSize = new Vector2(1f, 1f); // Kích thước của mỗi khối
    public Vector2 spacing = new Vector2(0.1f, 0.1f); // Khoảng cách giữa các khối
    public float scale = 1f; // Tỉ lệ giảm

    void Start()
    {
        Load();
    }
    public void Load()
    {
        StartCoroutine(LoadMetaData());
    }
    private IEnumerator LoadMetaData()
    {
        DataDIY.LoadMetadataList();
        yield return new WaitForEndOfFrame();
        var list = DataDIY.metadataList.textures;
        yield return new WaitForEndOfFrame();
        DataDIY.SetMetaDataCurrent(0);
        yield return new WaitForEndOfFrame();
        Texture2D texture2D = DataDIY.LoadTextureByID(list[0].textureID, true);
        yield return new WaitForEndOfFrame();
        Texture2D texture2DGray = DataDIY.LoadTextureByID(list[0].textureID, false);
        yield return new WaitForEndOfFrame();
        GeneratePixelArt(texture2D);
        yield return new WaitForEndOfFrame();
        GeneratePixelArt(texture2DGray);
    }
    bool isSetPos = false;
    void GeneratePixelArt(Texture2D sourceImage)
    {
        Color[] pixels = sourceImage.GetPixels();
        int width = Mathf.FloorToInt(sourceImage.width * scale);
        int height = Mathf.FloorToInt(sourceImage.height * scale);

        int index = 0;


        GameObject par = gameObject;
        par = Instantiate(obj, new Vector2(!isSetPos ? -30 : 30, 0), Quaternion.identity, transform);
        isSetPos = true;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = pixels[index];

                if (pixelColor.a < 1)
                {
                    index += Mathf.FloorToInt(1 / scale); // Nhảy qua pixel theo tỷ lệ
                    continue;
                }
                GameObject block = Instantiate(blockPrefab, new Vector3(x * (blockSize.x + spacing.x), y * (blockSize.y + spacing.y), 0), Quaternion.identity, par.transform);
                block.transform.localPosition = new Vector3(x * (blockSize.x + spacing.x), y * (blockSize.y + spacing.y), 0);
                block.transform.localScale = new Vector3(scale, scale, scale); // Đặt tỷ lệ của khối
                block.GetComponent<SpriteRenderer>().color = pixelColor;

                index += Mathf.FloorToInt(1 / scale); // Nhảy qua pixel theo tỷ lệ
            }
        }

        Debug.Log("Done");
    }
    public void Reload()
    {
        SceneManager.LoadSceneAsync(TypeSceneCurrent.BeginScene.ToString());
    }
}

