using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameStatUI : UIBase
{
    public static string identifier = "IngameStatUI";
    [SerializeField] ToggleListUI healthUI;
    [SerializeField] ToggleListUI rollUI;
    [SerializeField] ToggleListUI magUI;


    public override void Initialization(UIData _data)
    {
        IngameStatUIData t_IngameStatUIData = _data as IngameStatUIData;
        if (t_IngameStatUIData == null)
        {
            Debug.Log("Invalid DataType in FadeUI");
            return;
        }
        this.healthUI.Initialization(t_IngameStatUIData.hp);
        this.rollUI.Initialization(t_IngameStatUIData.rollPoint);
        this.magUI.Initialization(t_IngameStatUIData.magAmount);
    }

    public override void Hide()
    {
        this.isShow = false;
        this.contents.SetActive(false);
        this.data.onHide?.Invoke();
    }

    public override void Show()
    {
        this.isShow = true;
        this.contents.SetActive(true);
    }

    public void UseRoll()
    {
        this.rollUI.UntoggleOne();
    }
    public void GainRoll()
    {
        this.rollUI.ToggleOne();
    }
    public void ToggleHp(bool _is)
    {
        if(_is)
        {
            this.healthUI.ToggleOne();
        }
        else
        {
            this.healthUI.UntoggleOne();
        }
    }
    public void UseMag()
    {
        this.magUI.UntoggleOne();
    }
    public void ResetMag()
    {
        this.magUI.ToggleAll();
    }
}

public class IngameStatUIData : UIData
{
    public int hp;
    public int rollPoint;
    public int magAmount;
}