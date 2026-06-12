using UnityEngine;

public class ObstaculoMalsAlimentos : MonoBehaviour
{
    [Header("Configurações do Obstáculo")]
    public int dano = 2;

    [Header("Efeitos Visuais")]
    public GameObject efeitoExplosao;
    private bool _jaFoiDestruido = false;

    private void OnTriggerEnter2D(Collider2D colisao)
    {
        if (_jaFoiDestruido) return;

        PlayerRunner leader = null;

        if (colisao.CompareTag("Player"))
        {
            leader = colisao.GetComponent<PlayerRunner>();
        }
        else if (colisao.CompareTag("Follower"))
        {
            Follower seguidor = colisao.GetComponent<Follower>();
            if (seguidor != null)
            {
                leader = seguidor.player;
            }
        }

        if (leader != null)
        {
            _jaFoiDestruido = true;

            leader.PerderSeguidores(dano);

            if (efeitoExplosao != null)
            {
                Instantiate(efeitoExplosao, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
