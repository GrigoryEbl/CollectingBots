using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Resource _resource;
    [SerializeField] private float _delay;
    [SerializeField] private int _count;
    [SerializeField] private float _spawnRadius;

    private Transform _transform;
    private WaitForSeconds _sleepTime;


    public event UnityAction Spawned;

    private void Start()
    {
        _transform = transform;
        _sleepTime = new WaitForSeconds(_delay);

        StartCoroutine(Spawn(_sleepTime));
    }

    private void InstantiateResource()
    {
        Vector3 position = new Vector3(Random.Range(_transform.localPosition.x - _spawnRadius, _transform.localPosition.x + _spawnRadius),
                                        _transform.localPosition.y,
                                        Random.Range(_transform.localPosition.z - _spawnRadius, _transform.localPosition.z + _spawnRadius));

        Instantiate(_resource, position, Quaternion.identity, _transform);
    }

    private IEnumerator Spawn(WaitForSeconds timeBetweenSpawns)
    {
        for (int i = 0; i < _count; i++)
        {
            InstantiateResource();
            Spawned?.Invoke();
            yield return timeBetweenSpawns;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}