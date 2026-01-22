using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_BoomShhRoom", menuName = "SO/Enemy/BoomShhRoom")]
public class BoomshhRoomBehavior : EnemyBehavior
{
    [SerializeField] float duration = 2f;
    [SerializeField] float revolutions = 1.5f;

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
        await UniTask.WaitForSeconds(1f);
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
        var t_player = IngameManager.instance.GetPlayer();
        if (t_player == null)
            return;

        Transform t_playerTr = t_player.transform;

        // 시작 오프셋 기준 각도 계산 (튐 방지)
        Vector2 t_center = t_playerTr.position;
        Vector2 t_offset = (Vector2)this.enemyBase.transform.position - t_center;

        if (t_offset.sqrMagnitude < 0.0001f)
            t_offset = Vector2.right * this.attackRad;

        float t_startAngle = Mathf.Atan2(t_offset.y, t_offset.x);
        float t_elapsed = 0f;

        while (t_elapsed < this.duration)
        {
            // 플레이어 이동 따라가기
            t_center = t_playerTr.position;

            t_elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(t_elapsed / this.duration);

            float t_angle =
                t_startAngle +
                (t * this.revolutions * Mathf.PI * 2f);

            Vector2 t_newPos =
                t_center +
                new Vector2(Mathf.Cos(t_angle), Mathf.Sin(t_angle)) * this.attackRad;

            this.enemyBase.transform.position = t_newPos;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
