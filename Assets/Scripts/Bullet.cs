using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Vector2 currentDir;
    float currentSpeed;
    float lifeTime;
    float currentLifeTime;
    bool isHoming;
    [SerializeField] float homingRad = 5f;
    [SerializeField] int damage;
    [SerializeField] GameObject bulletEffect;
    [SerializeField] CircleCollider2D col;
    [SerializeField] ParticleSystem particle;
    public void Initialization(Vector2 _dir, float _speed, float _scale, float _lifeTime, bool _isHoming = false)
    {
        this.currentDir = _dir;
        this.currentSpeed = _speed;
        this.currentLifeTime = _lifeTime;
        this.isHoming = _isHoming;
        this.bulletEffect.SetActive(true);
        if (_scale != -1)
        {
            var t_particle = particle.main;
            t_particle.startSize = 0.35f * _scale;
            col.radius = 0.5f * _scale;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameState == GameState.Paused) return;

        if (this.isHoming)
        {
            var t_overlap = Physics2D.OverlapCircleAll(
                this.transform.position,
                this.homingRad,
                LayerMask.GetMask("Enemy")
            );

            if (t_overlap != null && t_overlap.Length > 0)
            {
                Transform t_nearestTarget = null;
                float t_minSqrDist = float.MaxValue;

                Vector2 t_myPos = this.transform.position;

                foreach (var t in t_overlap)
                {
                    Vector2 t_targetPos = t.transform.position;
                    float t_sqrDist = (t_targetPos - t_myPos).sqrMagnitude;

                    if (t_sqrDist < t_minSqrDist)
                    {
                        t_minSqrDist = t_sqrDist;
                        t_nearestTarget = t.transform;
                    }
                }

                if (t_nearestTarget != null)
                {
                    Vector2 t_targetDir = ((Vector2)t_nearestTarget.position - t_myPos).normalized;

                    this.currentDir = Vector2.Lerp(
                        this.currentDir,
                        t_targetDir,
                       5f * Time.fixedDeltaTime
                    ).normalized;
                }
            }
        }

        this.transform.position += (Vector3)(this.currentDir * this.currentSpeed * Time.fixedDeltaTime);

        if (this.currentLifeTime <= 0f)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.currentLifeTime = Mathf.MoveTowards(this.currentLifeTime, 0f, Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Enemy>(out var t_enemy))
        {
            t_enemy.Hit(this.damage);
            Destroy(this.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.homingRad);
    }
}
