using UnityEngine;
using System.Collections.Generic;

public class InventarioManager : MonoBehaviour
{
    private const string CHAVE_LISTA_IDS = "Inv_AllKeys";

    public static void AdicionarAlimento(string idUnico, int quantidade = 1)
    {
        int quantidadeAtual = ObterQuantidade(idUnico);
        PlayerPrefs.SetInt("Inv_" + idUnico, quantidadeAtual + quantidade);

        RegistrarIdNaLista(idUnico);

        PlayerPrefs.Save();

        Debug.Log($"Adicionado: {quantidade}x {idUnico}. Total agora: {quantidadeAtual + quantidade}");
    }

    public static void RemoverAlimento(string idUnico, int quantidade = 1)
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

    public static Dictionary<string, int> ObterInventarioCompleto()
    {
        Dictionary<string, int> inventario = new Dictionary<string, int>();

        string idsSalvos = PlayerPrefs.GetString(CHAVE_LISTA_IDS, "");

        if (!string.IsNullOrEmpty(idsSalvos))
        {
            string[] ids = idsSalvos.Split(',');

            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    int qtd = ObterQuantidade(id);
                    if (qtd > 0)
                    {
                        inventario.Add(id, qtd);
                    }
                }
            }
        }

        return inventario;
    }

    private static void RegistrarIdNaLista(string idUnico)
    {
        string idsSalvos = PlayerPrefs.GetString(CHAVE_LISTA_IDS, "");

        if (!idsSalvos.Contains(idUnico))
        {
            if (idsSalvos.Length > 0)
            {
                idsSalvos += ",";
            }
            idsSalvos += idUnico;

            PlayerPrefs.SetString(CHAVE_LISTA_IDS, idsSalvos);
        }
    }
}
