using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject painelDePausa;
    public string nomeSceneMainMenu = "MainMenu";
    public GameObject hudDoJogo;
    private void Start()
    {
        painelDePausa.SetActive(false);
        //Time.timeScale = 1f;
    }

    public void PausarGame()
    {
        if (hudDoJogo != null) hudDoJogo.SetActive(false);

        painelDePausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinuarGame()
    {
        if (hudDoJogo != null) hudDoJogo.SetActive(true);

        painelDePausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void VoltarMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeSceneMainMenu);
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo...");
        Application.Quit();
    }
    public void ReiniciarJogo()
    {
        SceneManager.LoadScene("Runner");
    }
}
