using Cysharp.Threading.Tasks;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float selfDestroyHight = -40f;
    public float brushSize = 0.2f;
    public Material material;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Canvas"))
        {
            GameManager.Instance.PlayGameSound(GameManager.Instance.HitClip);

            if (Physics.Raycast(transform.position, CanvasTiltFx.Instance.transform.forward, out RaycastHit hitInfo, 10f, LayerMask.GetMask("Canvas")))
            {
                PaintRTTest.Instance.DrawAtUV(hitInfo.textureCoord, brushSize, gameObject.tag.CompareTo("BulletL") == 0 ? 0 : 1);
                CameraShaker.Instance.ShakeCamera().Forget();
            }
        }

        Destroy(gameObject);

    }

    void OnTriggerEnter (Collider other)
    {
        if (brushSize >= PaintRTTest.Instance.brushSize && other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            GameManager.Instance.PlayGameSound(GameManager.Instance.BombClip);

            switch (other.gameObject.tag)
            {
                case "Bomb":
                    Debug.Log("Bullet hit Bomb");
                    if (Physics.Raycast(other.gameObject.transform.position, CanvasTiltFx.Instance.transform.forward, out RaycastHit hitInfo, 10f, LayerMask.GetMask("Canvas")))
                    {
                        PaintRTTest.Instance.DrawAtUV(hitInfo.textureCoord, 1.0f, gameObject.tag.CompareTo("BulletL") == 0 ? 0 : 1);
                        CameraShaker.Instance.ShakeCamera().Forget();
                    }
                    break;
                default:
                    Debug.Log("Bullet hit unknown Item");
                    break;
            }

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (transform.position.y < selfDestroyHight)
        {
            Destroy(gameObject);
        }

    }
}
