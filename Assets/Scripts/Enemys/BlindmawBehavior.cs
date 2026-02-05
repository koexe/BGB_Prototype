using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_BlindMaw", menuName = "SO/Enemy/BlindMaw")]
public class BlindmawBehavior : EnemyBehavior
{
    [SerializeField] float rungeDis = 1f;
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] float rungeSpeed = 60f;
    public override void Update()
    {
        switch (this.currentState)
        {
            case EnemyBehaviorState.Idle:
                Idle();
                break;
            case EnemyBehaviorState.Recognize:
                break;
            case EnemyBehaviorState.Move:
                Move();
                break;
            case EnemyBehaviorState.Attack:
                break;
        }
    }
    public override void ChangeState(EnemyBehaviorState _state)
    {
        switch (_state)
        {
            case EnemyBehaviorState.Idle:
                this.animModule.PlayIdleAnimation();
                break;
            case EnemyBehaviorState.Recognize:
                RecognizeWait();
                break;
            case EnemyBehaviorState.Move:
                this.animModule.PlayWalkAnimation();
                break;
            case EnemyBehaviorState.Attack:
                this.animModule.PlayAttackAnimation();
                Attack();
                break;
        }
        this.currentState = _state;
    }


    async void RecognizeWait()
    {
        this.animModule.PlayIdleAnimation();
        await UniTask.WaitForSeconds(0.3f);
        this.animModule.PlayWalkAnimation();
        ChangeState(EnemyBehaviorState.Move);
    }
    void Move()
    {
        var t_player = IngameManager.instance.GetPlayer();
        if (Vector2.Distance(t_player.transform.position, this.enemyBase.transform.position) >= this.attackRad)
        {
            Vector3 t_dir = (t_player.transform.position - this.enemyBase.transform.position).normalized;
            this.enemyBase.transform.position += t_dir * this.speed * Time.fixedDeltaTime;
        }
        else
        {
            ChangeState(EnemyBehaviorState.Attack);
        }
    }
    void Idle()
    {
        var t_player = IngameManager.instance.GetPlayer();
        if (Vector2.Distance(t_player.transform.position, this.enemyBase.transform.position) >= this.recognizeRad)
        {
            Vector3 t_dir = Random.insideUnitCircle.normalized;
            this.enemyBase.transform.position += t_dir * this.speed * Time.fixedDeltaTime;
        }
        else
        {
            RecognizeWait();
        }
    }
    async void Attack()
    {
        await UniTask.WaitForSeconds(this.attackDelay);

        var t_player = IngameManager.instance.GetPlayer();
        if (t_player == null)
            return;

        Vector2 t_startPos = this.enemyBase.transform.position;
        Vector2 t_targetPos = t_player.transform.position;
        Vector2 t_targetDir = (t_targetPos - t_startPos).normalized;

        float t_movedDistance = 0f;

        while (t_movedDistance < this.rungeDis)
        {
            float t_step = this.rungeSpeed * Time.fixedDeltaTime;

            this.enemyBase.transform.position +=
                (Vector3)(t_targetDir * t_step);

            t_movedDistance += t_step;

            await UniTask.WaitForFixedUpdate();
        }

        ChangeState(EnemyBehaviorState.Move);
    }
}
