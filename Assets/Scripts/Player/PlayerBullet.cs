using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    StatSystem playerStat;
    public int currentMag;
    List<BuffBlock> bulletBuffs;

    bool isInAttack = false;
    bool isInReload = false;
    public void Initialization(StatSystem _playerStat)
    {
        this.playerStat = _playerStat;
        this.currentMag = (int)this.playerStat.GetStat(StatType.Mag);
    }

    public void LoadBullet()
    {
        foreach (var t_buff in this.bulletBuffs)
        {
            this.playerStat.AddBuff(t_buff);
        }
    }
    public void UnloadBullet()
    {
        foreach (var t_buff in this.bulletBuffs)
        {
            this.playerStat.RemoveBuff(t_buff);
        }
    }
    public async void Attack(GameObject _temp_bullet)
    {
        if (this.isInAttack || this.isInReload) return;
        if (this.currentMag <= 0)
        {
            Reload();
            return;
        }

        this.isInAttack = true;

        this.currentMag--;

        var t_bullet = Instantiate(_temp_bullet).GetComponent<Bullet>();
        Vector3 t_mouseScreenPos = Input.mousePosition;
        Vector3 t_mouseWorldPos = Camera.main.ScreenToWorldPoint(t_mouseScreenPos);
        t_mouseWorldPos.z = 0f;

        Vector3 t_dir = (t_mouseWorldPos - transform.position).normalized;
        t_bullet.transform.position = this.transform.position;
        t_bullet.Initialization(t_dir, 10f, 1f, 5f);


        await UniTask.WaitForSeconds(1 / this.playerStat.GetStat(StatType.Ats));
        this.isInAttack = false;
    }

    public async void Reload()
    {
        await UniTask.WaitForSeconds(this.playerStat.GetStat(StatType.ReloadTime));
        this.currentMag = (int)this.playerStat.GetStat(StatType.Mag);
    }

}
