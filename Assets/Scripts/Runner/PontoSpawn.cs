using UnityEngine;

public class PontoSpawn : MonoBehaviour
{
// Criamos uma lista de opções para você escolher no Inspector!
    public enum TipoDePonto { AlimentoBom, ObstaculoRuim }

    [Header("Configuração do Level Design")]
    [Tooltip("Escolha se este ponto específico vai gerar a comida da missão ou um ultraprocessado.")]
    public TipoDePonto tipoDestePonto;

    [Header("Lista de Obstáculos")]
    public GameObject[] prefabsUltraprocessados;

    // Roda toda vez que o pedaço de chão aparece na frente do jogador
    private void OnEnable() 
    {
        // 1. Limpa os itens antigos que ficaram da última vez que esse chão passou
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 2. Gera o item exato que você planejou para este local
        GerarItem();
    }

    private void GerarItem()
    {
        GameObject prefabParaGerar = null;

        // Se você marcou no Inspector que aqui nasce a recompensa:
        if (tipoDestePonto == TipoDePonto.AlimentoBom)
        {
            if (GameManager.Instance != null && GameManager.Instance.prefabAlimentoObjetivo != null)
            {
                // Ele puxa exatamente a fruta/legume que foi sorteada para essa partida!
                prefabParaGerar = GameManager.Instance.prefabAlimentoObjetivo;
            }
        }
        // Se você marcou que aqui é um obstáculo que a criança precisa pular:
        else if (tipoDestePonto == TipoDePonto.ObstaculoRuim)
        {
            if (prefabsUltraprocessados.Length > 0)
            {
                // O obstáculo ainda é aleatório (pode ser um refri ou um hambúrguer), 
                // mas você tem certeza de que ele vai nascer NESSE lugar exato.
                int indexAleatorio = Random.Range(0, prefabsUltraprocessados.Length);
                prefabParaGerar = prefabsUltraprocessados[indexAleatorio];
            }
        }

        // 3. Cria o item e "gruda" ele neste ponto do chão
        if (prefabParaGerar != null)
        {
            Instantiate(prefabParaGerar, transform.position, Quaternion.identity, transform);
        }
    }
}
