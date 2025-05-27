using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GO_BombBehavior_Inventories : MonoBehaviour
{
    public float damage = 50f;
    public float explosionRadius = 5f;
    public float fuseTime = 3f;

    private void Start()
    {
        Invoke(nameof(Explode), fuseTime); // explode after X seconds
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode(); // explode at contact
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            var health = hit.GetComponentInParent<GO_HealthComponent_FirstPerson>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // VFX here <--
        Destroy(gameObject);
    }
}
