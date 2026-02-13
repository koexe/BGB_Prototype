using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardElement : MonoBehaviour
{
    [SerializeField] Sprite weaponModImage;
    [SerializeField] Sprite[] weaponOptionImages;
    [SerializeField] Image cardBackGround;
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI cardText;
    [SerializeField] TextMeshProUGUI nameText;
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
        this.cardText.text = _perkInfo.perkInfo.perkDescription;
        this.nameText.text = _perkInfo.perkInfo.perkName;
    }

    public void OnClick()
    {
        if (this.rewardInfo.perkInfo.perkType == PerkType.Option)
        {
            IngameManager.instance.GetPlayer().AddBaseStat(this.rewardInfo.perkInfo.perkStat[this.rewardInfo.optionGrade]);
        }
        else if (this.rewardInfo.perkInfo.perkType == PerkType.Perk)
            IngameManager.instance.GetPlayer().AddBaseStat(this.rewardInfo.perkInfo.perkStat[0]);

        UIManager.instance.HideUI<RewardUI>(RewardUI.identifier);

    }
}
