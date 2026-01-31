using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float selfDestroyHight = -40f;
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Canvas"))
        {
            if(Physics.Raycast(transform.position, (collision.contacts[0].point - transform.position).normalized, out RaycastHit hitInfo, 1f, LayerMask.GetMask("Canvas")))
            {
                PaintRTTest.Instance.DrawAtUV(hitInfo.textureCoord);
            }
            
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
