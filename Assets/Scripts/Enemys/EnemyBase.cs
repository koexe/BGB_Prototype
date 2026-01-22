using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] int hp;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] EnemyBehavior behavior;
    [SerializeField] EnemyType enemyType;

    Action onKill;
    public void Initialization(Action _onKill)
    {
        this.onKill = _onKill;
        Type t_behaviorType = DataLibrary.instance.GetEnemyType(enemyType);

        this.behavior = (EnemyBehavior)Activator.CreateInstance(t_behaviorType);

        this.behavior.Initialization(this);
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
            Instantiate(itemPrefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

    }
}
