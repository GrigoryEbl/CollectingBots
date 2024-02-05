using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Transform _transform;
    private Transform _target;
    private float _minCatchDistance = 2;

    public event UnityAction TargetReached;

    private void Start()
    {
        _transform = transform;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        while (true)
        {
            _transform.position = Vector3.MoveTowards(_transform.position, _target.position, _speed * Time.deltaTime);
            _transform.LookAt(_target);

            if (Vector3.Distance(_transform.position, _target.position) <= _minCatchDistance)
            {
                TargetReached?.Invoke();
                break;
            }

            yield return null;
        }
    }
}
