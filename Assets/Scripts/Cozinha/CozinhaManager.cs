using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CozinhaManager : MonoBehaviour
{
    public GameObject painelCozinha;

    [Header("Configurações do Prato")]
    public SlotPrato[] slotsDoPrato;
    public Button botaoServir;
    public float bonusDeVariedade = 15f;

    [Header("Despensa")]
    public Transform painelDespensa;
    public GameObject prefabItemArrastavel;
    public List<AlimentoSO> bancoDeDadosAlimentos;

    private void Start()
    {
        AtualizarDespensa();

        if (botaoServir != null)
        {
            botaoServir.onClick.AddListener(ServirRefeicao);
        }
    }

    public void AbrirCozinha()
    {
        painelCozinha.SetActive(true);
        AtualizarDespensa();
    }

    public void FecharCozinha()
    {
        painelCozinha.SetActive(false);
    }

    public void AtualizarDespensa()
    {
        foreach (Transform child in painelDespensa)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, int> inventario = InventarioManager.ObterInventarioCompleto();

        if (inventario != null)
        {
            foreach (var item in inventario)
            {
                string id = item.Key;
                int quantidade = item.Value;

                if (quantidade > 0)
                {
                    AlimentoSO alimentoDado = bancoDeDadosAlimentos.Find(x => x.idUnico == id);

                    if (alimentoDado != null)
                    {
                        for (int i = 0; i < quantidade; i++)
                        {
                            GameObject novoItem = Instantiate(prefabItemArrastavel, painelDespensa);

                            novoItem.GetComponent<Image>().sprite = alimentoDado.iconeVisual;
                            novoItem.GetComponent<ItemArrastavel>().alimentoData = alimentoDado;
                        }
                    }
                }
            }
        }
    }

    private void ServirRefeicao()
    {
        float energiaTotal = 0f;
        List<AlimentoSO> alimentosParaConsumir = new List<AlimentoSO>();

        HashSet<string> tiposDiferentes = new HashSet<string>();

        foreach (SlotPrato slot in slotsDoPrato)
        {
            if (slot.alimentoNesteSlot != null)
            {
                alimentosParaConsumir.Add(slot.alimentoNesteSlot);
                tiposDiferentes.Add(slot.alimentoNesteSlot.idUnico);
                energiaTotal += slot.alimentoNesteSlot.energiaRestaurada;
            }
        }

        if (alimentosParaConsumir.Count == 0)
        {
            Debug.Log("O prato está vazio!");
            return;
        }

        if (tiposDiferentes.Count >= 3)
        {
            energiaTotal += bonusDeVariedade;
            Debug.Log("Prato Colorido! +" + bonusDeVariedade + " de Bônus!");
        }

        foreach (AlimentoSO comida in alimentosParaConsumir)
        {
            InventarioManager.RemoverAlimento(comida.idUnico, 1);
        }

        PetManager.Instancia.DarEnergia(energiaTotal);
        Debug.Log("Refeição servida! Energia recebida: " + energiaTotal);

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("nhac");

        foreach (SlotPrato slot in slotsDoPrato)
        {
            slot.EsvaziarSlot();
        }

        AtualizarDespensa();
    }
}
