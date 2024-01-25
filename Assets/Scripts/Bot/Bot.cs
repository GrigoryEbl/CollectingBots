using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Mover))]
public class Bot : MonoBehaviour
{
    private Transform _target;
    private Transform _transform;
    private Mover _mover;
    private bool _isFree;
    private Base _base;
    private Resource _resource;

    public bool IsFree => _isFree;

    public event Action FlagReached;

    private void Awake()
    {
        _transform = transform;
        _mover = GetComponent<Mover>();
        _base = GetComponentInParent<Base>();
        _isFree = true;
    }

    private void OnEnable()
    {
        _mover.TargetReached += OnReachedTarget;
    }

    private void OnDisable()
    {
        _mover.TargetReached -= OnReachedTarget;
    }

    public void SetTargetPosition(Transform target)
    {
        _target = target;
        _isFree = false;

        _mover.SetTarget(_target);
    }

    private void DeliveredResource()
    {
        if (_resource.GetComponentInChildren<Resource>())
        {
            _base.TakeResource();
            Destroy(_resource.gameObject);
            _resource = null;
            _isFree = true;
        }
        else
        {
            Waiting();
        }
    }

    private void OnReachedTarget()
    {
        if (_target.TryGetComponent(out Resource resource))
        {
            _resource = resource;
            _target.parent = transform;
            _target = _base.transform;
            _mover.SetTarget(_target);
        }
        else if (_target == _base.transform)
        {
            DeliveredResource();
        }
        else if (_target.TryGetComponent(out Flag flag))
        {
            _isFree = true;
            print("Reached Flag");
            FlagReached?.Invoke();
        }
    }

    private void Waiting()
    {
        _target = null;
    }
}
