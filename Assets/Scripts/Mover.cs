using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Bot))]
public class Mover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Bot _bot;
    private Transform _transform;
    private float _minCatchDistance = 2;

    public event UnityAction ReachTarget;

    private void Awake()
    {
        _bot = GetComponent<Bot>();
    }

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        Transform target = _bot.Target;

        if (target != null)
        {
            _transform.position = Vector3.MoveTowards(_transform.position, target.position, _speed * Time.deltaTime);
            _transform.LookAt(target);

            if (Vector3.Distance(_transform.position, target.position) <= _minCatchDistance)
            {
                ReachTarget?.Invoke();
            }
        }
    }
}
