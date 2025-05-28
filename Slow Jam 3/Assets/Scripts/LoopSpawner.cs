using UnityEngine;

public class LoopSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    [SerializeField] private float spawnRate = 5;

    private float _nextSpawnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _nextSpawnTime = Time.time + spawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _nextSpawnTime)
        {
            var spawn = Instantiate(_prefab);
            spawn.transform.position = transform.position;
            _nextSpawnTime = Time.time + spawnRate;
        }
    }
}