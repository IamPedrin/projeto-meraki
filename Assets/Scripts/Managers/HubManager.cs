using UnityEngine;
using UnityEngine.SceneManagement;

public class HubManager : MonoBehaviour
{
    [SerializeField] private string SceneRunner = "Runner";
    [SerializeField] private GameObject minigamesPanel;
    [SerializeField] private GameObject hubPanel;
    void Start()
    {
        AudioManager.Instance.PlayMusic("musica");
        BancoMoedas.AdicionarMoedas(1000);
        PetManager.Instancia.TirarEnergia(20f);
    }

    public void OpenMinigames()
    {
        minigamesPanel.SetActive(true);
        //hubPanel.SetActive(false);
    }

    public void CloseMinigames()
    {
        minigamesPanel.SetActive(false);
        //hubPanel.SetActive(true);
    }

    public void PlayRunner()
    {
        SceneManager.LoadScene(SceneRunner);
    }


    public void Quit()
    {
        Debug.Log("Quiting...");
        Application.Quit();
    }
}
