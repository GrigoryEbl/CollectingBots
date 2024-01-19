using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private LayerMask _botLayer;
    [SerializeField] private LayerMask _resourceLayer;
    [SerializeField] private Bot _prefab;
    [SerializeField] private float _baseRadius = 10f;
    [SerializeField] private float _scanRadius = 50f;

    private Transform _transform;
    private BaseCreating _baseCreating;
    private int _countResources;
    private int _minCountResourcesForCreate = 3;

    public int CountResources => _countResources;

    private void Awake()
    {
        _countResources = 0;
        _transform = transform;
        _baseCreating = FindObjectOfType<BaseCreating>();
    }

    private void FixedUpdate()
    {
        if (_countResources >= _minCountResourcesForCreate)
            CreateNewUnit();

        Collider[] bots = Physics.OverlapSphere(_transform.position, _baseRadius, _botLayer);
        Collider[] resources = Physics.OverlapSphere(_transform.position, _scanRadius, _resourceLayer);

        if (bots != null)
            SetTask(bots, resources);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _baseRadius);
        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }

    private void OnMouseDown()
    {
        if(_countResources >= 5)
        {
            print("try create base");
        _baseCreating.Create();
        }
    }

    public void TakeResource()
    {
        _countResources++;
    }

    private void SetTask(Collider[] bots, Collider[] resources)
    {
        if (resources == null)
            return;

        for (int j = 0; j < resources.Length; j++)
        {
            if (resources[j].TryGetComponent(out Resource resource))
            {
                if (bots[j].TryGetComponent(out Bot bot) && bot.IsFree)
                {
                    bot.GetTargetPosition(resources[j].transform);
                    Destroy(resources[j]);
                }
                else
                    break;
            }
            else
                break;
        }
    }

    private void CreateNewUnit()
    {
        if (transform.childCount > 10)
            return;

        _countResources -= _minCountResourcesForCreate;
        Instantiate(_prefab, transform.position, Quaternion.identity, transform);
    }
}