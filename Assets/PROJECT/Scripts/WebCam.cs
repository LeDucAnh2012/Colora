using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WebCam : MonoBehaviour
{
    public Image inputImage;    // Image để hiển thị ảnh chụp từ camera
    public Image outputImage;   // Image để hiển thị ảnh đã chuyển sang pixel
    public Image cameraImage;   // Image để hiển thị hình ảnh thời gian thực từ camera
    public Slider resolutionSlider;  // Slider để điều chỉnh độ phân giải
    public Button captureButton;    // Button để khởi động camera và lấy ảnh

    private WebCamTexture webCamTexture;
    private bool isCapturing = false; // Biến để kiểm tra xem đang chụp ảnh hay không

    void Start()
    {
        // Đăng ký sự kiện cho Slider và Button
        resolutionSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        captureButton.onClick.AddListener(OnCaptureButtonClicked);

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
    }

    void OnCaptureButtonClicked()
    {
        if (!isCapturing && webCamTexture != null && webCamTexture.isPlaying)
        {
            StartCoroutine(TakePhoto());
        }
    }

    IEnumerator TakePhoto()
    {
        isCapturing = true;
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        Sprite photoSprite = Sprite.Create(photo, new Rect(0, 0, photo.width, photo.height), new Vector2(0.5f, 0.5f));
        inputImage.sprite = photoSprite;

        // Sau khi chụp xong ảnh, cập nhật output image
        UpdateOutputImage();

        isCapturing = false;
    }

    void OnSliderValueChanged()
    {
        if (!isCapturing && inputImage.sprite != null)
        {
            UpdateOutputImage();
        }
    }

    void UpdateOutputImage()
    {
        int resolution = Mathf.FloorToInt(resolutionSlider.value); // Lấy giá trị độ phân giải từ Slider

        StartCoroutine(UpdateOutputImageCoroutine(resolution));
    }

    //IEnumerator UpdateOutputImageCoroutine(int resolution)
    //{
    //    Texture2D originalTexture = inputImage.sprite.textureGray;
    //    int width = (int)inputImage.sprite.rect.width;
    //    int height = (int)inputImage.sprite.rect.height;

    //    // Đảm bảo resolution luôn lớn hơn 0
    //    resolution = Mathf.Max(1, resolution);

    //    // Tạo một textureGray mới với độ phân giải thấp
    //    Texture2D lowResTexture = new Texture2D(resolution, resolution);
    //    Color[] newColors = new Color[resolution * resolution];

    //    // Scale và sao chép pixel từ original textureGray đến lowResTexture
    //    for (int y = 0; y < resolution; y++)
    //    {
    //        for (int x = 0; x < resolution; x++)
    //        {
    //            float newX = (float)x / resolution * width;
    //            float newY = (float)y / resolution * height;
    //            newColors[y * resolution + x] = originalTexture.GetPixelBilinear(newX / width, newY / height);
    //        }
    //    }

    //    lowResTexture.SetPixels(newColors);
    //    lowResTexture.Apply();

    //    // Tạo một textureGray mới với kích thước ban đầu và vẽ textureGray độ phân giải thấp lên đó
    //    Texture2D finalTexture = new Texture2D(width, height);
    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            int lowResX = Mathf.FloorToInt((float)x / width * resolution);
    //            int lowResY = Mathf.FloorToInt((float)y / height * resolution);
    //            finalTexture.SetPixel(x, y, lowResTexture.GetPixel(lowResX, lowResY));
    //        }
    //    }

    //    finalTexture.Apply();

    //    // Đặt textureGray cho output image
    //    outputImage.sprite = Sprite.Create(finalTexture, new Rect(0, 0, finalTexture.width, finalTexture.height), new Vector2(0.5f, 0.5f));
    //    outputImage.SetNativeSize(); // Đảm bảo Image điều chỉnh kích thước phù hợp với Texture

    //    yield return null;
    //}

    IEnumerator UpdateOutputImageCoroutine(int resolution)
    {
        if (inputImage.sprite != null)
        {
            Texture2D originalTexture = inputImage.sprite.texture;
            int width = (int)inputImage.sprite.rect.width;
            int height = (int)inputImage.sprite.rect.height;

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

            // Tạo sprite từ finalTexture
            Sprite outputSprite = Sprite.Create(finalTexture, new Rect(0, 0, finalTexture.width, finalTexture.height), new Vector2(0.5f, 0.5f));

            // Đặt sprite cho outputImage và đảm bảo hiển thị với kích thước pixel thực
            outputImage.sprite = outputSprite;
            outputImage.rectTransform.sizeDelta = new Vector2(finalTexture.width, finalTexture.height);

            yield return null;
        }
    }

}
