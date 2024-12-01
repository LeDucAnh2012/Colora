using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace KTool.UI.BitmapFont
{
    public class NumberFont : MonoBehaviour
    {
        #region Properties
        private const string FORMAT_NAME = "BitmapFont {0}";
        [SerializeField]
        private Image prefabText = null;
        [SerializeField]
        private Sprite[] bitmaps = null;
        [SerializeField]
        private TextAlignment alignment = TextAlignment.Center;
        [SerializeField]
        private int sizeHight = 1;
        [SerializeField]
        private int space = 0;
        [SerializeField]
        private int number = 0;

#if !UNITY_EDITOR
        private List<Image> trashs = null;
#endif
        private List<Image> imgTexts = null;
        private float textWidth = 0;
        public int Number
        {
            set
            {
                number = value;
                RebuildText();
            }
            get
            {
                return number;
            }
        }
        public float TextWidth => textWidth;
        public int TextHight => sizeHight;
        #endregion Properties
    

        private void OnValidate()
        {
            sizeHight = Mathf.Max(0, sizeHight);
            //space = Mathf.Max(0, space);
            number = Mathf.Max(0, number);
        }

        public void RebuildText()
        {
            Sprite[] sprites = GetNumberSprites(number);
            Image[] images = GetImages(sprites);
            RectTransform[] rects = new RectTransform[images.Length];
            Vector2 anchorMin = Vector2.zero,
                anchorMax = Vector2.zero,
                pivot = new Vector2(0f, 0.5f);
            switch (alignment)
            {
                case TextAlignment.Left:
                    anchorMin = anchorMax = new Vector2(0f, 0.5f);
                    break;
                case TextAlignment.Center:
                    anchorMin = anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                case TextAlignment.Right:
                    anchorMin = anchorMax = new Vector2(1f, 0.5f);
                    break;
            }
            //
            textWidth = 0;
            for (int i = 0; i < images.Length; i++)
            {
                Image img = images[i];
                RectTransform imgRect = img.GetComponent<RectTransform>();
                img.sprite = sprites[i];
                img.name = string.Format(FORMAT_NAME, sprites[i].name);
                //
                rects[i] = imgRect;
                imgRect.anchorMin = anchorMin;
                imgRect.anchorMax = anchorMax;
                imgRect.pivot = pivot;
                //
                float sizeWidth = sizeHight / sprites[i].rect.size.y * sprites[i].rect.size.x;
                imgRect.sizeDelta = new Vector2(sizeWidth, sizeHight);
                textWidth += sizeWidth + (i == 0 ? 0 : space);
            }
            //
            UpdatePosition(rects, alignment, textWidth);
        }

        private Sprite[] GetNumberSprites(int number)
        {
            List<Sprite> sprites = new List<Sprite>();
            string numberText = number.ToString();
            for (int i = 0; i < numberText.Length; i++)
            {
                char c = numberText[i];
                int tmp;
                if (int.TryParse(c.ToString(), out tmp))
                    sprites.Add(bitmaps[tmp]);
            }
            return sprites.ToArray();
        }
        private Image[] GetImages(Sprite[] sprites)
        {
            if (imgTexts == null)
                imgTexts = new List<Image>();
            while (imgTexts.Count != sprites.Length)
            {
                if (imgTexts.Count > sprites.Length)
                {
                    Image oldImg = imgTexts[0];
                    imgTexts.RemoveAt(0);
                    Pooling_DestroyImage(oldImg);
                }
                else if (imgTexts.Count < sprites.Length)
                {
                    Image newImg = Pooling_CreateImage();
                    imgTexts.Add(newImg);
                }
            }
            return imgTexts.ToArray();
        }
        private void UpdatePosition(RectTransform[] rects, TextAlignment alignment, float textWidth)
        {
            float startX = 0;
            switch (alignment)
            {
                case TextAlignment.Left:
                    startX = 0;
                    break;
                case TextAlignment.Center:
                    startX = -textWidth / 2;
                    break;
                case TextAlignment.Right:
                    startX = -textWidth;
                    break;
            }
            for (int i = 0; i < rects.Length; i++)
            {
                rects[i].anchoredPosition = new Vector2(startX, 0);
                startX += rects[i].sizeDelta.x + space;
            }
        }

        private Image Pooling_CreateImage()
        {
            Image newImg = null;
#if !UNITY_EDITOR
            if (trashs != null && trashs.Count > 0)
            {
                newImg = trashs[0];
                trashs.RemoveAt(0);
                newImg.gameObject.SetActive(true);
                return newImg;
            }
#endif
            newImg = Instantiate(prefabText);
            newImg.transform.SetParent(transform, false);
            newImg.gameObject.SetActive(true);
            return newImg;
        }
        private void Pooling_DestroyImage(Image oldImg)
        {
#if !UNITY_EDITOR
            if (trashs == null)
                trashs = new List<Image>();
            if (trashs.Count < 10)
            {
                oldImg.gameObject.SetActive(false);
                trashs.Add(oldImg);
                return;
            }
#endif
#if UNITY_EDITOR
            DestroyImmediate(oldImg.gameObject);
#else
            Destroy(oldImg.gameObject);
#endif
        }
    }
}
