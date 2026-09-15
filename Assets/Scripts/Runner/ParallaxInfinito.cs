using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxInfinito : MonoBehaviour
{
    [Header("Configuração")]
    [Range(0f, 1f)]
    public float efeitoParallax;

    private Transform _cameraTransform;
    private float _posicaoInicialX;
    private float _tamanhoDaImagem;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;

        _posicaoInicialX = transform.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _tamanhoDaImagem = sr.bounds.size.x;

        GameObject clone = new GameObject("CloneParallax");
        SpriteRenderer cloneSr = clone.AddComponent<SpriteRenderer>();
        cloneSr.sprite = sr.sprite;
        cloneSr.color = sr.color;
        cloneSr.sortingLayerID = sr.sortingLayerID;
        cloneSr.sortingOrder = sr.sortingOrder;

        clone.transform.SetParent(transform);
        clone.transform.position = new Vector3(transform.position.x + _tamanhoDaImagem, transform.position.y, transform.position.z + 0.1f);
        clone.transform.localScale = Vector3.one;
    }

    private void LateUpdate()
    {
        float distanciaMovida = (_cameraTransform.position.x * efeitoParallax);
        float distanciaRestante = (_cameraTransform.position.x * (1 - efeitoParallax));

        transform.position = new Vector3(_posicaoInicialX + distanciaMovida, transform.position.y, transform.position.z);

        if (distanciaRestante > _posicaoInicialX + _tamanhoDaImagem)
        {
            _posicaoInicialX += _tamanhoDaImagem;
        }
        else if (distanciaRestante < _posicaoInicialX - _tamanhoDaImagem)
        {
            _posicaoInicialX -= _tamanhoDaImagem;
        }
    }
}