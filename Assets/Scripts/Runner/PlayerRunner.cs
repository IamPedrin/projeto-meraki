using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRunner : EntidadeRunner
{
    [Header("GroundCheck")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Sistema de Seguidores")]
    public float distanciaEntreSeguidores = 1.2f;
    public float atrasoPorSeguidor = 0.15f;
    public int limiteParaCesta = 5;
    public CestaFollower prefabCestaFollower;
    public Follower prefabFollowerPadrao;

    private CestaFollower _instaciaCestaFollower;
    private int _alimentosAtivos = 0;

    public List<TipoAlimento> alimentosColetados = new List<TipoAlimento>();
    public List<Follower> filaSeguidores = new List<Follower>();

    private GameInput _input;
    private bool _isGrounded;

    protected override void Awake()
    {
        base.Awake();
        _input = new GameInput();
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        GravityAndPhysics();
    }

    private void OnEnable()
    {
        _input.Gameplay.Enable();
        _input.Gameplay.Tap.performed += OnTap;
    }

    private void OnDisable()
    {
        _input.Gameplay.Tap.performed -= OnTap;
        _input.Gameplay.Disable();
    }

    private void OnTap(InputAction.CallbackContext ctx)
    {
        if (_isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            IniciarPulo();

            int offsetCesta = 0;
            if (_instaciaCestaFollower != null)
            {
                _instaciaCestaFollower.PularComAtraso(atrasoPorSeguidor);
                offsetCesta = 1;
            }


            for (int i = 0; i < filaSeguidores.Count; i++)
            {
                float tempoDeAtraso = (i + 1 + offsetCesta) * atrasoPorSeguidor;
                filaSeguidores[i].PularComAtraso(tempoDeAtraso);
            }
        }
    }

    public void AdicionarSeguidor(Follower prefabNovo, TipoAlimento tipo)
    {
        GameManager.Instance.AdicionarPontoHUD();
        alimentosColetados.Add(tipo);

        _alimentosAtivos++;

        AtualizarFormacaoDaTela(prefabNovo);
    }

    public void PerderSeguidores(int dano)
    {
        if (_alimentosAtivos <= dano)
        {
            GameOver();
            return;
        }

        _alimentosAtivos -= dano;

        AtualizarFormacaoDaTela(null);
    }

    private void AtualizarFormacaoDaTela(Follower prefabNovo)
    {

        int itensNaCesta = (_alimentosAtivos / limiteParaCesta) * limiteParaCesta;
        int itensNaFila = _alimentosAtivos % limiteParaCesta;

        // 1. GERENCIA A CESTA (Posição 1)
        if (itensNaCesta >= limiteParaCesta)
        {
            if (_instaciaCestaFollower == null)
            {
                // Nasce LOGO ATRÁS do jogador
                float posX = transform.position.x - distanciaEntreSeguidores;
                Vector3 posSpawn = new Vector3(posX, transform.position.y, transform.position.z);

                _instaciaCestaFollower = Instantiate(prefabCestaFollower, posSpawn, Quaternion.identity);
                _instaciaCestaFollower.player = this;
            }
            _instaciaCestaFollower.AtualizarNumero(itensNaCesta);
        }
        else if (_instaciaCestaFollower != null)
        {
            // O dano foi tanto que perdeu os 5 da cesta. Destrói ela!
            Destroy(_instaciaCestaFollower.gameObject);
            _instaciaCestaFollower = null;
        }

        while (filaSeguidores.Count > itensNaFila)
        {
            Follower ultimo = filaSeguidores[filaSeguidores.Count - 1];
            filaSeguidores.RemoveAt(filaSeguidores.Count - 1);
            if (ultimo != null) Destroy(ultimo.gameObject);
        }


        while (filaSeguidores.Count < itensNaFila)
        {
            int offsetCesta = (_instaciaCestaFollower != null) ? 1 : 0;
            int indiceVisual = filaSeguidores.Count + 1 + offsetCesta;

            // 1. O SEGREDO: Descobre quem está logo na frente para copiar a Altura e Velocidade!
            float alturaY = transform.position.y; // Padrão: copia o líder
            Vector2 velocidadeReferencia = rb.linearVelocity;

            if (filaSeguidores.Count > 0)
            {
                // Copia do último boneco da fila
                Follower daFrente = filaSeguidores[filaSeguidores.Count - 1];
                alturaY = daFrente.transform.position.y;
                velocidadeReferencia = daFrente.GetComponent<Rigidbody2D>().linearVelocity;
            }
            else if (_instaciaCestaFollower != null)
            {
                // Se não tem ninguém na fila, copia direto da Cesta
                alturaY = _instaciaCestaFollower.transform.position.y;
                velocidadeReferencia = _instaciaCestaFollower.GetComponent<Rigidbody2D>().linearVelocity;
            }

            // 2. Calcula a posição com a nova altura
            float posX = transform.position.x - (indiceVisual * distanciaEntreSeguidores);
            Vector3 posSpawn = new Vector3(posX, alturaY + 0.5f, transform.position.z);

            Follower prefabUsado = (prefabNovo != null) ? prefabNovo : prefabFollowerPadrao;
            Follower novoBoneco = Instantiate(prefabUsado, posSpawn, Quaternion.identity);
            novoBoneco.player = this;

            novoBoneco.GetComponent<Rigidbody2D>().linearVelocity = velocidadeReferencia;

            filaSeguidores.Add(novoBoneco);

            prefabNovo = null;
        }
    }

    public void RemoverSeguidorQueCaiu(Follower seguidorQueCaiu)
    {
        if (_instaciaCestaFollower != null && seguidorQueCaiu.gameObject == _instaciaCestaFollower.gameObject)
        {

            int itensNaCesta = (_alimentosAtivos / limiteParaCesta) * limiteParaCesta;
            _alimentosAtivos -= itensNaCesta;

            _instaciaCestaFollower = null;
        }

        else if (filaSeguidores.Contains(seguidorQueCaiu))
        {
            _alimentosAtivos--; // Perde só 1
            filaSeguidores.Remove(seguidorQueCaiu);
        }


        if (_alimentosAtivos < 0) _alimentosAtivos = 0;

        AtualizarFormacaoDaTela(null);
    }

    public void GameOver()
    {
        GameManager.Instance.MostrarGameOver();
    }
    public int ObterQuantidadeAtivos()
    {
        return _alimentosAtivos;
    }

    public void EntregarAlimentos(int quantidade)
    {
        _alimentosAtivos -= quantidade;

        if (_alimentosAtivos < 0)
        {
            _alimentosAtivos = 0;
        }

        AtualizarFormacaoDaTela(null);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }


}
