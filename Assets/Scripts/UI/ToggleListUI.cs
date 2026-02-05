using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleListUI : MonoBehaviour
{
    [SerializeField] Sprite toggledImage;
    [SerializeField] Sprite untoggledImage;
    [SerializeField] GameObject elementPrefab;
    [SerializeField] List<Image> images = new List<Image>();
    [SerializeField] int currentAmount;
    public void Initialization(int _amount)
    {

        for (int i = 0; i < _amount; i++)
        {
            if (i >= this.images.Count)
            {
                Image t_image = Instantiate(this.elementPrefab, this.transform).GetComponent<Image>();
                t_image.sprite = this.toggledImage;
                t_image.SetNativeSize();
                this.images.Add(t_image);
            }
            else
            {
                this.images[i].sprite = this.toggledImage;
                this.images[i].SetNativeSize();
            }
        }
        this.currentAmount = _amount;
    }

    public void UntoggleOne()
    {
        this.currentAmount--;
        this.currentAmount = Mathf.Clamp(this.currentAmount, 0, this.images.Count + 1);
        this.images[this.currentAmount].sprite = this.untoggledImage;
    }

    public void ToggleOne()
    {
        this.currentAmount = Mathf.Clamp(this.currentAmount, 0, this.images.Count + 1);
        this.images[this.currentAmount].sprite = this.toggledImage;
        this.currentAmount++;
    }
}
