using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GradientWindow : EditorWindow
{
    public static bool isOpen;

    CustomGradient gradient;
    Func<Layout> layoutGetter;
    int texWidth = 32;
    int texHeight = 32;

    const int borderSize = 10;
    const float keyWidth = 10;
    const float keyHeight = 20;

    Rect gradientPreviewRect;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    int selectedKeyIndex;
    bool needRepaint;

    float curNormalizedPos;

    private void OnGUI()
    {
        Draw();
        HandleInput();

        if (needRepaint)
        {
            needRepaint = false;
            Repaint();
        }
    }

    void Draw()
    {
        gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, position.height * 0.5f);

        GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture(texWidth, texHeight));

        keyRects = new Rect[gradient.NumKeys];

        for (int i = 0; i < gradient.NumKeys; i++)
        {
            ColorKey key = gradient.GetKey(i);
            Rect keyRect = new Rect(
                gradientPreviewRect.x + gradientPreviewRect.width * key.time - keyWidth / 2f,
                gradientPreviewRect.yMax + borderSize,
                keyWidth,
                keyHeight);

            if (i == selectedKeyIndex)
            {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
            }

            EditorGUI.DrawRect(keyRect, key.color);
            keyRects[i] = keyRect;
        }

        Rect settingRect = new Rect(borderSize, keyRects[0].yMax + borderSize, position.width - borderSize * 2, position.height);
        GUILayout.BeginArea(settingRect);

        EditorGUI.BeginChangeCheck();
        curNormalizedPos = EditorGUILayout.FloatField("Position", gradient.GetKey(selectedKeyIndex).time);
        if(EditorGUI.EndChangeCheck())
        {
            float time = Mathf.Clamp(curNormalizedPos, 0f, 1f);
            selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, time);
            gradient.UpdateTexture();
        }

        EditorGUI.BeginChangeCheck();
        Color newColor = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).color);
        if (EditorGUI.EndChangeCheck())
        {
            gradient.UpdateKeyColor(selectedKeyIndex, newColor);
            gradient.UpdateTexture();
        }

        EditorGUI.BeginChangeCheck();
        gradient.blendMode = (BlendMode)EditorGUILayout.EnumPopup("Blend Mode", gradient.blendMode);
        gradient.subBlendType = (SubBlendMethod)EditorGUILayout.EnumPopup("SubBlend", gradient.subBlendType);
        if (EditorGUI.EndChangeCheck())
        {
            gradient.UpdateTexture();
        }

        if (gradient.subBlendType == SubBlendMethod.Corner)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < gradient.cornerColors.Count; i++)
            {
                var t = gradient.cornerColors[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                bool use = EditorGUILayout.Toggle(((SubBlend_Corner)i).ToString(), t.use);
                Color32 color = t.color.Value;

                if (t.use)
                {
                    color = EditorGUILayout.ColorField(t.color.Value);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    gradient.ChangeSubBlend_Corner((SubBlend_Corner)i, use, color);
                    gradient.UpdateTexture();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
        else if (gradient.subBlendType == SubBlendMethod.Segment)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < gradient.segmentColors.Count; i++)
            {
                var t = gradient.segmentColors[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                bool use = EditorGUILayout.Toggle(((SubBlend_Segment)i).ToString(), t.use);
                Color32 color = t.color.Value;

                if (t.use)
                {
                    color = EditorGUILayout.ColorField(t.color.Value);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    gradient.ChangeSubBlend_Segment((SubBlend_Segment)i, use, color);
                    gradient.UpdateTexture();
                }

                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reset"))
        {
            gradient.ResetSetting();
            gradient.UpdateTexture();
        }
        GUILayout.EndArea();
    }

    private void HandleInput()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseDown &&
            guiEvent.button == 0)
        {
            for (int i = 0; i < keyRects.Length; i++)
            {
                if (keyRects[i].Contains(guiEvent.mousePosition))
                {
                    mouseIsDownOverKey = true;
                    selectedKeyIndex = i;
                    needRepaint = true;
                    break;
                }
            }

            Rect addKeyRect = new Rect(borderSize, gradientPreviewRect.yMax + borderSize, position.width - borderSize * 2, keyHeight);

            if (mouseIsDownOverKey == false && addKeyRect.Contains(guiEvent.mousePosition))
            {
                Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);

                selectedKeyIndex = gradient.AddKey(randomColor, keyTime);
                gradient.UpdateTexture();
                mouseIsDownOverKey = true;
                needRepaint = true;
            }
        }

        if (guiEvent.type == EventType.MouseUp &&
            guiEvent.button == 1)
        {
            for (int i = 0; i < keyRects.Length; i++)
            {
                if (keyRects[i].Contains(guiEvent.mousePosition))
                {
                    gradient.RemoveKey(i);
                    gradient.UpdateTexture();

                    if (selectedKeyIndex >= i)
                    {
                        selectedKeyIndex--;
                        selectedKeyIndex = Mathf.Clamp(selectedKeyIndex, 0, selectedKeyIndex);
                    }

                    needRepaint = true;
                    break;
                }
            }
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            mouseIsDownOverKey = false;
        }

        if (mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
        {
            float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
            selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
            gradient.UpdateTexture();
            needRepaint = true;
        }

        if (guiEvent.keyCode == KeyCode.Backspace && guiEvent.type == EventType.KeyDown)
        {
            gradient.RemoveKey(selectedKeyIndex);
            if (selectedKeyIndex >= gradient.NumKeys)
            {
                selectedKeyIndex--;
            }

            needRepaint = true;
        }
    }

    public void SetGradient(CustomGradient gradient)
    {
        this.gradient = gradient;
    }

    private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
        isOpen = true;
    }

    private void OnDisable()
    {
        isOpen = false;
    }
}
