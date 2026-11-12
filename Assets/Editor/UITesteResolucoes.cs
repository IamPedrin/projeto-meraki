using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Testes visuais de UI: renderiza o Canvas da cena aberta em várias resoluções e
/// verifica textos que não cabem e elementos fora da tela. Não salva nada na cena.
/// </summary>
public static class UITesteResolucoes
{
    public static readonly Vector2Int[] Resolucoes =
    {
        new Vector2Int(1920, 1080), // 16:9
        new Vector2Int(2400, 1080), // 20:9 (celular moderno)
        new Vector2Int(2048, 1536), // 4:3 (tablet)
    };

    /// <summary>Renderiza o primeiro Canvas raiz da cena aberta. Retorna o relatório de problemas.</summary>
    public static string Capturar(string pastaSaida, string prefixo, Vector2Int res)
    {
        Directory.CreateDirectory(pastaSaida);
        var canvas = EncontrarCanvas();
        var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();

        var modoOriginal = canvas.renderMode;
        bool scalerOriginal = scaler != null && scaler.enabled;

        var camGo = new GameObject("__CamTesteUI") { hideFlags = HideFlags.HideAndDontSave };
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1);
        cam.cullingMask = 1 << canvas.gameObject.layer;
        var rtTex = new RenderTexture(res.x, res.y, 24);
        cam.targetTexture = rtTex;

        var sb = new StringBuilder();
        try
        {
            if (scaler) scaler.enabled = false;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 10;
            float match = scaler ? scaler.matchWidthOrHeight : 0.5f;
            Vector2 refRes = scaler ? scaler.referenceResolution : new Vector2(1920, 1080);
            float lw = Mathf.Log(res.x / refRes.x, 2), lh = Mathf.Log(res.y / refRes.y, 2);
            canvas.scaleFactor = Mathf.Pow(2, Mathf.Lerp(lw, lh, match));

            Canvas.ForceUpdateCanvases();
            foreach (var t in canvas.GetComponentsInChildren<TMP_Text>(false)) t.ForceMeshUpdate(true);
            Canvas.ForceUpdateCanvases();

            cam.Render();
            var prev = RenderTexture.active;
            RenderTexture.active = rtTex;
            var tex = new Texture2D(res.x, res.y, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, res.x, res.y), 0, 0);
            tex.Apply();
            RenderTexture.active = prev;
            File.WriteAllBytes(Path.Combine(pastaSaida, $"{prefixo}_{res.x}x{res.y}.png"), tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            Verificar(canvas, sb);
        }
        finally
        {
            canvas.renderMode = modoOriginal;
            canvas.worldCamera = null;
            if (scaler) scaler.enabled = scalerOriginal;
            cam.targetTexture = null;
            Object.DestroyImmediate(rtTex);
            Object.DestroyImmediate(camGo);
        }
        return sb.ToString();
    }

    static Canvas EncontrarCanvas()
    {
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include))
            if (c.isRootCanvas) return c;
        return null;
    }

    static void Verificar(Canvas canvas, StringBuilder sb)
    {
        var raiz = (RectTransform)canvas.transform;
        var cantos = new Vector3[4];
        raiz.GetWorldCorners(cantos);
        var limite = new Rect(cantos[0], cantos[2] - cantos[0]);
        float tol = limite.width * 0.002f;

        foreach (var t in canvas.GetComponentsInChildren<TMP_Text>(false))
        {
            if (string.IsNullOrWhiteSpace(t.text)) continue;
            if (t.isTextOverflowing || t.isTextTruncated)
                sb.AppendLine($"  TEXTO NAO CABE: {Caminho(t.transform)} '{Resumo(t.text)}'");
            if (t.enableAutoSizing && t.fontSize <= t.fontSizeMin + 0.01f && t.fontSizeMin < t.fontSizeMax)
                sb.AppendLine($"  TEXTO NO TAMANHO MINIMO ({t.fontSize:0}): {Caminho(t.transform)}");
        }

        foreach (var g in canvas.GetComponentsInChildren<UnityEngine.UI.Graphic>(false))
        {
            if (g.transform == raiz) continue;
            if (EstaDentroDeMascara(g.transform)) continue;
            g.rectTransform.GetWorldCorners(cantos);
            var r = new Rect(cantos[0], cantos[2] - cantos[0]);
            if (r.width <= 0.01f || r.height <= 0.01f) continue;
            if (r.xMin < limite.xMin - tol || r.yMin < limite.yMin - tol || r.xMax > limite.xMax + tol || r.yMax > limite.yMax + tol)
                sb.AppendLine($"  FORA DA TELA: {Caminho(g.transform)}");
        }
    }

    static bool EstaDentroDeMascara(Transform t)
    {
        for (var p = t.parent; p != null; p = p.parent)
            if (p.GetComponent<UnityEngine.UI.Mask>() || p.GetComponent<UnityEngine.UI.RectMask2D>()) return true;
        return false;
    }

    static string Resumo(string s) => s.Replace("\n", " ").Length > 30 ? s.Replace("\n", " ").Substring(0, 30) + "…" : s.Replace("\n", " ");

    static string Caminho(Transform t)
    {
        var partes = new List<string>();
        for (; t != null; t = t.parent)
        {
            var c = t.GetComponent<Canvas>();
            if (c != null && c.isRootCanvas) break;
            partes.Insert(0, t.name);
        }
        return string.Join("/", partes);
    }
}
