using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(CustomGradient))]
//public class GradientDrawer : PropertyDrawer
//{
//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return base.GetPropertyHeight(property, label);
//    }

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        Event guiEvent = Event.current;

//        CustomGradient gradient = (CustomGradient)fieldInfo.GetValue(property.serializedObject.targetObject);

//        float labelWidth = GUI.skin.label.CalcSize(label).x + 5;
//        Rect textureRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

//        if (guiEvent.type == EventType.Repaint)
//        {
//            GUI.Label(position, label);
//            GUIStyle gradientStyle = new GUIStyle();
//            gradientStyle.normal.background = gradient.GetTexture((int)position.width);
//            GUI.Label(textureRect, GUIContent.none, gradientStyle);
//        }
//        else
//        {
//            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
//            {
//                if (textureRect.Contains(guiEvent.mousePosition))
//                {
//                    var gw = EditorWindow.GetWindow<GradientWindow>();
//                    gw.SetGradient(gradient);
//                }
//            }
//        }
//    }
//}

[CustomEditor(typeof(CustomGradientEntity))]
public class GradientDrawer : Editor
{
    GradientWindow window;

    bool colorOverrideFolded;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CustomGradientEntity instance = (CustomGradientEntity)target;

        EditorGUI.BeginChangeCheck();
        instance.gradient.layout = (Layout)EditorGUILayout.EnumPopup(instance.gradient.layout);// (CustomGradient.Layout)serializedObject.FindProperty("layout").enumValueIndex;

        if (EditorGUI.EndChangeCheck())
        {
            if (GradientWindow.isOpen)
            {
                window.Repaint();
            }
        }

        CustomGradient gradient = ((CustomGradientEntity)target).gradient;

        EditorGUILayout.LabelField("Gradient");

        GUIStyle gradientStyle = new GUIStyle();
        gradientStyle.normal.background = gradient.GetTexture(4, 4, instance.gradient.layout);
        EditorGUILayout.LabelField(GUIContent.none, gradientStyle);

        var guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (GUILayoutUtility.GetLastRect().Contains(guiEvent.mousePosition))
            {
                window = EditorWindow.GetWindow<GradientWindow>();
                window.SetGradient(gradient);
                gradient.UpdateTexture();
            }
        }

        // draw override option 

        GUILayout.Space(10);

        colorOverrideFolded = EditorGUILayout.Foldout(colorOverrideFolded, "Color Override");

        if (colorOverrideFolded == false)
        {
            EditorGUI.indentLevel++;

            instance.overrider.type = (CustomGradientRuntimeOverrideType)EditorGUILayout.EnumPopup("Type", instance.overrider.type);

            if (instance.overrider.type != CustomGradientRuntimeOverrideType.None)
            {
                switch (instance.overrider.type)
                {
                    case CustomGradientRuntimeOverrideType.BaseColor:
                        instance.overrider.baseColorIndex = EditorGUILayout.IntField("KeyIndex", instance.overrider.baseColorIndex);

                        if (instance.overrider.baseColorIndex >= gradient.NumKeys)
                        {
                            EditorGUILayout.HelpBox("Override Index is out of range, current colors count : " + gradient.NumKeys + ", current configured : " + instance.overrider.baseColorIndex, MessageType.Error);
                        }else if(instance.overrider.baseColorIndex < 0)
                        {
                            instance.overrider.baseColorIndex = 0;
                        }
                        break;
                    case CustomGradientRuntimeOverrideType.SubBlendCorner:
                        instance.overrider.targetCorner = (SubBlend_Corner)EditorGUILayout.EnumFlagsField("TargetCorner", instance.overrider.targetCorner);
                        EditorGUILayout.HelpBox("Only works when the original gradient's subBlendOption is Corner and is used", MessageType.Info);
                        break;
                    case CustomGradientRuntimeOverrideType.SubBlendSegment:
                        instance.overrider.targetSegment = (SubBlend_Segment)EditorGUILayout.EnumFlagsField("TargetSegment", instance.overrider.targetSegment);
                        break;
                    default:
                        break;
                }
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(20);
        }

        if (GUILayout.Button("Apply To Image"))
        {
            var img = ((CustomGradientEntity)target).imgTarget;

            if (img != null)
            {
                Sprite sprite = Sprite.Create(gradientStyle.normal.background, new Rect(0, 0, gradientStyle.normal.background.width, gradientStyle.normal.background.height), Vector2.zero);
                img.sprite = sprite;
            }
            else
            {
                Debug.LogError("TargetImage Is NULL");
            }
        }
    }
}