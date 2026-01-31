using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float selfDestroyHight = -40f;
    public Material material;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Canvas"))
        {
            GameManager.Instance.PlayGameSound(GameManager.Instance.HitClip);

            if (Physics.Raycast(transform.position, (collision.contacts[0].point - transform.position).normalized, out RaycastHit hitInfo, 1f, LayerMask.GetMask("Canvas")))
            {
                PaintRTTest.Instance.DrawAtUV(hitInfo.textureCoord, gameObject.tag.CompareTo("BulletL") == 0 ? 0 : 1);
                CameraShaker.Instance.ShakeCamera().Forget();
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            GameManager.Instance.PlayGameSound(GameManager.Instance.HitClip);

            switch (collision.gameObject.tag)
            {
                case "Bomb":
                    Debug.Log("Bullet hit Bomb");
                    if (Physics.Raycast(transform.position, (collision.contacts[0].point - transform.position).normalized, out RaycastHit hitInfo, 4f, LayerMask.GetMask("Canvas")))
                    {
                        PaintRTTest.Instance.DrawAtUV(hitInfo.textureCoord, 0.4f, gameObject.tag.CompareTo("BulletL") == 0 ? 0 : 1);
                        CameraShaker.Instance.ShakeCamera().Forget();
                    }
                    break;
                default:
                    Debug.Log("Bullet hit unknown Item");
                    break;
            }

            Destroy(collision.gameObject);
        }

        Destroy(gameObject);

    }

    private void Update()
    {
        if (transform.position.y < selfDestroyHight)
        {
            Destroy(gameObject);
        }

    }
}
