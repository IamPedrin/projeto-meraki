using UnityEngine;

public class ZonaMorte : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D colisao)
    {
        if (colisao.CompareTag("Player"))
        {
            PlayerRunner player = colisao.GetComponent<PlayerRunner>();
            if (player != null)
            {
                player.GameOver();
            }
        }

        else if (colisao.CompareTag("Follower"))
        {
            Follower seguidor = colisao.GetComponent<Follower>();
            if (seguidor != null && seguidor.player != null)
            {

                seguidor.player.RemoverSeguidorQueCaiu(seguidor);
            }

            Destroy(colisao.gameObject);
        }
    }
}
