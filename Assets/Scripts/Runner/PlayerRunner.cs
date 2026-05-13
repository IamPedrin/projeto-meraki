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
        if (_isGrounded)
        {
            IniciarPulo();
            for (int i = 0; i < filaSeguidores.Count; i++)
            {
                float tempoDeAtraso = (i + 1) * atrasoPorSeguidor;
                filaSeguidores[i].PularComAtraso(tempoDeAtraso);
            }
        }
    }

    public void AdicionarSeguidor(Follower prefabFollower, TipoAlimento tipo)
    {
        alimentosColetados.Add(tipo);

        float posicaoX = transform.position.x - ((filaSeguidores.Count + 1) * distanciaEntreSeguidores);
        Vector3 posicaoSpawn = new Vector3(posicaoX, transform.position.y, transform.position.z);

        Follower novoSeguidor = Instantiate(prefabFollower, posicaoSpawn, Quaternion.identity);
        
        novoSeguidor.tipo = tipo;
        novoSeguidor.player = this;

        filaSeguidores.Add(novoSeguidor);
    }

    public void GameOver()
    {
        GameManager.Instance.MostrarGameOver(alimentosColetados.Count);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }


}
