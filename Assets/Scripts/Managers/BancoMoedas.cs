using UnityEngine;

public class BancoMoedas : MonoBehaviour
{
    // A chave para salvar no celular
    private const string CHAVE_MOEDAS = "MoedasTotais";

    public static int ObterSaldo()
    {
        return PlayerPrefs.GetInt(CHAVE_MOEDAS, 0);
    }

    public static void AdicionarMoedas(int quantidade)
    {
        int saldoAtual = ObterSaldo();
        saldoAtual += quantidade;

        PlayerPrefs.SetInt(CHAVE_MOEDAS, saldoAtual);
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AtualizarHUDMoedas();
        }
    }

    public static bool GastarMoedas(int quantidade)
    {
        int saldoAtual = ObterSaldo();

        if (saldoAtual >= quantidade)
        {
            saldoAtual -= quantidade;
            PlayerPrefs.SetInt(CHAVE_MOEDAS, saldoAtual);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }
}
