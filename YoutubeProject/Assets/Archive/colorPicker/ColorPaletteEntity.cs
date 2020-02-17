using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPaletteEntity : MonoBehaviour
{ 
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    [Serializable]
    public class ColorPaletteProperty
    {
        public Color32[] color;
    }

    public Direction direction;

    public Image imgTarget;

    public Color32[] basePalette;
    public Color32 subPalette;

    public Texture2D texture;

    public int textureWidth, textureHeight;
    float fTextureWidth, fTextureHeight;

    public bool useSubPalette;

    bool setup;

    private void Start()
    {
        if (imgTarget == null)
            imgTarget = GetComponent<Image>();

        if (imgTarget == null)
        {
            Debug.LogError("Image Not Found");
            return;
        }

        if (imgTarget.sprite != null)
        {
            Debug.LogWarning("ColorPalette Image Sprite Must be Empty");
            return;
        }

        if (textureWidth == 0 ||
            textureHeight == 0)
        {
            Debug.LogWarning("ColorPalette Texture Size must larger than zero");
            return;
        }

        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
        imgTarget.sprite = Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), Vector2.zero);

        fTextureWidth = textureWidth;
        fTextureHeight = textureHeight;

//        SetColor(basePalette, )

        setup = true;
    }

    public void SetColor(Color32[] basePalette, Color32? subColor, Direction direction)
    {
        bool hasSubColor = subColor != null && subColor.HasValue;

        var rawData = texture.GetRawTextureData<Color32>();

        if (hasSubColor == false)
        {
            if (direction == Direction.Horizontal)
            {
                for (int x = 0; x < textureWidth; x++)
                {
                    for (int y = 0; y < textureHeight; y++)
                    {

                    }
                }
            }
            else
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    float verRatio = y / fTextureHeight;

                    for (int x = 0; x < textureWidth; x++)
                    {

                    }
                }
            }
        }
        else
        {
            for (int y = 0; y < textureHeight; y++)
            {
                for (int x = 0; x < textureWidth; x++)
                {
                    float horRatio = x / fTextureWidth;
                    float verRatio = y / fTextureHeight;


                }
            }
        }

        texture.Apply();
    }

    float GetRatio(int current, int dividedBy)
    {
        return current / (float)dividedBy;
    }

    private void FixedUpdate()
    {
        if (setup == false)
            return;
    }
}