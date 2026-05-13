using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Follower : EntidadeRunner
{
    [Header("Configurações")]
    public TipoAlimento tipo;
    public PlayerRunner player;
    private int _currentJumpIndex = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(stats.forwardSpeed, rb.linearVelocity.y);
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

        Gravity();
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * stats.jumpForce, ForceMode2D.Impulse);
    }

    public void SincronicazarPlayer()
    {
        if (player == null) return;

        while (_currentJumpIndex < player.jumpPointsX.Count && player.jumpPointsX[_currentJumpIndex] <= transform.position.x)
        {
            _currentJumpIndex++;
        }
    }
}
