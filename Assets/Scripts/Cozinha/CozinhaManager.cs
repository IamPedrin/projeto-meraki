using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    [Header("Animação do PET")]
    public Transform bocaDoPet;
    public Animator animatorPet;

    public static CozinhaManager Instancia;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
    }

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

        Dictionary<string, int> itensNoPrato = new Dictionary<string, int>();
        foreach (SlotPrato slot in slotsDoPrato)
        {
            if (slot.alimentoNesteSlot != null)
            {
                string id = slot.alimentoNesteSlot.idUnico;
                if (itensNoPrato.ContainsKey(id)) itensNoPrato[id]++;
                else itensNoPrato[id] = 1;
            }
        }

        Dictionary<string, int> itensNoMouse = new Dictionary<string, int>();
        foreach (string idTransito in ItemArrastavel.itensEmTransito)
        {
            if (itensNoMouse.ContainsKey(idTransito)) itensNoMouse[idTransito]++;
            else itensNoMouse[idTransito] = 1;
        }

        Dictionary<string, int> inventario = InventarioManager.ObterInventarioCompleto();

        if (inventario != null)
        {
            foreach (var item in inventario)
            {
                string id = item.Key;
                int quantidadeTotal = item.Value;

                int qtdPrato = itensNoPrato.ContainsKey(id) ? itensNoPrato[id] : 0;
                int qtdMouse = itensNoMouse.ContainsKey(id) ? itensNoMouse[id] : 0;

                int quantidadeDisponivel = quantidadeTotal - qtdPrato - qtdMouse;

                if (quantidadeDisponivel > 0)
                {
                    GameObject novoItem = Instantiate(prefabItemArrastavel, painelDespensa);
                    ItemArrastavel scriptArrastavel = novoItem.GetComponent<ItemArrastavel>();

                    AlimentoSO alimentoDado = bancoDeDadosAlimentos.Find(x => x.idUnico == id);
                    if (alimentoDado != null)
                    {
                        novoItem.GetComponent<Image>().sprite = alimentoDado.iconeVisual;
                        scriptArrastavel.alimentoData = alimentoDado;
                        scriptArrastavel.despensaTransform = painelDespensa;

                        scriptArrastavel.AtualizarQuantidade(quantidadeDisponivel);
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
        }

        foreach (AlimentoSO comida in alimentosParaConsumir)
        {
            InventarioManager.RemoverAlimento(comida.idUnico, 1);
        }

        PetRoomMovement petWander = null;
        if (animatorPet != null)
        {
            petWander = animatorPet.GetComponent<PetRoomMovement>();
            if (petWander != null) petWander.PausarPasseio();
        }

        float tempoEspera = 0.8f;
        float tempoVoo = 1.2f;
        bool acaoRealizada = false;

        foreach (SlotPrato slot in slotsDoPrato)
        {
            if (slot.transform.childCount > 0)
            {
                Transform iconeComida = slot.transform.GetChild(0);
                iconeComida.SetParent(painelCozinha.transform.parent);

                if (bocaDoPet != null && Camera.main != null)
                {
                    Vector3 posicaoBocaNaTela = Camera.main.WorldToScreenPoint(bocaDoPet.position);
                    iconeComida.DOMove(posicaoBocaNaTela, tempoVoo).SetDelay(tempoEspera).SetEase(Ease.InOutSine);
                }

                iconeComida.DOScale(Vector3.zero, tempoVoo).SetDelay(tempoEspera).SetEase(Ease.InOutSine).OnComplete(() =>
                {

                    if (!acaoRealizada)
                    {
                        acaoRealizada = true;

                        if (animatorPet != null) animatorPet.SetTrigger("isEating");
                        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("nhac");

                        PetManager.Instancia.DarEnergia(energiaTotal);

                        if (petWander != null) petWander.RetomarPasseio();
                    }

                    Destroy(iconeComida.gameObject);
                });
            }

            slot.alimentoNesteSlot = null;
        }

        FecharCozinha();
        AtualizarDespensa();
    }
}
