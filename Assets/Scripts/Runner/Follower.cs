using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Follower : MonoBehaviour
{
    [Header("Configurações")]
    public TipoAlimento tipo;
    public PlayerRunner player;
    public float forwardSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D _rb;
    private int _currentJumpIndex = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(forwardSpeed, _rb.linearVelocity.y);
    }

    private void Update()
    {
        if (player == null) return;

        if (_currentJumpIndex < player.jumpPointsX.Count)
        {
            float nextJumpX = player.jumpPointsX[_currentJumpIndex];

            if (transform.position.x >= nextJumpX)
            {
                Jump();
                _currentJumpIndex++;
            }
        }
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void SincronicazarPlayer()
    {
        if(player == null) return;

        while(_currentJumpIndex < player.jumpPointsX.Count && player.jumpPointsX[_currentJumpIndex] <= transform.position.x)
        {
            _currentJumpIndex++;
        }
    }
}
