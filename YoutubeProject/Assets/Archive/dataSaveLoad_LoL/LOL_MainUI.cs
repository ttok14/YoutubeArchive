using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatPropType
{
    Damage,
    Defense,
    CC,
    Move
}

public enum DamageType
{
    AD,
    AP
}

public class LOL_MainUI : MonoBehaviour
{
    [System.Serializable]
    public class StatProp
    {
        public StatPropType type;
        public Image[] stacks;
    }

    public Image imgChampion;

    public Text txtDamageType;
    public Slider sliderAttackType;

    public Image[] controlLevel;

    public Text txtDescription;

    public List<StatProp> stats;

    public Color controlLevelFilledColor, controlLevelEmptyColor;
    public Color statStackEmptyColor, statStackLevel1Color, statStackLevel2Color, statStackLevel3Color;

    ChampionData[] data;

    int curIndex; 

    private void Start()
    {
        data = LOL_SaveLoadSystem.Load();

        UpdateUI();
    }

    public void SetDamageType(DamageType type)
    {
        txtDamageType.text = type == DamageType.AD ? "물리" : "마법";
    }

    public void SetAttackType(float value)
    {
        sliderAttackType.value = value;
    }

    public void SetControlLevel(int level)
    {
        for (int i = 0; i < controlLevel.Length; i++)
        {
            if(i < level)
            {
                controlLevel[i].color = controlLevelFilledColor;
            }
            else
            {
                controlLevel[i].color = controlLevelEmptyColor;
            }
        }
    }

    public void SetDescription(string txt)
    {
        txtDescription.text = txt;
    }

    public void SetStat(StatProp target, int level)
    {
        for (int i = 0; i < target.stacks.Length; i++)
        {
            var imgs = target.stacks;

            if(i < level)
            {
                imgs[i].color = GetStatColor(i);
            }
            else
            {
                imgs[i].color = statStackEmptyColor;
            }
        }
    }

    Color GetStatColor(int index)
    {
        if(index == 0)
        {
            return statStackLevel1Color;
        }
        else if(index == 1)
        {
            return statStackLevel2Color;
        }else if(index == 2)
        {
            return statStackLevel3Color;
        }
        else
        {
            Debug.LogError("error");
            return Color.black;
        }
    }

    public void SetChampionImage(Sprite sprite)
    {
        imgChampion.sprite = sprite;
    }

    void MoveIndex(bool leftOrRight)
    {
        if(leftOrRight)
        {
            curIndex--;
        }
        else
        {
            curIndex++;
        }

        curIndex = Mathf.Clamp(curIndex, 0, data.Length - 1);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (data == null ||
            data.Length == 0)
            return;

        var champData = data[curIndex];

        SetDamageType((DamageType)champData.damageType);
        SetAttackType(champData.attackType);
        SetControlLevel(champData.controlLevel);
        SetDescription(champData.description);

        for (int i = 0; i < champData.statProps.Length; i++)
        {
            SetStat(stats[i], champData.statProps[i]);
        }

        SetChampionImage(Resources.Load<Sprite>(champData.path));
    }

    public void OnClickLeft()
    {
        MoveIndex(true);
    }

    public void OnClickRight()
    {
        MoveIndex(false);
    }
}
