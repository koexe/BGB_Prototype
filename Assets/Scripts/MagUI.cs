using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI magNameText;
    [SerializeField] TextMeshProUGUI buffText;
    [SerializeField] Mag mag;
    [SerializeField] Button magButton;

    public void initialization(Mag _mag, bool _isInteractable = false, Action<Mag> buttonAction = null)
    {
        this.magNameText.text = _mag.GetMagName();
        this.buffText.text = "";

        AddBuffText(_mag.GetBuffSystem());
        this.mag = _mag;
        this.magButton.interactable = _isInteractable;
        this.magButton.onClick.RemoveAllListeners();
        this.magButton.onClick.AddListener(() => buttonAction(this.mag));
    }

    void AddBuffText(BuffSystem _system)
    {
        foreach (var t_buff in _system.buffBlocks)
        {
            switch (t_buff.Key)
            {
                case BuffType.BulletScale:
                    this.buffText.text += "BulletScale";
                    break;
                case BuffType.BulletAmount:
                    this.buffText.text += "BulletAmount";
                    break;
                case BuffType.Judo:
                    this.buffText.text += "HomingShot";
                    break;
                default:
                    break;
            }
            this.buffText.text += $" {t_buff.Value.buffAmount}";

        }
    }
}
