using UnityEngine;
using UnityEngine.UI;
using TMPro; // Usando TextMeshPro para textos mais bonitos!
using UnityEngine.SceneManagement;

public class SetupManager : MonoBehaviour
{
    [Header("Interface")]
    public TMP_InputField campoNome;
    public Button botaoConfirmar;

    [Header("Visuais de Seleção (Opcional)")]
    public Image fundoAvatarMasculino;
    public Image fundoAvatarFeminino;
    public Color corSelecionado = Color.green;
    public Color corNormal = Color.white;

    private int _avatarEscolhido = -1;

    private void Start()
    {
        if (PlayerPrefs.GetInt("CadastroCompleto", 0) == 1)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        botaoConfirmar.interactable = false;
    }

    public void SelecionarMasculino()
    {
        _avatarEscolhido = 0;


        if (fundoAvatarMasculino != null) fundoAvatarMasculino.color = corSelecionado;
        if (fundoAvatarFeminino != null) fundoAvatarFeminino.color = corNormal;

        VerificarSePodeConfirmar();
    }


    public void SelecionarFeminino()
    {
        _avatarEscolhido = 1;

        if (fundoAvatarMasculino != null) fundoAvatarMasculino.color = corNormal;
        if (fundoAvatarFeminino != null) fundoAvatarFeminino.color = corSelecionado;

        VerificarSePodeConfirmar();
    }

    public void VerificarSePodeConfirmar()
    {
        if (!string.IsNullOrEmpty(campoNome.text) && _avatarEscolhido != -1)
        {
            botaoConfirmar.interactable = true;
        }
        else
        {
            botaoConfirmar.interactable = false;
        }
    }

    public void ConfirmarEAvancar()
    {
        PlayerPrefs.SetString("NomeCrianca", campoNome.text);
        PlayerPrefs.SetInt("AvatarEscolhido", _avatarEscolhido);

        PlayerPrefs.SetInt("CadastroCompleto", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainMenu");
    }
}