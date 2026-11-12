using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public GameObject hudDoJogo;

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
        if (hudDoJogo != null && alimentoObjetivo != null)
        {
            var imgIconeHud = hudDoJogo.transform.Find("MolduraColetadosHUD/IconeAlimentoHUD")?.GetComponent<Image>();
            if (imgIconeHud != null) imgIconeHud.sprite = alimentoObjetivo.iconeVisual;
        }

        AtualizarHUDMoedas();
        AtualizarTextoHUD();

        if (painelBalanca != null) painelBalanca.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void AdicionarPontoHUD()
    {
        _totalHistorico++;
        AtualizarTextoHUD();
        if (textoHUDTotal != null)
        {
            textoHUDTotal.transform.DOKill();
            textoHUDTotal.transform.localScale = Vector3.one;
            textoHUDTotal.transform.DOPunchScale(Vector3.one * 0.22f, 0.22f, 8, 1f);
        }
    }

    public void SofrerPunicao()
    {
        if (_totalHistorico > 0)
        {
            _totalHistorico--;
            Debug.Log("Bateu no ultraprocessado. Perdeu 1 alimento.");
        }
        AtualizarTextoHUD();
        if (textoHUDTotal != null)
        {
            textoHUDTotal.transform.DOKill();
            textoHUDTotal.transform.localScale = Vector3.one;
            textoHUDTotal.transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0f), 0.25f, 8, 1f);
        }
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
        if (painelBalanca != null)
        {
            painelBalanca.SetActive(true);
            var card = painelBalanca.transform.Find("CardBalanca");
            if (card != null)
            {
                card.DOKill();
                card.localScale = Vector3.one * 0.75f;
                card.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack).SetUpdate(true);
            }
        }

        if (textoBalancaContagem != null)
        {
            textoBalancaContagem.text = "0";
        }

        if (localDasFrutasNaBalanca != null)
        {
            foreach (Transform child in localDasFrutasNaBalanca)
            {
                Destroy(child.gameObject);
            }
        }

        Transform imgBalanca = painelBalanca != null
            ? painelBalanca.transform.Find("CardBalanca/MolduraVisorBalanca/ImageBalanca")
            : null;
        Transform badgeNumero = painelBalanca != null
            ? painelBalanca.transform.Find("CardBalanca/MolduraContagemBalanca/BadgeNumero")
            : null;

        yield return new WaitForSeconds(0.35f);

        for (int i = 1; i <= _totalHistorico; i++)
        {
            if (textoBalancaContagem != null)
            {
                textoBalancaContagem.text = i.ToString();
                Transform alvoPunch = badgeNumero != null ? badgeNumero : textoBalancaContagem.transform;
                alvoPunch.DOKill();
                alvoPunch.localScale = Vector3.one;
                alvoPunch.DOPunchScale(Vector3.one * 0.22f, 0.25f, 8, 1f).SetUpdate(true);
            }

            if (prefabIconeFruta != null && localDasFrutasNaBalanca != null && alimentoObjetivo != null)
            {
                GameObject icone = Instantiate(prefabIconeFruta, localDasFrutasNaBalanca);
                Image imgIcone = icone.GetComponent<Image>();
                if (imgIcone != null)
                {
                    imgIcone.sprite = alimentoObjetivo.iconeVisual;
                    imgIcone.preserveAspect = true;
                }

                icone.transform.localScale = Vector3.zero;
                icone.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
            }

            if (imgBalanca != null)
            {
                imgBalanca.DOKill();
                imgBalanca.localScale = Vector3.one;
                imgBalanca.localRotation = Quaternion.identity;
                imgBalanca.DOPunchScale(new Vector3(0.12f, -0.14f, 0f), 0.28f, 8, 0.8f).SetUpdate(true);
                imgBalanca.DOPunchRotation(new Vector3(0f, 0f, (i % 2 == 0 ? 5f : -5f)), 0.28f, 8, 0.8f).SetUpdate(true);
            }

            yield return new WaitForSeconds(0.35f);
        }

        if (badgeNumero != null)
        {
            badgeNumero.DOPunchScale(Vector3.one * 0.3f, 0.45f, 10, 1f).SetUpdate(true);
        }

        yield return new WaitForSeconds(1.6f);

        if (painelBalanca != null) painelBalanca.SetActive(false);

        MostrarGameOver(true);
    }

    public void MostrarGameOver(bool jogadorVenceu)
    {
        if (hudDoJogo != null) hudDoJogo.SetActive(false);
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);

        Transform cardGo = gameOverPanel.transform.Find("CardGameOver");
        if (cardGo != null)
        {
            cardGo.DOKill();
            cardGo.localScale = Vector3.one * 0.75f;
            cardGo.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack).SetUpdate(true);

            var tituloTmp = cardGo.Find("FaixaTituloGameOver/TituloGameOver")?.GetComponent<TextMeshProUGUI>();
            if (tituloTmp != null)
            {
                tituloTmp.text = jogadorVenceu ? "CORRIDA CONCLUÍDA!" : "FIM DE JOGO!";
            }
        }

        if (jogadorVenceu == true)
        {
            textoPontuacaoFinal.text = "Você coletou: " + _totalHistorico + " alimentos!\nIrão direto para a despensa da sua cozinha!";

            if (alimentoObjetivo != null && _totalHistorico > 0)
            {
                InventarioManager.AdicionarAlimento(alimentoObjetivo.idUnico, _totalHistorico);
            }
        }
        else
        {
            textoPontuacaoFinal.text = "Ops, você caiu! Mas conseguiu coletar: " + _totalHistorico + " alimentos.";
        }

        int lucroDaPartida = _totalHistorico * valorPorAlimento;
        if (lucroDaPartida > 0)
        {
            BancoMoedas.AdicionarMoedas(lucroDaPartida);
        }

        if (textoMoedasGanhas != null)
        {
            int valorAnimado = 0;
            textoMoedasGanhas.text = "Bônus final: +0 moedas!";
            DOTween.To(() => valorAnimado, v =>
            {
                valorAnimado = v;
                if (textoMoedasGanhas != null)
                    textoMoedasGanhas.text = "Bônus final: +" + valorAnimado + " moedas!";
            }, lucroDaPartida, 0.75f).SetEase(Ease.OutQuad).SetUpdate(true);

            Transform molduraBonus = textoMoedasGanhas.transform.parent;
            if (molduraBonus != null)
            {
                molduraBonus.DOKill();
                molduraBonus.localScale = Vector3.one;
                molduraBonus.DOPunchScale(Vector3.one * 0.14f, 0.45f, 8, 1f).SetDelay(0.25f).SetUpdate(true);
            }
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