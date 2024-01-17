using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private LayerMask _botLayer;
    [SerializeField] private LayerMask _resourceLayer;

    private Transform _transform;
    private int _countResources;

    private float _baseRadius = 10f;
    private float _scanRadius = 50f;

    public int CountResources => _countResources;

    private void Awake()
    {
        _countResources = 0;
        _transform = transform;
    }

    private void FixedUpdate()
    {
        Collider[] bots = Physics.OverlapSphere(_transform.position, _baseRadius, _botLayer);
        Collider[] resources = Physics.OverlapSphere(_transform.position, _scanRadius, _resourceLayer);

        if (bots != null && resources != null)
            SetTask(bots, resources);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _baseRadius);
        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }

    public void TakeResource()
    {
        _countResources++;
    }

    private void SetTask(Collider[] bots, Collider[] resources)
    {
        for (int i = 0; i < bots.Length; i++)
        {
            if (bots[i].TryGetComponent(out Bot bot) && bot.IsFree)
            {
                if (resources != null && resources[i].TryGetComponent(out Resource resource))
                {
                    bot.GetTargetPosition(resources[i].transform);
                    Destroy(resources[i]);
                    print(resources.Length);
                }
            }
        }
    }
}