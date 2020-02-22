using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(CustomGradientOverrider))]
//public class GradientColorOverriderEditor : PropertyDrawer
//{
//    public CustomGradientRuntimeOverrideType type;

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return base.GetPropertyHeight(property, label) + 20;
//    }

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);

//        EditorGUI.indentLevel++;
//        EditorGUI.LabelField(position, label);


//        label = EditorGUI.BeginProperty(position, label, property);

//        position.height = 16;

//        type = (CustomGradientRuntimeOverrideType)EditorGUI.EnumPopup(position, label, type);
//        position.y += 18;

//        EditorGUI.PropertyField(position, property.FindPropertyRelative("name"));
//        position.y += 18;

//      //  EditorGUI.PropertyField(position, property.FindPropertyRelative("position"));
//        position.y += 18;

//        //Im stuck here, how do i change stuff?
//        /*switch(label) {

//        }*/


//        EditorGUI.indentLevel;
//        EditorGUI.EndProperty();
//    }
//}
