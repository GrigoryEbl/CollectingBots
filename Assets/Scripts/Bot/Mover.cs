using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Transform _transform;
    private float _minCatchDistance = 2;
    private Transform _target;

    public event UnityAction TargetReached;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (_target != null)
            StartCoroutine(MoveToTarget());
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        
    }

    private IEnumerator MoveToTarget()
    {
        _transform.position = Vector3.MoveTowards(_transform.position, _target.position, _speed * Time.deltaTime);
        _transform.LookAt(_target);

        if (Vector3.Distance(_transform.position, _target.position) <= _minCatchDistance)
        {
            TargetReached?.Invoke();
            yield return null;
        }
    }

    
}
