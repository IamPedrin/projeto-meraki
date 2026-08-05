using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRunner : EntidadeRunner
{
    [Header("GroundCheck")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Sistema de Seguidores (Visual)")]
    public float distanciaEntreSeguidores = 1.2f;
    public float atrasoPorSeguidor = 0.15f;
    public int limiteParaCesta = 5;
    public CestaFollower prefabCestaFollower;
    public Follower prefabFollowerPadrao;

    private CestaFollower _instaciaCestaFollower;
    private int _alimentosVisuaisAtivos = 0;
    public List<Follower> filaSeguidores = new List<Follower>();

    private bool _corridaFinalizada = false;
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

        if (_corridaFinalizada)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
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

    private void OnTriggerEnter2D(Collider2D colisao)
    {
        if (colisao.CompareTag("AlimentoBom"))
        {
            GameManager.Instance.AdicionarPontoHUD();

            _alimentosVisuaisAtivos++;
            AtualizarFormacaoDaTela();

            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("coletar");
            Destroy(colisao.gameObject);
        }
        else if (colisao.CompareTag("Ultraprocessado"))
        {
            GameManager.Instance.SofrerPunicao();

            if (_alimentosVisuaisAtivos > 0)
            {
                _alimentosVisuaisAtivos--;
                AtualizarFormacaoDaTela();
            }

            Destroy(colisao.gameObject);
        }
        else if (colisao.CompareTag("LinhaChegada"))
        {
            _corridaFinalizada = true; // Aciona o nosso freio de mão!
            GameManager.Instance.IniciarSequenciaDeVitoria();
        }
    }

    private void AtualizarFormacaoDaTela()
    {
        int itensNaCesta = (_alimentosVisuaisAtivos / limiteParaCesta) * limiteParaCesta;
        int itensNaFila = _alimentosVisuaisAtivos % limiteParaCesta;

        if (itensNaCesta >= limiteParaCesta)
        {
            if (_instaciaCestaFollower == null)
            {
                Vector3 posSpawn = new Vector3(transform.position.x - distanciaEntreSeguidores, transform.position.y, transform.position.z);
                _instaciaCestaFollower = Instantiate(prefabCestaFollower, posSpawn, Quaternion.identity);
                _instaciaCestaFollower.player = this;
            }
            _instaciaCestaFollower.AtualizarNumero(itensNaCesta);
        }
        else if (_instaciaCestaFollower != null)
        {
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

            Vector3 posSpawn = new Vector3(transform.position.x - (indiceVisual * distanciaEntreSeguidores), transform.position.y + 0.5f, transform.position.z);
            Follower novoBoneco = Instantiate(prefabFollowerPadrao, posSpawn, Quaternion.identity);
            novoBoneco.player = this;
            filaSeguidores.Add(novoBoneco);
        }
    }

    public void RemoverSeguidorQueCaiu(Follower seguidorQueCaiu)
    {
        if (_instaciaCestaFollower != null && seguidorQueCaiu.gameObject == _instaciaCestaFollower.gameObject)
        {
            int itensNaCesta = (_alimentosVisuaisAtivos / limiteParaCesta) * limiteParaCesta;
            _alimentosVisuaisAtivos -= itensNaCesta;
            _instaciaCestaFollower = null;
        }
        else if (filaSeguidores.Contains(seguidorQueCaiu))
        {
            _alimentosVisuaisAtivos--;
            filaSeguidores.Remove(seguidorQueCaiu);
        }

        if (_alimentosVisuaisAtivos < 0) _alimentosVisuaisAtivos = 0;

        AtualizarFormacaoDaTela();
    }

    public void GameOver()
    {
        GameManager.Instance.IniciarSequenciaDeVitoria();
    }

    public int ObterQuantidadeAtivos()
    {
        return _alimentosVisuaisAtivos;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}