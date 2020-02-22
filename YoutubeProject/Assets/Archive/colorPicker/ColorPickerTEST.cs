using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPickerTEST : MonoBehaviour
{
    // 컬러를 받아올 entity 
    public CustomGradientEntity entity;

    private void Update()
    {
        // evaluate 에서 현재 xTime ,yTime 기준으로 컬러 가져옴  
        Debug.Log(entity.Evaluate());
    }
}
