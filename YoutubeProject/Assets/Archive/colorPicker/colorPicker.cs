using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class colorPicker : MonoBehaviour
{
    Camera cam;
    Canvas canvas;

    Image imgBasePaletteColorPicker;
    Image imgBrightnessPicker;

    Image imgBasePalette;
    Image imgBrightness;
    Image imgFinalColorView;

    Vector2 brightnessPaletteImgSize = new Vector2(64, 512);
    Vector2 basePaletteImgSize = new Vector2(512, 512);
    Vector2 finalColorViewImgSize = new Vector2(128, 128);

    Texture2D texBrightness;

    Color currentSelectedBaseColor;
    Color currentFinalColor = Color.black;

    Color[] basePaletteBlendingColors =
    {
        new Color(1,0,0,1),
        new Color(1,1,0,1),
        new Color(0,1,0,1),
        new Color(0,1,1,1),
        new Color(0,0,1,1),
        new Color(1,0,1,1),
        new Color(1,0,0,1)
    };

    float curBrightness;

    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        canvas = GetComponent<Canvas>();

        // Image 생성 
        imgBrightness = CreateImage("brightness", new Vector2(450, 0), brightnessPaletteImgSize);
        imgBasePalette = CreateImage("basePalette", new Vector2(0, 0), basePaletteImgSize);
        imgFinalColorView = CreateImage("colorView", new Vector2(-400, 0), finalColorViewImgSize, false);

        imgBasePaletteColorPicker = CreateImage("basePalettePicker", Vector2.zero, new Vector2(20, 20), false);
        imgBrightnessPicker = CreateImage("brightnessPicker", Vector2.zero, new Vector2(20, 20), false);

        // Picker 에 이벤트연결
        var trigger = imgBasePalette.gameObject.AddComponent<EventTrigger>();
        AddListener(trigger, OnDragBasePalette, EventTriggerType.Drag);

        trigger = imgBrightness.gameObject.AddComponent<EventTrigger>();
        AddListener(trigger, OnDragBrightnessPalette, EventTriggerType.Drag);

        // Texture 생성 
        Texture2D basePaletteTex = new Texture2D((int)basePaletteImgSize.x, (int)basePaletteImgSize.y);
        texBrightness = new Texture2D((int)basePaletteImgSize.x, (int)basePaletteImgSize.y);
        imgBrightness.sprite = Sprite.Create(texBrightness, new Rect(0, 0, brightnessPaletteImgSize.x, brightnessPaletteImgSize.y), Vector2.zero);

        for (int i = 0; i < basePaletteImgSize.y; i++)
        {
            for (int j = 0; j < basePaletteImgSize.x; j++)
            {
                float curHorPos = j / basePaletteImgSize.x;
                float curVerPos = i / basePaletteImgSize.y;

                Color c = ExtractColorFromBasePalette(basePaletteBlendingColors, curHorPos, curVerPos);
                basePaletteTex.SetPixel(j, i, c);
            }
        }

        basePaletteTex.Apply();

        Sprite sprBasePalette = Sprite.Create(basePaletteTex, new Rect(0, 0, basePaletteImgSize.x, basePaletteImgSize.y), Vector2.zero);
        imgBasePalette.sprite = sprBasePalette;
    }

    void AddListener(EventTrigger trigger, Action<PointerEventData> listener, EventTriggerType type)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((eventData) => listener((PointerEventData)eventData));
        trigger.triggers.Add(entry);
    }

    public void SetBrightnessPalette(Color baseColor)
    {
        Color[] basePalette = new Color[] { Color.black, baseColor, Color.white };

        for (int i = 0; i < brightnessPaletteImgSize.y; i++)
        {
            float curVerPos = i / brightnessPaletteImgSize.y;
            Color c = ExtractColorFromBrightnessPalette(basePalette, curVerPos);

            for (int j = 0; j < brightnessPaletteImgSize.x; j++)
            {
                texBrightness.SetPixel(j, i, c);
            }
        }

        texBrightness.Apply();
    }

    Image CreateImage(string name, Vector2 pos, Vector2 size, bool raycast = true)
    {
        GameObject obj = new GameObject();
        obj.name = name;
        Image img = obj.AddComponent<Image>();
        img.rectTransform.SetParent(canvas.transform);
        img.rectTransform.anchoredPosition = pos;
        img.rectTransform.sizeDelta = size;
        img.raycastTarget = raycast;
        img.color = Color.white;
        return img;
    }

    Color ExtractColorFromBasePalette(Color[] palette, float horRatio, float verRatio)
    {
        int leftColorId, rightColorId;
        float subRatio;

        GetTwoAdjacentIndexesInArray(palette.Length, horRatio, out leftColorId, out rightColorId, out subRatio);

        Color pureColor = GetInterpolatedColor(palette[leftColorId], palette[rightColorId], subRatio);

        return GetInterpolatedColor(Color.grey, pureColor, verRatio);
    }

    Color ExtractColorFromBrightnessPalette(Color[] palette, float verRatio)
    {
        int leftColorId, rightColorId;
        float subRatio;

        GetTwoAdjacentIndexesInArray(palette.Length, verRatio, out leftColorId, out rightColorId, out subRatio);

        return GetInterpolatedColor(palette[leftColorId], palette[rightColorId], subRatio);
    }

    Color GetInterpolatedColor(Color sour, Color dest, float ratio)
    {
        return (sour * (1 - ratio)) + (dest * ratio);
    }

    void GetTwoAdjacentIndexesInArray(int arrayCount, float ratio, out int left, out int right, out float subRatio)
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
    
    private void FixedUpdate()
    {
        if(imgFinalColorView.color.Equals(currentFinalColor) == false)
        {
            imgFinalColorView.color = currentFinalColor;
        }
    }

    Vector2 ScreenPosToUI(Vector2 pos)
    {
        return canvas.transform.InverseTransformPoint(pos);
    }

    void OnDragBasePalette(PointerEventData data)
    {
        Vector2 pos = ScreenPosToUI(data.position);

        imgBasePaletteColorPicker.rectTransform.anchoredPosition = pos;

        float horRatio = ((pos.x + imgBasePalette.rectTransform.anchoredPosition.x * -1) + imgBasePalette.rectTransform.rect.width * 0.5f) / imgBasePalette.rectTransform.rect.width;
        float verRatio = ((pos.y + imgBasePalette.rectTransform.anchoredPosition.y * -1) + imgBasePalette.rectTransform.rect.height * 0.5f) / imgBasePalette.rectTransform.rect.height;

        horRatio = Mathf.Clamp(horRatio, 0f, 1f);
        verRatio = Mathf.Clamp(verRatio, 0, 1f);

        currentSelectedBaseColor = ExtractColorFromBasePalette(basePaletteBlendingColors, horRatio, verRatio);

        SetBrightnessPalette(currentSelectedBaseColor);

        CalculateFinalColor();
    }

    void OnDragBrightnessPalette(PointerEventData data)
    {
        var pos = ScreenPosToUI(data.position);
        imgBrightnessPicker.rectTransform.anchoredPosition = pos;

        curBrightness = ((pos.y + imgBrightness.rectTransform.anchoredPosition.y * -1) + imgBrightness.rectTransform.rect.height * 0.5f) / imgBrightness.rectTransform.rect.height;
        curBrightness = Mathf.Clamp(curBrightness, 0f, 1f);

        CalculateFinalColor();
    }

    void CalculateFinalColor()
    {
        currentFinalColor = ExtractColorFromBrightnessPalette(new Color[] { Color.black, currentSelectedBaseColor , Color.white }, curBrightness);
    }
}