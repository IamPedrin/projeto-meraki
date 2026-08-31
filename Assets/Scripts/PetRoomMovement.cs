using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PetRoomMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 2f;
    public float tempoEsperaMin = 2f;
    public float tempoEsperaMax = 5f;

    [Header("Área de Caminhada (Limites)")]
    public Transform limiteInferiorEsquerdo;
    public Transform limiteSuperiorDireito;

    private Vector3 _destino;
    private bool _estaMovendo = false;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(RotinaWander());
    }

    private void Update()
    {
        if (_estaMovendo && PetManager.Instancia != null && PetManager.Instancia.energiaAtual <= 0)
        {
            _estaMovendo = false;
        }

        if (_estaMovendo)
        {
            MoverParaDestino();
        }
    }

    private IEnumerator RotinaWander()
    {
        while (true)
        {
            if (PetManager.Instancia != null && PetManager.Instancia.energiaAtual <= 0)
            {
                _estaMovendo = false;
                _animator.SetBool("isMoving", false);
                _animator.SetBool("isSleeping", true);


                yield return new WaitUntil(() => PetManager.Instancia.energiaAtual > 0);

                _animator.SetBool("isSleeping", false);
            }

            _estaMovendo = false;
            _animator.SetBool("isMoving", false);

            float tempoEspera = Random.Range(tempoEsperaMin, tempoEsperaMax);
            yield return new WaitForSeconds(tempoEspera);

            if (PetManager.Instancia != null && PetManager.Instancia.energiaAtual <= 0)
                continue;

            EscolherNovoDestino();

            if (_destino.x < transform.position.x)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;

            _estaMovendo = true;
            _animator.SetBool("isMoving", true);

            yield return new WaitUntil(() => !_estaMovendo);
        }
    }

    private void EscolherNovoDestino()
    {
        float randomX = Random.Range(limiteInferiorEsquerdo.position.x, limiteSuperiorDireito.position.x);
        float randomY = Random.Range(limiteInferiorEsquerdo.position.y, limiteSuperiorDireito.position.y);

        _destino = new Vector3(randomX, randomY, transform.position.z);
    }

    private void MoverParaDestino()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destino, velocidade * Time.deltaTime);

        if (Vector3.Distance(transform.position, _destino) < 0.1f)
        {
            _estaMovendo = false;
        }
    }

    public void PausarPasseio()
    {
        _estaMovendo = false;
        _animator.SetBool("isMoving", false);

        StopAllCoroutines();
    }

    public void RetomarPasseio()
    {
        StopAllCoroutines();
        StartCoroutine(RotinaWander());
    }
}