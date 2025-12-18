using UnityEngine;

[CreateAssetMenu(fileName = "Bullet_", menuName = "SO/Bullet")]
public class Mag : ScriptableObject
{
    [SerializeField] string magName;
    [SerializeField] int mag;
    [SerializeField] int currentMag;
    [SerializeField] int attackAmount;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] public BuffSystem buffSystem;

    public string GetMagName() => this.magName;
    public int GetAttackAmount() => this.attackAmount;
    public GameObject GetBulletPrefab() => this.bulletPrefab;
    public int GetCurrentAmount() => this.currentMag;
    public void UseBullet() => this.currentMag -= 1;
    public void FullCurrentAmount()
    {
        var t_buff = this.buffSystem.GetStat(BuffType.BulletAmount);
        this.currentMag = this.mag + (t_buff != -1 ? (int)t_buff : 0);
    }
    public BuffSystem GetBuffSystem() => this.buffSystem;
}
