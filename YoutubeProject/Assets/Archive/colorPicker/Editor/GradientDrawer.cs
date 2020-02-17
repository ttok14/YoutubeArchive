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

[CustomEditor(typeof(TESTEST))]
public class GradientDrawer : Editor
{
    GradientWindow window;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TESTEST instance = (TESTEST)target;

        EditorGUI.BeginChangeCheck();
        instance.layout = (CustomGradient.Layout)EditorGUILayout.EnumPopup(instance.layout);// (CustomGradient.Layout)serializedObject.FindProperty("layout").enumValueIndex;

        if(EditorGUI.EndChangeCheck())
        {
            if(GradientWindow.isOpen)
            {
                window.Repaint();
            }
        }

        CustomGradient gradient = ((TESTEST)target).myGradient;

        EditorGUILayout.LabelField("Gradient");

        GUIStyle gradientStyle = new GUIStyle();
        gradientStyle.normal.background = gradient.GetTexture(100, instance.layout);

        EditorGUILayout.LabelField(GUIContent.none, gradientStyle);

        var guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (GUILayoutUtility.GetLastRect().Contains(guiEvent.mousePosition))
            {
                window = EditorWindow.GetWindow<GradientWindow>();
                window.SetGradient(gradient, GetLayout);
            }
        }

        if (GUILayout.Button("Apply To Image"))
        {
            var img = ((TESTEST)target).targetImage;

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

    CustomGradient.Layout GetLayout()
    {
        return ((TESTEST)target).layout;
    }
}