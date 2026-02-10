using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StatSystem
{
    Dictionary<StatType, StatBlock> baseStats;
    List<BuffBlock> buffBlocks;


    public StatSystem(WeaponInfo _weaponInfo)
    {
        this.baseStats = new Dictionary<StatType, StatBlock>();
        this.buffBlocks = new List<BuffBlock>();

        this.baseStats.Add(StatType.Atk, new StatBlock(StatType.Atk, _weaponInfo.atk, StatSign.Static));
        this.baseStats.Add(StatType.Ats, new StatBlock(StatType.Ats, _weaponInfo.ats, StatSign.Static));
        this.baseStats.Add(StatType.Crit, new StatBlock(StatType.Crit, _weaponInfo.crit, StatSign.Static));
        this.baseStats.Add(StatType.CritMag, new StatBlock(StatType.CritMag, _weaponInfo.critMag, StatSign.Static));
        this.baseStats.Add(StatType.Mag, new StatBlock(StatType.Mag, _weaponInfo.mag, StatSign.Static));
        this.baseStats.Add(StatType.ReloadTime, new StatBlock(StatType.ReloadTime, _weaponInfo.reload, StatSign.Static));
    }
    public void AddBaseStat(StatBlock _stat)
    {
        if (!this.baseStats.ContainsKey(_stat.statType))
            this.baseStats.Add(_stat.statType, _stat);
        else
        {
            switch (_stat.sign)
            {
                case StatSign.Static:
                    this.baseStats[_stat.statType].value += _stat.value;
                    break;
                case StatSign.Constant:
                    this.baseStats[_stat.statType].value += _stat.value;
                    break;
                case StatSign.Percentage:
                    float t_statValue = this.baseStats[_stat.statType].value * _stat.value;
                    this.baseStats[_stat.statType].value = t_statValue;
                    break;
                default:
                    break;
            }
        }
    }

    public void AddBuff(BuffBlock _buff)
    {
        this.buffBlocks.Add(_buff);
    }
    public void RemoveBuff(BuffBlock _buff)
    {
        this.buffBlocks.Remove(_buff);
    }
    public float GetStat(StatType _stat)
    {
        float t_base = 0f;
        float t_constMod = 0f;
        float t_magMod = 1f;

        if (this.baseStats.TryGetValue(_stat, out var t_staticValue))
        {
            t_base = t_staticValue.value;
        }
        foreach (var t_buff in this.buffBlocks)
        {
            if (t_buff.buffBlocks.TryGetValue(_stat, out var t_value))
            {
                switch (t_value.sign)
                {
                    case StatSign.Static:
                        t_base += t_value.value;
                        break;
                    case StatSign.Constant:
                        t_constMod += t_value.value;
                        break;
                    case StatSign.Percentage:
                        t_magMod += t_value.value;
                        break;
                    default:
                        break;
                }
            }
        }

        t_base = Mathf.Clamp(t_base, 0f, float.PositiveInfinity);
        t_constMod = Mathf.Clamp(t_constMod, 0f, float.PositiveInfinity);

        float t_returnValue = (t_base + t_constMod) * t_magMod;


        return t_returnValue;
    }


}

public class BuffBlock
{
    public Dictionary<StatType, StatBlock> buffBlocks;
    public float duration;
}

public class StatBlock
{
    public StatType statType;
    public float value;
    public StatSign sign;
    public StatBlock(StatType _type, float _amount, StatSign _sign)
    {
        this.statType = _type;
        this.value = _amount;
        this.sign = _sign;
    }
}