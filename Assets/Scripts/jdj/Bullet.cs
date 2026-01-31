using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float selfDestroyHight = -40f;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Canvas"))
        {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if(transform.position.y < selfDestroyHight)
        {
            Destroy(gameObject);
        }
    }
}
