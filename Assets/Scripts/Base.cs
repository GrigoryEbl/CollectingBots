using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Spawner _spawner;
    [SerializeField] private LayerMask _botLayer;
    [SerializeField] private LayerMask _resourceLayer;

    private Bot[] _freeBots;
    private Transform _transform;
    private Resource[] _resources;
    private int _countResources;

    private float _baseRadius = 10f;
    private float _scanRadius = 50f;

    public int CountResources => _countResources;
    public int FreeBots { get; set; }

    private void Awake()
    {
        _countResources = 0;
        _transform = transform;
    }

    private void FixedUpdate()
    {
        Collider[] bots = Physics.OverlapSphere(_transform.position, _baseRadius, _botLayer);
        Collider[] resources = Physics.OverlapSphere(_transform.position, _scanRadius, _resourceLayer);

        foreach (Collider item in bots)
        {
            if (item.TryGetComponent<Bot>(out Bot bot))
                print(item.name);
        }
        
        foreach (Collider item in resources)
        {
            if (item.TryGetComponent<Resource>(out Resource resource))
                print(resource.name);
        }

        FindFreeBots(bots);
        FindResources(resources);

        SetWork();
    }

    private void FindFreeBots(Collider[] colliders)
    {
        print("Bots " + colliders.Length);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<Bot>(out Bot bot))
            {
                _freeBots[i] = bot;
                FreeBots++;
            }
        }
    }

    private void FindResources(Collider[] resources)
    {
        print("resLength " + resources.Length);

        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i].TryGetComponent<Resource>(out Resource resource))
            {
                _resources[i] = resource;
            }
        }

    }

    public void TakeResource()
    {
        _countResources++;
    }

    private void SetWork()
    {
        if (_freeBots != null && _resources != null)
        {
            for (int i = 0; i < _freeBots.Length; i++)
            {
                _freeBots[i].SetResoucrePosition(_resources[i].transform);
            }
        }
    }

    //private void OnEnable()
    //{
    //    _spawner.Spawned += OnScan;
    //}

    //private void OnDisable()
    //{
    //    _spawner.Spawned -= OnScan;
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _baseRadius);
        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }

}
