using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

        Transform panelCard = painelDePausa.transform.Find("Panel");
        if (panelCard != null)
        {
            panelCard.DOKill();
            panelCard.localScale = Vector3.one * 0.85f;
            panelCard.DOScale(Vector3.one, 0.28f).SetEase(Ease.OutBack).SetUpdate(true);
        }
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("Runner");
    }
}
