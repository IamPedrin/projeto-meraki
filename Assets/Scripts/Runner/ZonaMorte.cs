using UnityEngine;

public class ZonaMorte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerRunner player = collision.GetComponent<PlayerRunner>();
            if (player != null)
            {
                player.GameOver();
            }
        }
        else if (collision.CompareTag("Follower"))
        {
            Follower f = collision.GetComponent<Follower>();
            if (f  != null && f.player != null)
            {
                f.player.filaSeguidores.Remove(f);
            }
            Destroy(collision.gameObject);
        }
    }
}
