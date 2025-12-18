using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerScript>(out var t_player))
        {
            t_player.FullMag();
            Destroy(this.gameObject);
        }
    }
}
