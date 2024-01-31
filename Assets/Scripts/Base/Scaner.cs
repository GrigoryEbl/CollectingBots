using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaner : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _scanRadius;

    private Transform _transform;
    private Collider[] _resourceColliders;

    private void Start()
    {
        _transform = transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }

    public Collider[] GetScanigItems()
    {
        _resourceColliders = Physics.OverlapSphere(_transform.position, _scanRadius, _layerMask);

        return _resourceColliders;
    }
}
