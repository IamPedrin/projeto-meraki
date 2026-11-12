using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Aplica regras de encaixe de texto (TextMeshPro UGUI) na cena aberta ou no prefab em edição.
/// Idempotente: pode rodar quantas vezes quiser.
/// </summary>
public static class AjustarTextosUI
{
    const float TamanhoMinimoAbsoluto = 14f;
    const float FracaoMinima = 0.45f;

    [MenuItem("Meraki/UI/Ajustar textos da cena ou prefab aberto")]
    public static void AjustarMenu()
    {
        int n = AjustarTudo();
        Debug.Log($"[AjustarTextosUI] {n} textos ajustados.");
    }

    public static int AjustarTudo()
    {
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        TextMeshProUGUI[] textos = stage != null
            ? stage.prefabContentsRoot.GetComponentsInChildren<TextMeshProUGUI>(true)
            : Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include);

        int n = 0;
        foreach (var t in textos)
            if (Ajustar(t)) n++;

        if (stage != null) EditorSceneManager.MarkSceneDirty(stage.scene);
        else EditorSceneManager.MarkAllScenesDirty();
        return n;
    }

    public static bool Ajustar(TextMeshProUGUI t)
    {
        if (DeveIgnorar(t)) return false;

        Undo.RecordObject(t, "Ajustar texto UI");

        if (!t.enableAutoSizing)
        {
            float max = t.fontSize;
            t.fontSizeMax = max;
            t.fontSizeMin = Mathf.Min(max, Mathf.Max(TamanhoMinimoAbsoluto, Mathf.Round(max * FracaoMinima)));
            t.enableAutoSizing = true;
        }

        t.textWrappingMode = TextWrappingModes.Normal;
        t.overflowMode = TextOverflowModes.Ellipsis;

        // Margem interna quando o texto preenche uma caixa/botão (não encosta na borda do sprite).
        var rt = t.rectTransform;
        bool preenchePai = rt.anchorMin == Vector2.zero && rt.anchorMax == Vector2.one;
        var pai = rt.parent;
        if (preenchePai && pai != null && pai.GetComponent<UnityEngine.UI.Image>() != null && t.margin == Vector4.zero)
            t.margin = new Vector4(12, 6, 12, 6);

        EditorUtility.SetDirty(t);
        return true;
    }

    static bool DeveIgnorar(TextMeshProUGUI t)
    {
        // Textos que crescem com o conteúdo (créditos com rolagem, ContentSizeFitter) não devem auto-ajustar.
        if (t.GetComponent<UnityEngine.UI.ContentSizeFitter>() != null) return true;
        for (var p = t.transform.parent; p != null; p = p.parent)
            if (p.GetComponent("CreditosRolagem") != null) return true;
        return false;
    }
}

