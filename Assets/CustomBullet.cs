using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    public Rigidbody Rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physics_mat;

    bool isAnimated = false;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        if ( collisions > maxCollisions ) Explode();
    }

    private void Explode()
    {
        if (explosion != null)
        {
            if (!isAnimated)
            {
                GameObject newExplosion = Instantiate(explosion, transform.position, Quaternion.identity);

                Destroy(newExplosion, explosion.GetComponent<ParticleSystem>().startLifetime);

                isAnimated = true;
            }
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        Invoke("Delay", 0.05f);
    }
    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.collider.CompareTag("Bullet")) return;

        collisions++;

        if ( collision.collider.CompareTag("Enemy") && explodeOnTouch) Explode();
    }

    private void Setup()
    {
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physics_mat;

        Rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }

}
