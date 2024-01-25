using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Transform _transform;
    private float _minCatchDistance = 2;

    public Transform Target { get; set; }

    public event UnityAction TargetReached;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        if (Target != null)
            StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        _transform.position = Vector3.MoveTowards(_transform.position, Target.position, _speed * Time.deltaTime);
        _transform.LookAt(Target);

        if (Vector3.Distance(_transform.position, Target.position) <= _minCatchDistance)
        {
            TargetReached?.Invoke();
            yield return null;
        }
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }
}
