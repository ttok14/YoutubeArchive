using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class GradientWindow : EditorWindow
{
    public static bool isOpen;

    CustomGradient gradient;
    Func<CustomGradient.Layout> layoutGetter;
    int texSize;

    const int borderSize = 10;
    const float keyWidth = 10;
    const float keyHeight = 20;

    Rect gradientPreviewRect;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    int selectedKeyIndex;
    bool needRepaint;
    
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
        int length;

        if (layoutGetter() == CustomGradient.Layout.Horizontal)
        {
            gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, 25);
        }
        else
        {
            gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, position.height * 0.5f);
        }

        length = (int)gradientPreviewRect.width;

        GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture(length, layoutGetter()));

        keyRects = new Rect[gradient.NumKeys];

        for (int i = 0; i < gradient.NumKeys; i++)
        {
            CustomGradient.ColorKey key = gradient.GetKey(i);
            Rect keyRect = new Rect(
                gradientPreviewRect.x + gradientPreviewRect.width * key.Time_ - keyWidth / 2f,
                gradientPreviewRect.yMax + borderSize,
                keyWidth,
                keyHeight);

            if (i == selectedKeyIndex)
            {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
            }

            EditorGUI.DrawRect(keyRect, key.Color_);
            keyRects[i] = keyRect;

        }

        Rect settingRect = new Rect(borderSize, keyRects[0].yMax + borderSize, position.width - borderSize * 2, position.height);
        GUILayout.BeginArea(settingRect);
        EditorGUI.BeginChangeCheck();
        Color newColor = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Color_);
        if (EditorGUI.EndChangeCheck())
        {
            gradient.UpdateKeyColor(selectedKeyIndex, newColor);
        }
        gradient.blendMode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", gradient.blendMode);
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

            if (!mouseIsDownOverKey)
            {
                Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);

                selectedKeyIndex = gradient.AddKey(randomColor, keyTime);
                mouseIsDownOverKey = true;
                needRepaint = true;
            }
        }

        if (guiEvent.type == EventType.MouseUp &&
            guiEvent.button == 1)
        {
            for (int i = 0; i < keyRects.Length; i++)
            {
                if(keyRects[i].Contains(guiEvent.mousePosition))
                {
                    gradient.RemoveKey(i);

                    if(selectedKeyIndex >= i)
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

    public void SetGradient(CustomGradient gradient, Func<CustomGradient.Layout> layoutGetter)
    {
        this.gradient = gradient;
        this.layoutGetter = layoutGetter;
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
