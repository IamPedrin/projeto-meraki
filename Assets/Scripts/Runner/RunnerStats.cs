using UnityEngine;

[CreateAssetMenu(fileName = "RunnerStats", menuName = "Scriptable Objects/RunnerStats")]
public class RunnerStats : ScriptableObject
{   
    [Header("Movimento")]
    public float forwardSpeed = 5f;
    public float jumpForce = 10f;
    [Header("Gravidade")]
    public float baseGravity = 1f;
    public float fallGravityMultiplier = 2f;
    public float shortJumpGravityMultiplier = 3.5f;
    public float maxFallSpeed = -20f;
}
