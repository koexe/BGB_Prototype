using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_BlindMaw", menuName = "SO/Enemy/BlindMaw")]
public class BlindmawBehavior : EnemyBehavior
{
    [SerializeField] float rungeDis = 1f;
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] float attackDuration = 0.6f;

    public override void Initialization(EnemyBase _base)
    {
        this.enemyBase = _base;
    }

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
                break;
            case EnemyBehaviorState.Recognize:
                RecognizeWait();
                break;
            case EnemyBehaviorState.Move:
                break;
            case EnemyBehaviorState.Attack:
                Attack();
                break;
        }
        this.currentState = _state;
    }


    async void RecognizeWait()
    {
        await UniTask.WaitForSeconds(0.3f);
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
        Vector2 t_targetDir =
            ((Vector2)t_player.transform.position - t_startPos).normalized;

        float t_elapsed = 0f;

        // attackDuration 동안 돌진
        while (t_elapsed < this.attackDuration)
        {
            float t_dt = Time.deltaTime;
            t_elapsed += t_dt;

            this.enemyBase.transform.position +=
                (Vector3)(t_targetDir * this.rungeDis * t_dt);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        ChangeState(EnemyBehaviorState.Move);
    }
}
