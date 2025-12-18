using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int hp;
    [SerializeField] Vector2 currentDir;
    [SerializeField] float speed;
    [SerializeField] GameObject itemPrefab;
    private void FixedUpdate()
    {
        if (GameManager.instance.gameState == GameState.Paused) return;
        var t_dir = GameManager.instance.playerScript.transform.position - this.transform.position;

        this.transform.position += t_dir.normalized * this.speed * Time.fixedDeltaTime;
    }

    public void Hit(int _damage)
    {
        this.hp -= _damage;


        if (this.hp <= 0)
        {
            GameManager.instance.AddKillCount();
            Instantiate(itemPrefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

    }
}
