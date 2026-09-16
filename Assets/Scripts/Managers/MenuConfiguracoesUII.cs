using UnityEngine;
using UnityEngine.UI;

public class MenuConfiguracoesUI : MonoBehaviour
{
    [Header("Sliders de Áudio")]
    public Slider sliderMaster;
    public Slider sliderMusica;
    public Slider sliderEfeitos;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            if (sliderMaster != null)
                sliderMaster.value = AudioManager.Instance.GetMasterVolume();
            
            if (sliderMusica != null)
                sliderMusica.value = AudioManager.Instance.GetMusicVolume();
            
            if (sliderEfeitos != null)
                sliderEfeitos.value = AudioManager.Instance.GetSFXVolume();
        }
    }

    public void AoMudarVolumeMaster(float valor)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.MasterVolume(valor);
    }

    public void AoMudarVolumeMusica(float valor)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.MusicVolume(valor);
    }

    public void AoMudarVolumeEfeitos(float valor)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SFXVolume(valor);
    }
}