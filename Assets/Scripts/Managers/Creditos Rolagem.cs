using UnityEngine;

public class CreditosRolagem : MonoBehaviour
{
    [Header("Configurações dos Créditos")]
    public RectTransform conteudoTexto;

    public float velocidade = 50f;

    private Vector2 _posicaoInicial;

    private void Awake()
    {
        if (conteudoTexto != null)
        {
            _posicaoInicial = conteudoTexto.anchoredPosition;
        }
    }

    private void OnEnable()
    {
        if (conteudoTexto != null)
        {
            conteudoTexto.anchoredPosition = _posicaoInicial;
        }
    }

    private void Update()
    {
        if (conteudoTexto != null)
        {
            conteudoTexto.anchoredPosition += Vector2.up * (velocidade * Time.unscaledDeltaTime);
        }
    }
}