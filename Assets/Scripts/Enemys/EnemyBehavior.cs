using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : ScriptableObject
{
    [SerializeField] protected float hp;
    [SerializeField] protected float recognizeRad = 6f;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float attackRad = 1f;
    [SerializeField] protected EnemyType enemyType;

    protected EnemyBase enemyBase;
    protected EnemyBehaviorState currentState;
    public abstract void Initialization(EnemyBase _base);
    public abstract void Update();
    public abstract void ChangeState(EnemyBehaviorState _state);

    public enum EnemyBehaviorState
    {
        Idle,
        Recognize,
        Move,
        Attack,
    }
    public EnemyType GetEnemyType() { return this.enemyType; }
}
