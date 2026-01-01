using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] int hp;
    [SerializeField] Vector2 currentDir;
    [SerializeField] float speed;
    [SerializeField] GameObject itemPrefab;
    Action onKill;
    public void Initialization(Action _onKill)
    {
        this.onKill = _onKill;
    }


    private void FixedUpdate()
    {

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
