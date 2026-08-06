using UnityEngine;

public enum CategoriaAlimento 
{ 
    InNatura, 
    MinimamenteProcessado, 
    Processado, 
    Ultraprocessado 
}

[CreateAssetMenu(fileName = "AlimentoSO", menuName = "Scriptable Objects/AlimentoSO")]
public class AlimentoSO : ScriptableObject
{
    [Header("Informações Principais")]
    public string idUnico;
    public string nomeAlimento;
    public Sprite iconeVisual;
    public int precoMoedas;
    public float energiaRestaurada = 15f;

    [Header("Detalhes Nutricionais (Para o Popup)")]
    public CategoriaAlimento categoria;

    [TextArea(3, 5)]
    public string descricao;
}

