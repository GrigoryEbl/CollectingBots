using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Mover))]
public class Bot : MonoBehaviour
{
    private Transform _target;
    private Base _base;
    private Transform _transform;
    private Mover _mover;
    private bool _isFree;

    public bool IsFree => _isFree;
    public Transform Target => _target;

    private void Awake()
    {
        _transform = transform;
        _mover = GetComponent<Mover>();
        _isFree = true;
    }

    private void Start()
    {
        _base = GetComponentInParent<Base>();
    }

    private void Update()
    {
        DeliveredResource();
    }

    private void OnEnable()
    {
        _mover.RichTarget += OnCatchResource;
    }

    private void OnDisable()
    {
        _mover.RichTarget -= OnCatchResource;
    }

    private void DeliveredResource()
    {
        Resource resource = GetComponentInChildren<Resource>();

        if (resource != null && _transform.position == _base.transform.position)
        {
            _base.TakeResource();
            Destroy(resource.gameObject);
            _isFree = true;
        }
    }

    private void OnCatchResource()
    {
        _target.parent = transform;
        _target = _base.transform;
    }

    public void GetTargetPosition(Transform target)
    {
        _target = target;
        _isFree = false;
    }
}
