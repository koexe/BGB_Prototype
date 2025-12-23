using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] float spawnCoolTime = 2.0f;
    [SerializeField] int spawnAmout = 15;
    [SerializeField] int spawnCount;
    [SerializeField] float spawnRad = 10;

    [SerializeField] float currentSpawnCooltime;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            this.spawnCount = 0;
    }

    private void FixedUpdate()
    {
        if (GameManagerr.instance.gameState == GameState.Paused) return;

        if (this.spawnCount >= spawnAmout) return;

        if (this.currentSpawnCooltime == 0)
        {
            Vector2 t_center = GameManagerr.instance.playerScript.transform.position;

            float t_angle = Random.Range(0f, Mathf.PI * 2f);

            Vector2 t_pos = t_center + new Vector2(
                Mathf.Cos(t_angle),
                Mathf.Sin(t_angle)
            ) * this.spawnRad;

            Instantiate(this.enemyPrefab, t_pos, Quaternion.identity);
            this.spawnCount += 1;
            this.currentSpawnCooltime = spawnCoolTime;
        }
        else
        {
            this.currentSpawnCooltime = Mathf.MoveTowards(this.currentSpawnCooltime, 0, Time.fixedDeltaTime);
        }
    }
}
