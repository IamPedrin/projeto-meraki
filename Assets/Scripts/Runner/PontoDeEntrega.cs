using TMPro;
using UnityEngine;

public class PontoDeEntrega : MonoBehaviour
{
    [Header("Configurações")]
    public int quantidadeNecessaria = 5;

    public int recompensaMoedas = 10;

    [Header("Interface")]
    public TextMeshPro textoExigencia;


    private bool _jaFoiTocado = false;

    private void Start()
    {
        if (textoExigencia != null)
        {
            textoExigencia.text = quantidadeNecessaria.ToString();
        }
    }

    private void OnTriggerEnter2D(Collider2D colisao)
    {
        if (_jaFoiTocado) return;

        PlayerRunner leader = null;

        if (colisao.CompareTag("Player"))
        {
            leader = colisao.GetComponent<PlayerRunner>();
        }
        else if (colisao.CompareTag("Follower"))
        {
            leader = colisao.GetComponent<Follower>().player;
        }

        if (leader != null)
        {
            int alimentosDoJogador = leader.ObterQuantidadeAtivos();

            if (alimentosDoJogador >= quantidadeNecessaria)
            {
                _jaFoiTocado = true;

                leader.EntregarAlimentos(quantidadeNecessaria);

                BancoMoedas.AdicionarMoedas(recompensaMoedas);

                Destroy(gameObject);
            }

        }
    }
}
