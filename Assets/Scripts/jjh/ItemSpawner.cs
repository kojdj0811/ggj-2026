using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs;

    private Transform _spawnTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnTarget = transform.root;

        InvokeRepeating(nameof(Spawn), 0f, 3f);
    }

    public void StartSpawn()
    {
        InvokeRepeating(nameof(Spawn), 0f, 3f);
    }

    public void Spawn()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0 || _spawnTarget == null)
            return;

        Renderer quadRenderer = _spawnTarget.GetComponent<Renderer>();
        if (quadRenderer == null)
            return;

        Bounds bounds = quadRenderer.bounds;
        // bounds.center는 월드 좌표, extents는 절반 크기

        // z축은 타겟과 동일하게 고정, x/y축만 랜덤
        float randX = Random.Range(bounds.min.x, bounds.max.x);
        float randY = Random.Range(bounds.min.y, bounds.max.y);
        float z = bounds.center.z;
        Vector3 worldPos = new Vector3(randX, randY, z);

        // 평면에 수직인 방향(노멀)
        Vector3 normal = _spawnTarget.up;
        Quaternion rot = Quaternion.LookRotation(normal);

        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        GameObject spawned = Instantiate(prefab, worldPos, rot);
        // 타겟의 자식으로 옮기고, 로컬 z축 포지션을 0으로
        spawned.transform.SetParent(_spawnTarget, true);
        Vector3 localPos = spawned.transform.localPosition;
        localPos.z = -0.01f;
        spawned.transform.localPosition = localPos;
    }

    public void StopSpawn()
    {
        CancelInvoke(nameof(Spawn));
    }
}
