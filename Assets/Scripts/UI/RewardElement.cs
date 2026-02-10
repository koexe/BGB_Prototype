using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardElement : MonoBehaviour
{
    [SerializeField] Sprite weaponModImage;
    [SerializeField] Sprite[] weaponOptionImages;
    [SerializeField] Image cardBackGround;
    [SerializeField] Image cardImage;
    RewardUIData.RewardInfo rewardInfo;

    public void Initialization(RewardUIData.RewardInfo _perkInfo)
    {
        if (_perkInfo.perkInfo.perkType == PerkType.Perk)
        {
            this.cardBackGround.sprite = this.weaponModImage;
        }
        else if (_perkInfo.perkInfo.perkType == PerkType.Option)
        {
            this.cardBackGround.sprite = this.weaponOptionImages[_perkInfo.optionGrade];
        }
        this.rewardInfo = _perkInfo;
    }

    public void OnClick()
    {
        if (this.rewardInfo.perkInfo.perkType == PerkType.Option)
        {
            IngameManager.instance.GetPlayer().AddBaseStat(this.rewardInfo.perkInfo.perkStat[this.rewardInfo.optionGrade]);
        }

    }
}
