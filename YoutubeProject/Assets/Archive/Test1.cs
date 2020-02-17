using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class Test1 : MonoBehaviour
{
    public Vector2 textureSize = new Vector2(16, 16);
    public Image img;

    public Color colLeftTop, colBottom, colRightTop;
    public int interpo;
    private void Start()
    {
        Texture2D tex = new Texture2D((int)textureSize.x, (int)textureSize.y);
        Sprite spr = Sprite.Create(tex, new Rect(0, 0, textureSize.x, textureSize.y), Vector2.zero);
        img.sprite = spr;

        tex.wrapMode = TextureWrapMode.Clamp;

        Color32[] colors = tex.GetPixels32();

        for (int i = 0; i < colors.Length; i++)
        {
            int xPos = i % (int)textureSize.x;
            int yPos = 0;

            yPos = i / (int)textureSize.x;

            var colBottomTop = Color32.Lerp(colBottom, colLeftTop, yPos / textureSize.y);
            colors[i] = Color32.Lerp(colBottomTop, colRightTop, Mathf.Lerp(0f, 1f, xPos / textureSize.x * yPos / textureSize.y));
        }

        tex.SetPixels32(colors);
        tex.Apply();
    }
}