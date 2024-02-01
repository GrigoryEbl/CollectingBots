using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;
    [SerializeField] private float _delay;
    [SerializeField] private float _spawnRadius;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;

        StartCoroutine(Spawn());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            InstantiateResource();
            yield return new WaitForSeconds(_delay);
        }
    }

    private void InstantiateResource()
    {
        float height = 1f;
        Vector2 RandomPosition = Random.insideUnitCircle * _spawnRadius;

        Instantiate(_resourcePrefab, new Vector3(_transform.position.x + RandomPosition.x, height, _transform.position.z + RandomPosition.y), Quaternion.identity, _transform);
    }
}