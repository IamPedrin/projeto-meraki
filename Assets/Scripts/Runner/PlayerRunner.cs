using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRunner : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private Rigidbody2D _rb;
    private GameInput _input;
    private bool _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = new GameInput();
    }

    private void Update()
    {
        _rb.linearVelocity = new Vector2(forwardSpeed, _rb.linearVelocity.y);
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
