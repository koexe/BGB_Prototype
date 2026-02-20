using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    [SerializeField] GameObject weaponSelect;
    
    public void ShowWeaponSelect()
    {
        this.weaponSelect.SetActive(true);
    }

    public void ChangeToGameScene()
    {
        GameManager.instance.ChangeToGameScene();
    }
}
