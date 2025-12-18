using System.Collections.Generic;
using UnityEngine;

public class BuffSystem
{
    public Dictionary<BuffType, BuffBlock> buffBlocks;
    public float GetStat(BuffType _type)
    {
        if (this.buffBlocks.TryGetValue(_type, out var t_value))
        {
            return t_value.buffAmount;
        }
        return -1;
    }
    public BuffSystem()
    {
        this.buffBlocks = new Dictionary<BuffType, BuffBlock>();
    }
    public void AddBuff(BuffType _type, float _value)
    {
        this.buffBlocks.Add(_type, new BuffBlock() { buffType = _type, buffAmount = _value });
    }
}

public class BuffBlock
{
    public BuffType buffType;
    public float buffAmount;
}

public enum BuffType
{
    BulletScale,
    BulletAmount,
    Judo
}