using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntidadeRunner : MonoBehaviour
{
    public RunnerStats stats;
    protected Rigidbody2D rb;
    protected bool isJumpButtonHeld;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected void GravityAndPhysics()
    {
        rb.linearVelocity = new Vector2(stats.forwardSpeed, rb.linearVelocity.y);
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = stats.baseGravity * stats.fallGravityMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, stats.maxFallSpeed));
        }
        else if (rb.linearVelocity.y > 0 && !isJumpButtonHeld)
        {
            rb.gravityScale = stats.baseGravity * stats.shortJumpGravityMultiplier;
        }
        else
        {
            rb.gravityScale = stats.baseGravity;
        }
    }

    protected virtual void IniciarPulo()
    {

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * stats.jumpForce, ForceMode2D.Impulse);
    }

}
