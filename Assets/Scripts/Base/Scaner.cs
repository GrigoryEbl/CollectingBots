using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaner : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _scanRadius;

    private Transform _transform;
    private float _scanDelay = 2f;
    private Collider[] _resourceColliders;

    private void Start()
    {
        _transform = transform;
        StartCoroutine(Scan());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }

    public Collider[] GetColliders()
    {
        return _resourceColliders;
    }

    private IEnumerator Scan()
    {
        while (true)
        {
            _resourceColliders = Physics.OverlapSphere(_transform.position, _scanRadius, _layerMask);
            
            yield return new WaitForSeconds(_scanDelay);
        }
    }
}
