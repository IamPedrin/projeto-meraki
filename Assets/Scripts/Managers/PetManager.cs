using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
public class PetManager : MonoBehaviour
{
    public static PetManager Instancia;

    [Header("Status do Avatar")]
    public float energiaAtual;
    public float energiaMaxima = 100f;

    [Header("Interface")]
    public Slider sliderEnergia;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
    }

    private void Start()
    {
        CarregarStatus();
        CalcularTempoOffline();
        AtualizarUI();

        InvokeRepeating("DrenarEnergia", 60f, 60f);
    }

    public void DarEnergia(float quantidade)
    {
        energiaAtual += quantidade;
        energiaAtual = Mathf.Clamp(energiaAtual, 0, energiaMaxima);

        AtualizarUI();
        SalvarStatus();
    }

    public void TirarEnergia(float quantidade)
    {
        energiaAtual -= quantidade;
        energiaAtual = Mathf.Clamp(energiaAtual, 0, energiaMaxima);

        AtualizarUI();
        SalvarStatus();
    }

    private void DrenarEnergia()
    {
        energiaAtual -= 2f;
        energiaAtual = Mathf.Clamp(energiaAtual, 0, energiaMaxima);
        AtualizarUI();
    }

    private void CalcularTempoOffline()
    {
        string tempoSalvo = PlayerPrefs.GetString("UltimoAcesso", "");
        if (!string.IsNullOrEmpty(tempoSalvo))
        {
            DateTime ultimoAcesso = DateTime.Parse(tempoSalvo);
            TimeSpan tempoPassado = DateTime.Now - ultimoAcesso;

            float horasPassadas = (float)tempoPassado.TotalHours;
            energiaAtual -= horasPassadas * 5f;

            energiaAtual = Mathf.Clamp(energiaAtual, 0, energiaMaxima);
        }
    }


    public void SalvarStatus()
    {
        PlayerPrefs.SetFloat("EnergiaAvatar", energiaAtual);

        PlayerPrefs.SetString("UltimoAcesso", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    private void CarregarStatus()
    {
        energiaAtual = PlayerPrefs.GetFloat("EnergiaAvatar", energiaMaxima);
    }

    private void AtualizarUI()
    {
        if (sliderEnergia != null)
        {
            sliderEnergia.DOValue(energiaAtual / energiaMaxima, 1f).SetEase(Ease.OutCubic);
        }
    }

    private void OnApplicationQuit()
    {
        SalvarStatus();
    }

    // //Testes para adicionar e diminuir energia e felicidade
    // public void TesteAdicionarEnergia()
    // {
    //     CuidarDoPersonagem(10f, 0f);
    // }

    // public void TesteAdicionarFelicidade()
    // {
    //     CuidarDoPersonagem(0f, 10f);
    // }

    // public void TesteDiminuirEnergia()
    // {
    //     CuidarDoPersonagem(-10f, 0f);
    // }

    // public void TesteDiminuirFelicidade()
    // {
    //     CuidarDoPersonagem(0f, -10f);
    // }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused) SalvarStatus();
    }
}
