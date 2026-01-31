using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float selfDestroyHight = -40f;
    public Material material;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Canvas"))
        {
            if(Physics.Raycast(transform.position, (collision.contacts[0].point - transform.position).normalized, out RaycastHit hitInfo, 1f, LayerMask.GetMask("Canvas")))
            {
                PaintRTTest.Instance.DrawAtUV(hitInfo.textureCoord, gameObject.tag.CompareTo("BulletL") == 0 ? 0 : 1);
                CameraShaker.Instance.ShakeCamera().Forget();
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
