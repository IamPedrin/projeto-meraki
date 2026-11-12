using UnityEngine;

/// <summary>
/// Ajusta o RectTransform para ficar dentro da área segura do aparelho (notch, bordas arredondadas, barra de gestos).
/// Coloque num objeto filho do Canvas que agrupe os elementos de HUD.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SafeArea : MonoBehaviour
{
    RectTransform _rt;
    Rect _ultimaArea;
    Vector2Int _ultimaTela;

    void Awake() => _rt = GetComponent<RectTransform>();

    void OnEnable() => Aplicar();

    void Update()
    {
        if (Screen.safeArea != _ultimaArea || _ultimaTela.x != Screen.width || _ultimaTela.y != Screen.height)
            Aplicar();
    }

    void Aplicar()
    {
        if (_rt == null) _rt = GetComponent<RectTransform>();
        if (Screen.width <= 0 || Screen.height <= 0) return;

        Rect area = Screen.safeArea;
        _ultimaArea = area;
        _ultimaTela = new Vector2Int(Screen.width, Screen.height);

        Vector2 min = area.position;
        Vector2 max = area.position + area.size;
        min.x /= Screen.width; min.y /= Screen.height;
        max.x /= Screen.width; max.y /= Screen.height;

        _rt.anchorMin = min;
        _rt.anchorMax = max;
        _rt.offsetMin = Vector2.zero;
        _rt.offsetMax = Vector2.zero;
    }
}

