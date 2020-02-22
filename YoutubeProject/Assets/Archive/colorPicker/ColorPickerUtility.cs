using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlendMode
{
    Linear,
    Discrete
}

public enum Layout
{
    Horizontal,
    Vertical
}

public enum SubBlend_Corner
{
    LeftTop, RightTop, LeftBottom, RightBottom, MAX
}

public enum SubBlend_Segment
{
    Left, Top, Right, Bottom, MAX
}

public enum CustomGradientRuntimeOverrideType
{
    None,
    BaseColor,
    SubBlendCorner,
    SubBlendSegment
}

public class ColorPickerUtility
{
    public static Color32 Evaluate(List<ColorKey> keys, float time, BlendMode blendMode)
    {
        ColorKey keyLeft = keys[0];
        ColorKey keyRight = keys[keys.Count - 1];

        for (int i = 0; i < keys.Count - 1; i++)
        {
            if (keys[i].time <= time)
            {
                keyLeft = keys[i];
            }
            if (keys[i].time >= time)
            {
                keyRight = keys[i];
                break;
            }
        }

        if (blendMode == BlendMode.Linear)
        {
            float blendTime = Mathf.InverseLerp(keyLeft.time, keyRight.time, time);
            return Color32.Lerp(keyLeft.color, keyRight.color, blendTime);
        }

        return keyRight.color;
    }

    public static Color32 EvaluateCorner(
        Color32 sourceColor,
        float xTime,
        float yTime,
        Color32? leftTop,
        Color32? rightTop,
        Color32? leftBot,
        Color32? rightBot)
    {
        Color32 result = sourceColor;

        if (leftTop != null &&
            leftTop.HasValue)
        {
            result = Color32.Lerp(result, leftTop.Value, ((1 - xTime) * yTime));
        }

        if (rightTop != null &&
            rightTop.HasValue)
        {
            result = Color32.Lerp(result, rightTop.Value, xTime * yTime);
        }

        if (rightBot != null &&
            rightBot.HasValue)
        {
            result = Color32.Lerp(result, rightBot.Value, xTime * (1 - yTime));
        }

        if (leftBot != null &&
            leftBot.HasValue)
        {
            result = Color32.Lerp(result, leftBot.Value, (1 - xTime) * (1 - yTime));
        }

        return result;
    }

    public static Color32 EvaluateSegment(
        Color32 sourceColor,
        float xTime,
        float yTime,
        Color32? left,
        Color32? top,
        Color32? right,
        Color32? bottom)
    {
        Color32 result = sourceColor;

        if (left != null && left.HasValue)
            result = Color32.Lerp(result, left.Value, 1 - xTime);
        if (top != null && top.HasValue)
            result = Color32.Lerp(result, top.Value, yTime);
        if (right != null && right.HasValue)
            result = Color32.Lerp(result, right.Value, xTime);
        if (bottom != null && bottom.HasValue)
            result = Color32.Lerp(result, bottom.Value, 1 - yTime);

        return result;
    }

    /*
     * 뒤집기전 코드 
     * 
     *  const float INVALID_NUMBER = -1;

    public static Color32 GetBlendedColorOneWay(Color32[] palette, float ratio)
    {
        return ExtractColorTwoWays(palette, null, ratio, INVALID_NUMBER);
    }

    public static Color32 ExtractColorTwoWays(Color32[] basePalette, Color32? subColor, float baseRatio, float subRatio)
    {
        int leftColorId, rightColorId;
        float ratioInBetween;

        GetTwoAdjacentIndexesInArray(basePalette.Length, baseRatio, out leftColorId, out rightColorId, out ratioInBetween);

        Color32 baseColor = GetInterpolatedColor32(basePalette[leftColorId], basePalette[rightColorId], ratioInBetween);

        if (subColor == null ||
            subColor.HasValue == false)
        {
            return baseColor;
        }
        else
        {
            return GetInterpolatedColor32(subColor.Value, baseColor, subRatio);
        }
    }

    public static void GetTwoAdjacentIndexesInArray(int arrayCount, float ratio, out int left, out int right, out float subRatio)
    {
        arrayCount--;

        left = right = 0;
        subRatio = 0;

        float posInArray = arrayCount * ratio;

        left = (int)posInArray;
        right = left + 1;

        if (right > arrayCount)
        {
            right = arrayCount;
        }

        subRatio = posInArray - left;
    }

    public static Color GetInterpolatedColor(Color sour, Color dest, float ratio)
    {
        return Color.Lerp(sour, dest, ratio);
    }

    public static Color32 GetInterpolatedColor32(Color32 sour, Color32 dest, float ratio)
    {
        return Color32.Lerp(sour, dest, ratio);
    }

    public static Vector2 ScreenPosToUI(RectTransform transformer, Vector2 pos)
    {
        return transformer.InverseTransformPoint(pos);
    }

    */
}
 