using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class ScreenshotHandler : MonoBehaviour
{
    public int sizeX = 850;
    public int sizeY = 850;
    public Vector2 capturePosition;
    public RenderTexture renderTexture;
    public Camera camShot;
    private string path = "";
  private  Texture2D ToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    private IEnumerator TakeScreenshort(UnityAction callback, bool isShare)
    {
        yield return new WaitForEndOfFrame();
        camShot.gameObject.SetActive(true);
        GameplayUIManager.instance.canvasGameplay.SetActive(false);
        yield return new WaitForEndOfFrame();

        camShot.Render();

        //Texture2D textureGray = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Texture2D texture = ToTexture2D(renderTexture);
        //Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        //textureGray.ReadPixels(rect, 0, 0);
        //textureGray.Apply();
        //Color color = Color.white;
        //var size = GameplayUIManager.instance.sprRenderFrame.sprite.bounds.size;
        //Texture2D text = new Texture2D(860, 860);

        //int row = 0;
        //int colum = 0;
        //bool isSet = false;
        //for (int i = 0; i < textureGray.height; i++)
        //{
        //    for (int j = 0; j < textureGray.width; j++)
        //    {
        //        var col = textureGray.GetPixel(j, i);
        //        if (col != color)
        //        {
        //            isSet = true;
        //            text.SetPixel(row++, colum, col);
        //        }
        //    }
        //    if (isSet)
        //        colum++;
        //}

        string name = "Screenshort_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png";
        string path = Application.dataPath + "/../" + name;
        // PC
        if (ActionHelper.IsEditor())
        {
            byte[] data = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/../" + name, data);
            Destroy(texture);
        }

        // MOBILE
        if (!ActionHelper.IsEditor() && !isShare)
            NativeGallery.SaveImageToGallery(texture, "Downloads", name);


        if (isShare)
        {
            //string filePathColor = Path.Combine(Application.temporaryCachePath, "shared img.png");
            //File.WriteAllBytes(filePathColor, textureGray.EncodeToPNG());
            //new NativeShare().AddFile(filePathColor)
            //.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            //.Share();
        }

        Destroy(texture);
        callback?.Invoke();
        camShot.gameObject.SetActive(false);
        GameplayUIManager.instance.canvasGameplay.SetActive(true);
    }
    public void TakeScreenshot(UnityAction callback)
    {
        StartCoroutine(TakeScreenshort(callback, false));
    }

    //===================================================================================================

    public void ShareImage(UnityAction callback)
    {
        // StartCoroutine(TakeScreenshort(callback, true));
    }

    //private IEnumerator TakeScreenshotAndShare()
    //{
    //    yield return new WaitForEndOfFrame();

    //    Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    //    ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //    ss.Apply();

    //    string filePathColor = Path.Combine(Application.temporaryCachePath, "shared img.png");
    //    File.WriteAllBytes(filePathColor, ss.EncodeToPNG());

    //    // To avoid memory leaks
    //    Destroy(ss);

    //    new NativeShare().AddFile(filePathColor)
    //        .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
    //        .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
    //        .Share();

    //    // Share on WhatsApp only, if installed (Android only)
    //    //if( NativeShare.TargetExists( "com.whatsapp" ) )
    //    //	new NativeShare().AddFile( filePathColor ).AddTarget( "com.whatsapp" ).Share();
    //}


}
