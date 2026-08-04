using UnityEngine;

public class InventarioManager : MonoBehaviour
{
    public static void AdicionarAlimento(string idUnico, int quantidade = 1)
    {
        int quantidadeAtual = ObterQuantidade(idUnico);
        PlayerPrefs.SetInt("Inv_" + idUnico, quantidadeAtual + quantidade);
        PlayerPrefs.Save();
        
        Debug.Log($"Adicionado: {quantidade}x {idUnico}. Total agora: {quantidadeAtual + quantidade}");
    }

    public static void ConsumirAlimento(string idUnico, int quantidade = 1)
    {
        int quantidadeAtual = ObterQuantidade(idUnico);
        int novaQuantidade = Mathf.Max(0, quantidadeAtual - quantidade);
        PlayerPrefs.SetInt("Inv_" + idUnico, novaQuantidade);
        PlayerPrefs.Save();
    }

    public static int ObterQuantidade(string idUnico)
    {
        return PlayerPrefs.GetInt("Inv_" + idUnico, 0);
    }
}
