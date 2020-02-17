using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Gradient gr;
    [Serializable]
    public class Prop
    {
        public RectTransform rt;
        [HideInInspector]
        public Vector2 oriPos;
    }

    public List<Prop> targets;
    public float speed;

    Vector2 dragOriPos;
    Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        return;

        UpdateOriPos();
    }
    
    
    void UpdateOriPos()
    {
        if (targets != null)
        {
            foreach (var target in targets)
            {
                target.oriPos = target.rt.anchoredPosition;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragOriPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UpdateOriPos();
    }

    public void OnDrag(PointerEventData eventData)
    {
        offset = ((eventData.position - dragOriPos));
        Apply();
    }

    void Apply()
    {
        foreach (var target in targets)
        {
            target.rt.anchoredPosition = target.oriPos + offset;
        }
    }

    float f = 0;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            f += 1f;
            Debug.LogError("f:" + f);

            Debug.LogError(Mathf.InverseLerp(0f, 5f, f));
            Debug.LogError(Mathf.InverseLerp(0f, 10f, f));

            Debug.LogError(Mathf.InverseLerp(1f, 8f, f));
            Debug.LogError(Mathf.InverseLerp(2f, 10f, f));
        }
    }
}
