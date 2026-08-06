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

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI textoPontuacaoFinal;
    public TextMeshProUGUI textoMoedasGanhas;

    [Header("Economia")]
    public int valorPorAlimento = 5;

    [Header("UI da Balança")]
    public GameObject painelBalanca;
    public Transform localDasFrutasNaBalanca;
    public GameObject prefabIconeFruta;
    public TextMeshProUGUI textoBalancaContagem;

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
            Debug.Log("Bateu no ultraprocessado. Perdeu 1 alimento.");
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
        if (painelBalanca != null) painelBalanca.SetActive(true);

        if (localDasFrutasNaBalanca != null)
        {
            foreach (Transform child in localDasFrutasNaBalanca)
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 1; i <= _totalHistorico; i++)
        {
            if (textoBalancaContagem != null) textoBalancaContagem.text = i.ToString();

            if (prefabIconeFruta != null && localDasFrutasNaBalanca != null && alimentoObjetivo != null)
            {
                GameObject icone = Instantiate(prefabIconeFruta, localDasFrutasNaBalanca);
                icone.GetComponent<Image>().sprite = alimentoObjetivo.iconeVisual;
            }

            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(2f);

        if (painelBalanca != null) painelBalanca.SetActive(false);
        
        MostrarGameOver(true);
    }

    public void MostrarGameOver(bool jogadorVenceu)
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        
        if (jogadorVenceu == true)
        {
            textoPontuacaoFinal.text = "Você coletou: " + _totalHistorico + " alimentos!\nIrão direto para o despensa da sua cozinha!";
            
            if (alimentoObjetivo != null && _totalHistorico > 0)
            {
                InventarioManager.AdicionarAlimento(alimentoObjetivo.idUnico, _totalHistorico);
            }
        }
        else
        {
            textoPontuacaoFinal.text = "Ops, você caiu! Mas conseguiu: " + _totalHistorico + " alimentos.";
        }

        int lucroDaPartida = _totalHistorico * valorPorAlimento;
        if (lucroDaPartida > 0)
        {
            BancoMoedas.AdicionarMoedas(lucroDaPartida);
        }

        if (textoMoedasGanhas != null)
        {
            textoMoedasGanhas.text = "\nBônus final: +" + lucroDaPartida + " moedas!";
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