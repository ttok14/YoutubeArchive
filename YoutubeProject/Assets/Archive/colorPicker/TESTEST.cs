using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TESTEST : MonoBehaviour
{
    [HideInInspector]
    public CustomGradient myGradient;
    public Image targetImage;

    [HideInInspector]
    public CustomGradient.Layout layout;

    public int texWidthSize, texHeightSize;

    Texture2D tex;

    private void Start()
    {
        tex = new Texture2D(texWidthSize, texHeightSize);
        tex.wrapMode = TextureWrapMode.Clamp;
        targetImage.sprite = Sprite.Create(tex, new Rect(0, 0, texWidthSize, texHeightSize), Vector2.zero);

        SetTexture();
    }

    private void SetTexture()
    {
        Color32[] cols = tex.GetPixels32();

        if (layout == CustomGradient.Layout.Horizontal)
        {
            for (int x = 0; x < texWidthSize; x++)
            {
                Color32 color = ColorPickerUtility.Evaluate(myGradient.Keys, x / (float)texWidthSize);

                for (int y = 0; y < texHeightSize; y++)
                {
                    int index = x + texWidthSize * y;
                    cols[index] = color;
                }
            }
        }
        else
        {
            Color32 col = Color.white;
            int y = 0;

            for (int i = 0; i < cols.Length; i++)
            {
                if ((i % texWidthSize) == 0)
                {
                    col = ColorPickerUtility.Evaluate(myGradient.Keys, y / (float)texHeightSize);
                    y++;
                }

                cols[i] = col;
            }
        }

        tex.SetPixels32(cols);
        tex.Apply();
    }
}
