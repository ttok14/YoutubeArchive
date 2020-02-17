using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerUtility
{
    const float INVALID_NUMBER = -1;

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

    public static Color32 Evaluate(List<CustomGradient.ColorKey> colorKeys, float time)
    {
        var leftKey = colorKeys[0];
        var rightKey = colorKeys[colorKeys.Count - 1];

        for (int i = 0; i < colorKeys.Count; i++)
        {
            if(colorKeys[i].Time_ <= time)
            {
                leftKey = colorKeys[i];
            }
            if(colorKeys[i].Time_ >= time)
            {
                rightKey = colorKeys[i];
                break;
            }
        }

        float blendTime = Mathf.InverseLerp(leftKey.Time_, rightKey.Time_, time);
        return Color32.Lerp(leftKey.Color_, rightKey.Color_, blendTime);
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
}