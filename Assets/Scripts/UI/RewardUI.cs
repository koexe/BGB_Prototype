using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RewardUI : UIBase
{
    public static string identifier = "RewardUI";
    [SerializeField] RewardElement[] elements;

    public override void Initialization(UIData _data)
    {
        RewardUIData t_rewardUIData = _data as RewardUIData;
        if (t_rewardUIData == null)
        {
            Debug.Log("Invalid DataType in RewardUI");
            return;
        }
        this.data = _data;
        for (int i = 0; i < t_rewardUIData.rewardInfos.Count; i++)
        {
            this.elements[i].Initialization(t_rewardUIData.rewardInfos[i]);
        }




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

}

public class RewardUIData : UIData
{
    public List<RewardInfo> rewardInfos;

    public struct RewardInfo
    {
        public PerkInfo perkInfo;
        public int optionGrade;
    }
}

