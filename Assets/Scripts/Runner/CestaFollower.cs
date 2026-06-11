using TMPro;
using UnityEngine;

public class CestaFollower : Follower
{
    [Header("Interface da Cesta")]
    public TextMeshPro textoQuantidade;

    public void AtualizarNumero(int quantidade)
    {
        if (textoQuantidade != null)
        {
            textoQuantidade.text = "x" + quantidade;
        }
    }
}
