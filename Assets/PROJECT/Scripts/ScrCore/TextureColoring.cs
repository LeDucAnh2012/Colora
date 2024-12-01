using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureColoring
{
	private static int MaxColorCount = 64;

	private static IList<Color> simpleColorPallete;

	public static bool CheckToNeedConverColor(Texture2D texture, int maxColorCount)
	{
		MaxColorCount = maxColorCount;
		Color32[] pixels = texture.GetPixels32();
		List<Color32> source = pixels.Distinct().ToList();
		if (source.Count() > MaxColorCount - 1)
		{
            Debug.Log("CheckToNeedConverColor = true " + source.Count());
			return true;
		}
        Debug.Log("CheckToNeedConverColor = false " + source.Count());
		return false;
	}

	public static bool CompareColor(Color32 c1, Color32 c2)
	{
		if ((int)Math.Sqrt(Math.Pow((double)(c1.r - c2.r), 2.0) + Math.Pow((double)(c1.g - c2.g), 2.0) + Math.Pow((double)(c1.b - c2.b), 2.0)) == 0)
		{
			return true;
		}
		return false;
	}

	public static bool isNeedConvertColor(Texture2D texture)
	{
		IList<Color> list = new List<Color>();
		for (int i = 0; i < texture.width; i++)
		{
			for (int j = 0; j < texture.height; j++)
			{
			}
		}
		return true;
	}
}


