﻿using UnityEngine;
using System.Collections;

public static class TextureGenerator
{

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
	{
		Texture2D texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colourMap);
		texture.Apply();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(float[] heightMap, int Width, int Height)
	{
		int width = Width;
		int height = Height;

		int index = 0;

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				colourMap[index] = Color.Lerp(Color.black, Color.white, heightMap[x * width + y]);
				index++;
			}
		}

		return TextureFromColourMap(colourMap, width, height);
	}

}