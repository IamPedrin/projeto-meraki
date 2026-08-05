using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Follower : EntidadeRunner
{
    [Header("Configurações")]
    public PlayerRunner player;

    protected override void Awake()
    {
        base.Awake();
    }

    private void FixedUpdate()
    {
        GravityAndPhysics();
    }

    public void PularComAtraso(float tempoEspera)
    {
        StartCoroutine(RotinaPulo(tempoEspera));
    }

    private IEnumerator RotinaPulo(float delay)
    {
        yield return new WaitForSeconds(delay);
        IniciarPulo();
    }
}
