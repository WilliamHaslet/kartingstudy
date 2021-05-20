using UnityEngine;

public class CartSpawner : MonoBehaviour
{

    [SerializeField] private GameObject aiCartPrefab;
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnDistance;

    private void Start()
    {
        
        for (int i = 0; i < spawnCount; i++)
        {

            Vector3 spawnPosition = new Vector3((i + 1) * spawnDistance, 0, 0);

            Instantiate(aiCartPrefab, spawnPosition, Quaternion.identity);

        }

    }

}
