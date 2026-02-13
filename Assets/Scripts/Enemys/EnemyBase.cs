using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] float hp;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] EnemyBehavior behavior;
    [SerializeField] EnemyType enemyType;
    [SerializeField] EnemyAnimationModule enemyAnimationModule;

    Action onKill;
    public void Initialization(EnemyBehavior _behavior, Action _onKill)
    {
        this.onKill = _onKill;
        this.behavior = _behavior;
        this.hp = _behavior.GetHp();
        this.behavior.Initialization(this, this.enemyAnimationModule);
    }


    private void FixedUpdate()
    {
        if (IngameManager.instance.gameState != GameState.Running) return;
        this.behavior.Update();
    }

    public void Hit(int _damage)
    {
        this.hp -= _damage;
        if (this.hp <= 0)
        {
            this.onKill?.Invoke();
            this.behavior.CancelAllAsync();
            Destroy(this.gameObject);
        }

    }
}
