using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehavior : ScriptableObject
{
    [SerializeField] RuntimeAnimatorController anim;
    [SerializeField] protected float hp;
    [SerializeField] protected float recognizeRad = 6f;
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float attackRad = 1f;
    [SerializeField] protected EnemyType enemyType;

    protected EnemyAnimationModule animModule;
    protected EnemyBase enemyBase;
    protected EnemyBehaviorState currentState;
    public virtual void Initialization(EnemyBase _base, EnemyAnimationModule _animationModule)
    {
        this.enemyBase = _base;
        this.animModule = _animationModule;
        this.animModule.Initialization(this.anim);
        ChangeState(EnemyBehaviorState.Attack);
    }
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
    public RuntimeAnimatorController GetAnim() => this.anim;
}
