using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorKey
{
    public Color32 color;
    public float time;

    public ColorKey(Color c, float t)
    {
        color = c;
        time = t;
    }
}

[System.Serializable]
public class ColorWithExistence
{
    public bool use;
    public Color32? color;
}

public enum SubBlendMethod
{
    None,
    Corner,
    Segment
}

[System.Serializable]
public class CustomGradient
{
    public class TextureProperty
    {
        public int width, height;
        public Texture2D tex;
        public Color32[] colors;

        public TextureProperty(int w, int h, Texture2D tex)
        {
            width = w;
            height = h;
            this.tex = tex;
            colors = new Color32[w * h];
        }
    }

    public BlendMode blendMode;
    public SubBlendMethod subBlendType;

    public Layout layout = Layout.Horizontal;

    [SerializeField]
    List<ColorKey> keys = new List<ColorKey>();

    [SerializeField]
    public List<ColorWithExistence> cornerColors;
    [SerializeField]
    public List<ColorWithExistence> segmentColors;

    public List<ColorKey> Keys { get { return keys; } }

    // Dictionary serialize 가 안대기때문에 
    // 게임 시작시 데이터 다 날아감 하지만 애는 상관없음 
    // 런타임때 생성해줄애임 . 에디터에서는 에디터용에서 쓸 텍스쳐만 드가는거고 . 
    // 즉 context depend 임 
    Dictionary<Layout, List<TextureProperty>> texes;

    public CustomGradient()
    {
        ResetSetting();
    }

    public Color32 Evaluate(Layout layout, float xTime, float yTime)
    {
        float sourceColorTime = layout == Layout.Horizontal ? xTime : yTime; ;
        Color32 result = EvaluateSourceColor(sourceColorTime);
        result = EvaluateSubBlend(result, xTime, yTime);
        return result;
    }

    public Color32 Evaluate(float xTime, float yTime)
    {
        return Evaluate(layout, xTime, yTime);
    }

    Color32 EvaluateSourceColor(float time)
    {
        if (keys.Count == 0)
            ResetSetting();

        Color32 result = ColorPickerUtility.Evaluate(keys, time, blendMode);
        return result;
    }

    Color32 EvaluateSubBlend(Color32 sourceColor, float xTime, float yTime)
    {
        Color32 result = sourceColor;

        switch (subBlendType)
        {
            case SubBlendMethod.Corner:
                {
                    bool use =
                        IsUsingCornerBlending(SubBlend_Corner.LeftTop) ||
                        IsUsingCornerBlending(SubBlend_Corner.RightTop) ||
                        IsUsingCornerBlending(SubBlend_Corner.LeftBottom) ||
                        IsUsingCornerBlending(SubBlend_Corner.RightBottom);

                    if (use)
                    {
                        result = ColorPickerUtility.EvaluateCorner(result, xTime, yTime,
                            GetCornerColor(SubBlend_Corner.LeftTop),
                            GetCornerColor(SubBlend_Corner.RightTop),
                            GetCornerColor(SubBlend_Corner.LeftBottom),
                            GetCornerColor(SubBlend_Corner.RightBottom));
                    }
                }
                break;
            case SubBlendMethod.Segment:
                {
                    bool use =
                        IsUsingSegmentBlending(SubBlend_Segment.Left) ||
                        IsUsingSegmentBlending(SubBlend_Segment.Top) ||
                        IsUsingSegmentBlending(SubBlend_Segment.Right) ||
                        IsUsingSegmentBlending(SubBlend_Segment.Bottom);

                    if (use)
                    {
                        result = ColorPickerUtility.EvaluateSegment(result, xTime, yTime,
                            GetSegmentColor(SubBlend_Segment.Left),
                            GetSegmentColor(SubBlend_Segment.Top),
                            GetSegmentColor(SubBlend_Segment.Right),
                            GetSegmentColor(SubBlend_Segment.Bottom));
                    }
                }
                break;
        }

        return result;
    }

