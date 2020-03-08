using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[DisallowMultipleComponent]
public class CustomGradientEntity : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public enum FollowerScrollType
    {
        Both,
        Horizontal,
        Vertical
    }

    [HideInInspector]
    public CustomGradient gradient;
    [HideInInspector]
    public CustomGradientOverrider overrider;

    public CustomGradientEntity[] overrideReceivers;

    public Image imgTarget;

    [Header("드래그용")]
    public Canvas canvas;
    public RectTransform pointerFollower;
    public FollowerScrollType scrollType;
    Vector3[] followerClampPoses = new Vector3[4]; // leftBot, leftTop, rightTop, rightBot 순
    [Space]

    [Range(0f, 1f)]
    [SerializeField]
    float xTime = 0.5f;
    [Range(0f, 1f)]
    [SerializeField]
    float yTime = 0.5f;

    public int texWidth = 32;
    public int texHeight = 32;

    bool updateColorOverrideChain;

    Vector3 lastPos;

    void Start()
    {
        lastPos = imgTarget.transform.position;
        CalculateFollowerClampCorners();
        imgTarget.sprite = Sprite.Create(gradient.GetTexture(texWidth, texHeight), new Rect(0, 0, texWidth, texHeight), Vector2.zero);
        gradient.UpdateTexture();
        SetPosition(xTime, yTime);
    }

    private void OnValidate()
    {
        updateColorOverrideChain = true;
        CalculateFollowerClampCorners();
    }

    private void Update()
    {
        if (lastPos != imgTarget.transform.position)
        {
            lastPos = imgTarget.transform.position;
            CalculateFollowerClampCorners();
        }

        if (updateColorOverrideChain)
        {
            updateColorOverrideChain = false;
            SendColorToNextReceiver(xTime, yTime);
        }
    }

    public void OverrideColor(Color color)
    {
        if (overrider != null &&
            overrider.type != CustomGradientRuntimeOverrideType.None)
        {
            overrider.Apply(gradient, color);
        }

        SendColorToNextReceiver(xTime, yTime);
    }

    public Color Evaluate(float xTime, float yTime)
    {
        return gradient.Evaluate(xTime, yTime);
    }

    public Color Evaluate()
    {
        return gradient.Evaluate(xTime, yTime);
    }

    public void SetPosition(float xTime, float yTime)
    {
        updateColorOverrideChain = true;

        xTime = Mathf.Clamp(xTime, 0f, 1f);
        yTime = Mathf.Clamp(yTime, 0f, 1f);

        this.xTime = xTime;
        this.yTime = yTime;

        if (pointerFollower != null)
        {
            Vector3 followerWorldPos = ConvertNormToFollowerWorldPos(xTime, yTime);
            SetFollowerPosByWorldPos(followerWorldPos);
        }
    }

    public void SendColorToNextReceiver(float xTime, float yTime)
    {
        if (overrideReceivers != null)
        {
            var colorSent = Evaluate(xTime, yTime);

            for (int i = 0; i < overrideReceivers.Length; i++)
            {
                if (overrideReceivers[i] != null)
                {
                    overrideReceivers[i].OverrideColor(colorSent);
                }
            }
        }
    }

    private void CalculateFollowerClampCorners()
    {
        if (imgTarget != null)
        {
            imgTarget.rectTransform.GetWorldCorners(followerClampPoses);

            if (scrollType == FollowerScrollType.Horizontal)
            {
                for (int i = 0; i < followerClampPoses.Length; i++)
                {
                    followerClampPoses[i].y = imgTarget.rectTransform.position.y;
                }
            }
            else if (scrollType == FollowerScrollType.Vertical)
            {
                for (int i = 0; i < followerClampPoses.Length; i++)
                {
                    followerClampPoses[i].x = imgTarget.rectTransform.position.x;
                }
            }
        }
    }

    Vector3 ConvertNormToFollowerWorldPos(float x, float y)
    {
        return new Vector3(
            Mathf.Lerp(followerClampPoses[0].x, followerClampPoses[3].x, x),
            Mathf.Lerp(followerClampPoses[0].y, followerClampPoses[1].y, y),
            followerClampPoses[0].z);
    }

    Vector3 ConvertScreenToWorld(RectTransform rt, Vector2 pos)
    {
        if (canvas == null)
            Debug.LogError("Canvas Unassigned");

        Vector3 worldPos;
        Camera cam = null;

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera ||
            canvas.renderMode == RenderMode.WorldSpace)
            cam = canvas.worldCamera;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, pos, cam, out worldPos);

        return worldPos;
    }

    void SetPositionByFollowerScreenPos(Vector2 screenPos)
    {
        SetPosition(
            Mathf.InverseLerp(followerClampPoses[0].x, followerClampPoses[3].x, ConvertScreenToWorld(pointerFollower, screenPos).x),
            Mathf.InverseLerp(followerClampPoses[0].y, followerClampPoses[1].y, ConvertScreenToWorld(pointerFollower, screenPos).y));
    }

    //void SetFollowerPosByScreenPos(Vector2 pos)
    //{
    //    Vector3 worldPos = ConvertScreenToWorld(pointerFollower, pos);
    //    SetFollowerPosByWorldPos(worldPos);
    //}

    void SetFollowerPosByWorldPos(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, followerClampPoses[0].x, followerClampPoses[3].x);
        pos.y = Mathf.Clamp(pos.y, followerClampPoses[0].y, followerClampPoses[1].y);

        pointerFollower.position = pos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pointerFollower == null)
            return;
        SetPositionByFollowerScreenPos(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointerFollower == null)
            return;
        SetPositionByFollowerScreenPos(eventData.position);
    }

    private void OnDrawGizmos()
    {
        if (imgTarget == null)
            return;

        Rect rt = imgTarget.rectTransform.rect;

        Color old = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Lerp(rt.xMin, rt.xMax, xTime), Mathf.Lerp(rt.yMin, rt.yMax, yTime)), 2);
        Gizmos.color = old;
    }
}
