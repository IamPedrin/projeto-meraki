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

    protected void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = stats.baseGravity * stats.fallGravityMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, stats.maxFallSpeed));
        }
        else
        {
            rb.gravityScale = stats.baseGravity;
        }
    }

}
