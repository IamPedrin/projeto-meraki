using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MissaoPossivel
{
    public AlimentoSO alimentoData;
    public GameObject alimentoPrefab;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD do jogo")]
    public TextMeshProUGUI textoHUDTotal;
    public TextMeshProUGUI textoHUDMoedas;
    private int _totalHistorico = 0;

    [Header("Catálogo de Missões (Sorteio)")]
    public List<MissaoPossivel> listaDeAlimentosSaudaveis;

    [HideInInspector] public AlimentoSO alimentoObjetivo;
    [HideInInspector] public GameObject prefabAlimentoObjetivo;

    [Header("Cinemática da Balança")]
    public GameObject painelBalanca;
    public TextMeshProUGUI textoBalancaContagem;
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI textoPontuacaoFinal;
    public TextMeshProUGUI textoMoedasGanhas;

    [Header("Economia")]
    public int valorPorAlimento = 5;

    private void Awake()
    {
        Instance = this;
        SortearMissaoDaPartida();
    }

    private void Start()
    {
        AtualizarHUDMoedas();
        AtualizarTextoHUD();

        if (painelBalanca != null) painelBalanca.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }


    public void AdicionarPontoHUD()
    {
        _totalHistorico++;
        AtualizarTextoHUD();
    }

    public void SofrerPunicao()
    {
        if (_totalHistorico > 0)
        {
            _totalHistorico--;
            Debug.Log("Eca! Bateu no ultraprocessado. Perdeu 1 alimento.");
        }
        AtualizarTextoHUD();
    }

    private void AtualizarTextoHUD()
    {
        if (textoHUDTotal != null) textoHUDTotal.text = "Coletados: " + _totalHistorico;
    }

    public void AtualizarHUDMoedas()
    {
        if (textoHUDMoedas != null) textoHUDMoedas.text = "Moedas: " + BancoMoedas.ObterSaldo();
    }

    public void IniciarSequenciaDeVitoria()
    {
        StartCoroutine(RotinaBalanca());
    }

    private IEnumerator RotinaBalanca()
    {
        // 1. Ativa a tela da balança
        if (painelBalanca != null) painelBalanca.SetActive(true);

        // 2. Faz a contagem de 1 a 1 (O Efeito "Juice")
        for (int i = 1; i <= _totalHistorico; i++)
        {
            // Aqui futuramente você pode colocar o código para instanciar/tocar o som da frutinha caindo
            if (textoBalancaContagem != null) textoBalancaContagem.text = i.ToString();

            // Espera 0.4 segundos antes de cair a próxima fruta
            yield return new WaitForSeconds(0.4f);
        }

        // 3. Dá 2 segundos para a criança ver o resultado final na balança
        yield return new WaitForSeconds(2f);

        // 4. Desliga a balança e chama a tela final de pontuação
        if (painelBalanca != null) painelBalanca.SetActive(false);
        MostrarGameOver();
    }


    public void MostrarGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        textoPontuacaoFinal.text = "Você coletou: " + _totalHistorico + " alimentos!";

        if (alimentoObjetivo != null && _totalHistorico > 0)
        {
            InventarioManager.AdicionarAlimento(alimentoObjetivo.idUnico, _totalHistorico);
        }

        int lucroDaPartida = _totalHistorico * valorPorAlimento;
        if (lucroDaPartida > 0)
        {
            BancoMoedas.AdicionarMoedas(lucroDaPartida);
        }

        if (textoMoedasGanhas != null)
        {
            textoMoedasGanhas.text = "Bônus final: +" + lucroDaPartida + " moedas!";
        }
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SortearMissaoDaPartida()
    {
        if (listaDeAlimentosSaudaveis.Count > 0)
        {
            int sorteio = Random.Range(0, listaDeAlimentosSaudaveis.Count);

            alimentoObjetivo = listaDeAlimentosSaudaveis[sorteio].alimentoData;
            prefabAlimentoObjetivo = listaDeAlimentosSaudaveis[sorteio].alimentoPrefab;

            Debug.Log("A missão desta corrida é coletar: " + alimentoObjetivo.nomeAlimento);
        }
    }
}