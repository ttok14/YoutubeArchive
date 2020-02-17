using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorKey
{
    public Color32 color;
    public float time;
}

[System.Serializable]
public class CustomGradient
{
    public enum Layout
    {
        Horizontal,
        Vertical
    }

    public enum BlendMode
    {
        Linear,
        Discrete
    }

    public class TextureProperty
    {
        public int length;
        public Texture2D tex;
    }

    public BlendMode blendMode;

    [SerializeField]
    List<ColorKey> keys = new List<ColorKey>();
    public List<ColorKey> Keys { get { return keys; } }

    Texture2D tex;

    int width, height;

    Dictionary<Layout, List<TextureProperty>> texes = new Dictionary<Layout, List<TextureProperty>>();

    public CustomGradient()
    {
        AddKey(Color.white, 0);
        AddKey(Color.black, 1);
    }

    public Color Evaluate(float time)
    {
        ColorKey keyLeft = keys[0];
        ColorKey keyRight = keys[keys.Count - 1];

        for (int i = 0; i < keys.Count - 1; i++)
        {
            if (keys[i].Time_ <= time)
            {
                keyLeft = keys[i];
            }
            if (keys[i].Time_ >= time)
            {
                keyRight = keys[i];
                break;
            }
        }

        if (blendMode == BlendMode.Linear)
        {
            float blendTime = Mathf.InverseLerp(keyLeft.Time_, keyRight.Time_, time);
            return Color.Lerp(keyLeft.Color_, keyRight.Color_, blendTime);
        }

        return keyRight.Color_;
    }

    public int AddKey(Color color, float time)
    {
        ColorKey newKey = new ColorKey(color, time);

        for (int i = 0; i < keys.Count; i++)
        {
            if (newKey.Time_ < keys[i].Time_)
            {
                keys.Insert(i, newKey);
                return i;
            }
        }

        keys.Add(newKey);
        return keys.Count - 1;
    }

    public void RemoveKey(int index)
    {
        if (keys.Count >= 2)
        {
            keys.RemoveAt(index);
        }
    }

    public int UpdateKeyTime(int index, float time)
    {
        Color col = keys[index].Color_;
        RemoveKey(index);
        return AddKey(col, time);
    }

    public void UpdateKeyColor(int index, Color col)
    {
        keys[index] = new ColorKey(col, keys[index].Time_);
    }

    public int NumKeys
    {
        get
        {
            return keys.Count;
        }
    }

    public ColorKey GetKey(int i)
    {
        return keys[i];
    }

    TextureProperty FindTextureProperty(Layout layout, int length)
    {
        if(texes.ContainsKey(layout) == false)
        {
            return null;
        }

        return texes[layout].Find(t => t.length == length);
    }

    public Texture2D GetTexture(int length, Layout layout)
    {
        var target = FindTextureProperty(layout, length);

        if (target == null)
        {
            int width = layout == Layout.Horizontal ? length : 1;
            int height = layout == Layout.Horizontal ? 1 : length;

            target = new TextureProperty() { length = length, tex = new Texture2D(width, height) };

            if(texes.ContainsKey(layout) == false)
            {
                texes.Add(layout, new List<TextureProperty>());
            }

            texes[layout].Add(target);
        }

        var tex = target.tex;

        var colors = new Color[length];

        if (layout == Layout.Horizontal)
        {
            for (int i = 0; i < length; i++)
            {
                colors[i] = Evaluate((float)i / (length - 1));
            }
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                colors[i] = Evaluate((float)i / (length - 1));
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    [System.Serializable]
    public struct ColorKey
    {
        [SerializeField]
        Color color;
        [SerializeField]
        float time;

        public ColorKey(Color color, float time)
        {
            this.color = color;
            this.time = time;
        }

        public Color Color_
        {
            get
            {
                return color;
            }
        }

        public float Time_
        {
            get
            {
                return time;
            }
        }
    }
}
