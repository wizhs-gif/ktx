using UnityEngine;

public class ItemSpawner3D : MonoBehaviour
{
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private Transform spawnPoint;

    public void Spawn()
    {
        if (pickupPrefab == null || spawnPoint == null) return;
        Instantiate(pickupPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}