    public int AddKey(Color color, float time)
    {
        ColorKey newKey = new ColorKey(color, time);

        for (int i = 0; i < keys.Count; i++)
        {
            if (newKey.time < keys[i].time)
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
        Color col = keys[index].color;
        RemoveKey(index);
        return AddKey(col, time);
    }

    public void UpdateKeyColor(int index, Color col)
    {
        keys[index] = new ColorKey(col, keys[index].time);
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

    TextureProperty AddTexture(Layout layout, int width, int height)
    {
        var target = new TextureProperty(width, height, new Texture2D(width, height));
        target.tex.wrapMode = TextureWrapMode.Clamp;

        if (texes.ContainsKey(layout) == false)
        {
            texes.Add(layout, new List<TextureProperty>());
        }

        texes[layout].Add(target);
        return target;
    }

    TextureProperty FindTextureProperty(Layout layout, int width, int height)
    {
        if (texes.ContainsKey(layout) == false)
        {
            return null;
        }

        return texes[layout].Find(t => t.width == width && t.height == height);
    }

    public Texture2D GetTexture(int width, int height, Layout layout)
    {
        var target = FindTextureProperty(layout, width, height);

        if (target == null)
        {
            target = AddTexture(layout, width, height);
            UpdateTexture();
        }

        return target.tex;
    }

    public Texture2D GetTexture(int width, int height)
    {
        return GetTexture(width, height, layout);
    }

    void UpdateTexture(TextureProperty target, Layout layout)
    {
        int width = target.width;
        int height = target.height;

        if (layout == Layout.Horizontal)
        {
            for (int x = 0; x < width; x++)
            {
                float xTime = (float)x / (width - 1);

                Color32 sourColor = EvaluateSourceColor(xTime);

                for (int y = 0; y < height; y++)
                {
                    float yTime = (float)y / (height - 1);
                    target.colors[x + y * width] = EvaluateSubBlend(sourColor, xTime, yTime);
                }
            }
        }
        else
        {
            for (int y = 0; y < height; y++)
            {
                float yTime = (float)y / (height - 1);
                Color32 sourColor = EvaluateSourceColor(yTime);

                for (int x = 0; x < width; x++)
                {
                    float xTime = (float)x / (width - 1);
                    target.colors[x + y * width] = EvaluateSubBlend(sourColor, xTime, yTime);
                }
            }
        }

        target.tex.SetPixels32(target.colors);
        target.tex.Apply();
    }

    public void UpdateTexture()
    {
        foreach (var tex in texes)
        {
            for (int i = 0; i < tex.Value.Count; i++)
            {
                UpdateTexture(tex.Value[i], tex.Key);
            }
        }
    }

    void SetSubBlend_Corner(SubBlend_Corner corner, bool use, Color32 color)
    {
        var target = cornerColors[(int)corner];
        target.use = use;
        target.color = color;
    }

    void SetSubBlend_Segment(SubBlend_Segment segment, bool use, Color32 color)
    {
        var target = segmentColors[(int)segment];
        target.use = use;
        target.color = color;
    }

    public void ChangeSubBlend_Corner(SubBlend_Corner corner, bool use, Color32 color)
    {
        SetSubBlend_Corner(corner, use, color);
    }

    public void ChangeSubBlend_Segment(SubBlend_Segment segment, bool use, Color32 color)
    {
        SetSubBlend_Segment(segment, use, color);
    }

    public void ChangeSubBlendColor_Corner(SubBlend_Corner corner, Color32 color)
    {
        ChangeSubBlend_Corner(corner, cornerColors[(int)corner].use, color);
    }

    public void ChangeSubBlendColor_Segment(SubBlend_Segment segment, Color32 color)
    {
        ChangeSubBlend_Segment(segment, segmentColors[(int)segment].use, color);
    }

    bool IsUsingCornerBlending(SubBlend_Corner corner)
    {
        return cornerColors[(int)corner].use;
    }

    bool IsUsingSegmentBlending(SubBlend_Segment segment)
    {
        return segmentColors[(int)segment].use;
    }

    Color32? GetCornerColor(SubBlend_Corner corner)
    {
        var t = cornerColors[(int)corner];
        return t.use ? t.color : null;
    }

    Color32? GetSegmentColor(SubBlend_Segment segment)
    {
        var t = segmentColors[(int)segment];
        return t.use ? t.color : null;
    }

    public void InitKeys()
    {
        keys.Clear();

        if (keys.Count == 0)
        {
            AddKey(Color.white, 0);
            AddKey(Color.black, 1);
        }
    }

    public void ResetSetting()
    {
        InitKeys();
        blendMode = BlendMode.Linear;

        if (cornerColors == null)
        {
            cornerColors = new List<ColorWithExistence>();

            for (int i = 0; i < (int)SubBlend_Corner.MAX; i++)
            {
                cornerColors.Add(new ColorWithExistence() { color = Color.black });
            }
        }
        else
        {
            foreach (var t in cornerColors)
            {
                t.use = false;
                t.color = Color.black;
            }
        }

        if (segmentColors == null)
        {
            segmentColors = new List<ColorWithExistence>();

            for (int i = 0; i < (int)SubBlend_Segment.MAX; i++)
            {
                segmentColors.Add(new ColorWithExistence() { color = Color.black });
            }
        }
        else
        {
            foreach (var t in segmentColors)
            {
                t.use = false;
                t.color = Color.black;
            }
        }

        if (texes != null)
            texes.Clear();
        else texes = new Dictionary<Layout, List<TextureProperty>>();
    }
}
