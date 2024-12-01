using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraCaptureHandler : MonoBehaviour
{
    public Image inputImage;    // Image để hiển thị ảnh từ camera
    public Image outputImage;   // Image để hiển thị ảnh đã chuyển sang pixel
    public Image cameraImage;   // Image để hiển thị hình ảnh thời gian thực từ camera
    public Slider resolutionSlider;  // Slider để điều chỉnh độ phân giải

    private WebCamTexture webCamTexture;
    private bool isUpdatingCamera = false;

    void Start()
    {
        // Đăng ký sự kiện cho Slider
        resolutionSlider.onValueChanged.AddListener(UpdateCameraTexture);

        // Bắt đầu camera
        StartCamera();
    }

    void StartCamera()
    {
        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture();
            webCamTexture.Play();
            cameraImage.material.mainTexture = webCamTexture;
        }

        // Bắt đầu coroutine để cập nhật hình ảnh từ camera
        StartCoroutine(UpdateCameraCoroutine());
    }

    IEnumerator UpdateCameraCoroutine()
    {
        while (true)
        {
            if (!isUpdatingCamera)
            {
                isUpdatingCamera = true;

                // Chuyển đổi WebCamTexture thành Texture2D
                Texture2D camTexture = new Texture2D(webCamTexture.width, webCamTexture.height);
                camTexture.SetPixels(webCamTexture.GetPixels());
                camTexture.Apply();

                // Tạo sprite từ Texture2D và đặt cho cameraImage
                Sprite camSprite = Sprite.Create(camTexture, new Rect(0, 0, camTexture.width, camTexture.height), new Vector2(0.5f, 0.5f));
                cameraImage.sprite = camSprite;

                isUpdatingCamera = false;
            }

            yield return null;
        }
    }

    void UpdateCameraTexture(float value)
    {
        // Lấy giá trị độ phân giải từ Slider
        int resolution = Mathf.FloorToInt(value);

        if (webCamTexture != null)
        {
            // Tạo một textureGray mới với độ phân giải mới
            Texture2D texture = ResizeTexture(webCamTexture, resolution, resolution);

            // Tạo sprite từ textureGray và đặt cho inputImage
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            inputImage.sprite = newSprite;

            // Cập nhật hình ảnh output
            UpdateOutputImage();
        }
    }

    void UpdateOutputImage()
    {
        if (inputImage.sprite != null)
        {
            int resolution = Mathf.FloorToInt(resolutionSlider.value); // Lấy giá trị độ phân giải từ Slider

            Texture2D texture = SpriteToPixelTexture(inputImage.sprite, resolution);
            Sprite outputSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Đặt sprite cho outputImage và đảm bảo hiển thị với kích thước pixel thực
            outputImage.sprite = outputSprite;
            outputImage.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
        }
    }

    Texture2D ResizeTexture(WebCamTexture sourceTexture, int targetWidth, int targetHeight)
    {
        RenderTexture renderTexture = new RenderTexture(targetWidth, targetHeight, 24);
        Graphics.Blit(sourceTexture, renderTexture);
        RenderTexture.active = renderTexture;

        Texture2D targetTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        targetTexture.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        targetTexture.Apply();

        RenderTexture.active = null;
        Destroy(renderTexture);

        return targetTexture;
    }

    Texture2D SpriteToPixelTexture(Sprite sprite, int resolution)
    {
        Texture2D originalTexture = sprite.texture;
        int width = (int)sprite.rect.width;
        int height = (int)sprite.rect.height;

        // Đảm bảo resolution luôn lớn hơn 0
        resolution = Mathf.Max(1, resolution);

        // Tạo một textureGray mới với độ phân giải thấp
        Texture2D lowResTexture = new Texture2D(resolution, resolution);
        Color[] newColors = new Color[resolution * resolution];

        // Scale và sao chép pixel từ original textureGray đến lowResTexture
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float newX = (float)x / resolution * width;
                float newY = (float)y / resolution * height;
                newColors[y * resolution + x] = originalTexture.GetPixelBilinear(newX / width, newY / height);
            }
        }

        lowResTexture.SetPixels(newColors);
        lowResTexture.Apply();

        // Tạo một textureGray mới với kích thước ban đầu và vẽ textureGray độ phân giải thấp lên đó
        Texture2D finalTexture = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int lowResX = Mathf.FloorToInt((float)x / width * resolution);
                int lowResY = Mathf.FloorToInt((float)y / height * resolution);
                finalTexture.SetPixel(x, y, lowResTexture.GetPixel(lowResX, lowResY));
            }
        }

        finalTexture.Apply();
        return finalTexture;
    }
}
