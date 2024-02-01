using System;
using UnityEngine;

[RequireComponent(typeof(Mover))]
public class Bot : MonoBehaviour
{
    public Base Base;
    private Transform _target;
    private Mover _mover;
    private bool _isFree;
    private Resource _resource;

    public bool IsFree => _isFree;

    public event Action FlagReached;

    private void Awake()
    {
        _mover = GetComponent<Mover>();
        Base = GetComponentInParent<Base>();
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

    public void SetNewBase(Base newBase)
    {
        Base = newBase;
    }

    private void DeliveredResource()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Resource resource))
            {
                Base.TakeResource();
                Destroy(resource.gameObject);
                _resource = null;
                _isFree = true;
            }
        }
    }

    private void OnReachedTarget()
    {
        if (_target != null && _target.TryGetComponent(out Resource resource))
        {
            _resource = resource;
            _target.parent = transform;
            _target = Base.transform;
            _mover.SetTarget(_target);
        }
        else if (_target == Base.transform)
        {
            DeliveredResource();
        }
        else if (_target != null && _target.TryGetComponent(out Flag flag))
        {
            _isFree = true;
            FlagReached?.Invoke();

            print("Reached Flag");
        }
    }
}
