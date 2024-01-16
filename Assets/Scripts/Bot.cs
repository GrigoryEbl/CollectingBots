using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bot : MonoBehaviour
{
    private Transform _target;
    private Base _base;
    private Mover _mover;

    public Transform Target => _target;

    private void Awake()
    {
        _mover = GetComponent<Mover>();

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

        if (resource != null && transform.position == _base.transform.position)
        {
            _base.TakeResource();
            Destroy(resource.gameObject);
        }
    }

    private void OnCatchResource()
    {
        _target.parent = transform;
        _target = _base.transform;
    }

    public void SetResoucrePosition(Transform resource)
    {
        _target = resource;
    }
}
