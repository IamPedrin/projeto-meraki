using UnityEngine;

public class ColetarAlimento : MonoBehaviour
{
    public TipoAlimento tipoItem;
    public Follower prefabFollower;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerRunner player = collision.GetComponent<PlayerRunner>();
            if (player != null)
            {
                AudioManager.Instance.PlaySFX("coletar");
                player.AdicionarSeguidor(prefabFollower, tipoItem);

                gameObject.SetActive(false);
            }
        }
    }
}
