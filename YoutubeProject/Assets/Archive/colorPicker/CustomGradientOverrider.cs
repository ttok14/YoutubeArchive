using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomGradientOverrider
{
    public CustomGradientRuntimeOverrideType type = CustomGradientRuntimeOverrideType.None;
    public int baseColorIndex;
    public SubBlend_Corner targetCorner;
    public SubBlend_Segment targetSegment;

    public void Apply(CustomGradient target, Color color)
    {
        switch (type)
        {
            case CustomGradientRuntimeOverrideType.BaseColor:
                target.UpdateKeyColor(baseColorIndex, color);
                break;
            case CustomGradientRuntimeOverrideType.SubBlendCorner:
                target.ChangeSubBlendColor_Corner(targetCorner, color);
                break;
            case CustomGradientRuntimeOverrideType.SubBlendSegment:
                target.ChangeSubBlendColor_Segment(targetSegment, color);
                break;
            default:
                break;
        }

        target.UpdateTexture();
    }
}
