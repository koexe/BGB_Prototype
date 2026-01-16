using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] PlayerBullet[] playerBullets;
    int currentBulletIndex = 0;
    public void Initialization(StatSystem _stat)
    {
        foreach(var t_playerBullet in this.playerBullets)
        {
            t_playerBullet.Initialization(_stat);
        }
    }

    public void LoadNewBullet()
    {
        this.playerBullets[currentBulletIndex].UnloadBullet();
        this.currentBulletIndex++;
        if (this.currentBulletIndex >= this.playerBullets.Length)
            this.currentBulletIndex = 0;
        this.playerBullets[currentBulletIndex].LoadBullet();
    }


    public void Attack(GameObject _temp_bullet)
    {
        this.playerBullets[currentBulletIndex].Attack(_temp_bullet);
    }
}
