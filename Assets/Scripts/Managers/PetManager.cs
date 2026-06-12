using UnityEngine;
using System;
using UnityEngine.UI;

public class PetManager : MonoBehaviour
{
    public static PetManager Instancia;

    [Header("Status do Personagem")]
    public float energia = 100f;
    public float felicidade = 100f;

    [Header("Configurações")]
    public float perdaPorHora = 5f;

    [Header("Interface")]
    public Slider barraEnergia;
    public Slider barraFelicidade;

    private void Awake()
    {
        Instancia = this;
    }

    private void Start()
    {
        CarregarStatusSalvos();
        CalcularTempoOfflineEDescontar();
        AtualizarBarrinhas();

        // A cada 60 segundos (1 minuto), desconta um pouquinho de energia em tempo real
        InvokeRepeating("DrenoEmTempoReal", 60f, 60f);
    }

    private void CalcularTempoOfflineEDescontar()
    {
        string ultimaVezString = PlayerPrefs.GetString("UltimaVezJogado", "");

        if (!string.IsNullOrEmpty(ultimaVezString))
        {

            DateTime dataUltimaVez = DateTime.Parse(ultimaVezString);

            TimeSpan tempoPassado = DateTime.Now - dataUltimaVez;

            float horasOffline = (float)tempoPassado.TotalHours;

            float pontosPerdidos = horasOffline * perdaPorHora;

            energia -= pontosPerdidos;
            felicidade -= pontosPerdidos;

            if (energia < 0) energia = 0;
            if (felicidade < 0) felicidade = 0;
        }
    }

    private void DrenoEmTempoReal()
    {
        float perdaPorMinuto = perdaPorHora / 60f;

        energia -= perdaPorMinuto;
        felicidade -= perdaPorMinuto;

        if (energia < 0) energia = 0;
        if (felicidade < 0) felicidade = 0;

        AtualizarBarrinhas();
        SalvarStatus();
    }

    public void CuidarDoPersonagem(float ganhoEnergia, float ganhoFelicidade)
    {
        energia += ganhoEnergia;
        felicidade += ganhoFelicidade;
        if (energia > 100f) energia = 100f;
        if (felicidade > 100f) felicidade = 100f;

        AtualizarBarrinhas();
        SalvarStatus();
    }

    private void AtualizarBarrinhas()
    {
        if (barraEnergia != null) barraEnergia.value = energia;
        if (barraFelicidade != null) barraFelicidade.value = felicidade;
    }

    private void CarregarStatusSalvos()
    {
        energia = PlayerPrefs.GetFloat("TamagotchiEnergia", 100f);
        felicidade = PlayerPrefs.GetFloat("TamagotchiFelicidade", 100f);
    }

    private void SalvarStatus()
    {
        PlayerPrefs.SetFloat("TamagotchiEnergia", energia);
        PlayerPrefs.SetFloat("TamagotchiFelicidade", felicidade);

        PlayerPrefs.SetString("UltimaVezJogado", DateTime.Now.ToString());

        PlayerPrefs.Save();
    }

    //Testes para adicionar e diminuir energia e felicidade
    public void TesteAdicionarEnergia()
    {
        CuidarDoPersonagem(10f, 0f);
    }

    public void TesteAdicionarFelicidade()
    {
        CuidarDoPersonagem(0f, 10f);
    }

    public void TesteDiminuirEnergia()
    {
        CuidarDoPersonagem(-10f, 0f);
    }

    public void TesteDiminuirFelicidade()
    {
        CuidarDoPersonagem(0f, -10f);
    }

    private void OnApplicationQuit()
    {
        SalvarStatus();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused) SalvarStatus();
    }
}
