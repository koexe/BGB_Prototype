using Cysharp.Threading.Tasks;
using System.Threading;
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
        CancelAllAsync();
        this.cts = new CancellationTokenSource();

        switch (_state)
        {
            case EnemyBehaviorState.Idle:
                this.animModule.PlayIdleAnimation();
                break;

            case EnemyBehaviorState.Recognize:
                RecognizeWait(this.cts.Token).Forget();
                break;

            case EnemyBehaviorState.Move:
                this.animModule.PlayWalkAnimation();
                break;

            case EnemyBehaviorState.Attack:
                this.animModule.PlayAttackAnimation();
                Attack(this.cts.Token).Forget();
                break;
        }

        this.currentState = _state;
    }

    public void CancelAllAsync()
    {
        if (this.cts != null)
        {
            this.cts.Cancel();
            this.cts.Dispose();
            this.cts = null;
        }
    }

    async UniTaskVoid RecognizeWait(CancellationToken _ct)
    {
        this.animModule.PlayIdleAnimation();

        await UniTask.WaitForSeconds(0.3f, cancellationToken: _ct);

        this.animModule.PlayWalkAnimation();
        ChangeState(EnemyBehaviorState.Move);
    }

    void Move()
    {
        var t_player = IngameManager.instance.GetPlayer();
        if (t_player == null)
            return;

        if (Vector2.Distance(t_player.transform.position, this.enemyBase.transform.position) >= this.attackRad)
        {
            FlipSprite(t_player.transform.position);
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
        if (t_player == null)
            return;

        if (Vector2.Distance(t_player.transform.position, this.enemyBase.transform.position) >= this.recognizeRad)
        {
            Vector3 t_dir = Random.insideUnitCircle.normalized;
            this.enemyBase.transform.position += t_dir * this.speed * Time.fixedDeltaTime;
        }
        else
        {
            ChangeState(EnemyBehaviorState.Recognize);
        }
    }

    async UniTaskVoid Attack(CancellationToken _ct)
    {
        await UniTask.WaitForSeconds(this.attackDelay, cancellationToken: _ct);

        var t_player = IngameManager.instance.GetPlayer();
        if (t_player == null)
            return;

        Vector2 t_startPos = this.enemyBase.transform.position;
        Vector2 t_targetPos = t_player.transform.position;
        Vector2 t_targetDir = (t_targetPos - t_startPos).normalized;

        FlipSprite(t_targetPos);

        float t_movedDistance = 0f;

        while (t_movedDistance < this.rungeDis)
        {
            _ct.ThrowIfCancellationRequested();

            float t_step = this.rungeSpeed * Time.fixedDeltaTime;

            this.enemyBase.transform.position += (Vector3)(t_targetDir * t_step);
            t_movedDistance += t_step;

            await UniTask.WaitForFixedUpdate(_ct);
        }

        ChangeState(EnemyBehaviorState.Move);
    }

    public void FlipSprite(Vector3 _target)
    {
        if (_target.x > this.enemyBase.transform.position.x)
            this.animModule.Flip(true);
        else
            this.animModule.Flip(false);
    }
}
