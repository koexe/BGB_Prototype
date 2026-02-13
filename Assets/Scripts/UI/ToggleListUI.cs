using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleListUI : MonoBehaviour
{
    [SerializeField] Sprite toggledImage;
    [SerializeField] Sprite untoggledImage;
    [SerializeField] GameObject elementPrefab;

    [SerializeField] List<Image> images = new List<Image>();

    int currentAmount;

    public void Initialization(int _amount)
    {
        if (_amount < 0)
            _amount = 0;

        for (int i = 0; i < _amount; i++)
        {
            if (i >= this.images.Count)
            {
                Image t_image = Instantiate(this.elementPrefab, this.transform)
                                .GetComponent<Image>();

                this.images.Add(t_image);
            }

            this.images[i].sprite = this.toggledImage;
            this.images[i].SetNativeSize();
        }

        // 남는 이미지가 있다면 untoggle 처리
        for (int i = _amount; i < this.images.Count; i++)
        {
            this.images[i].sprite = this.untoggledImage;
        }

        this.currentAmount = Mathf.Clamp(_amount, 0, this.images.Count);
    }

    public void UntoggleOne()
    {
        if (this.currentAmount <= 0)
            return;

        this.currentAmount--;

        this.images[this.currentAmount].sprite = this.untoggledImage;
    }

    public void ToggleOne()
    {
        if (this.currentAmount >= this.images.Count)
            return;

        this.images[this.currentAmount].sprite = this.toggledImage;
        this.currentAmount++;
    }

    public void ToggleAll()
    {
        foreach(var t_image in this.images)
            t_image.sprite = this.toggledImage;
        this.currentAmount = this.images.Count;
    }
}
