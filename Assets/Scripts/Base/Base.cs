using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Bot _prefab;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _botLayer;

    private Queue<Bot> _bots = new();
    private Queue<Resource> _resources = new();

    private Transform _transform;
    private BaseCreator _baseCreator;
    private Bot _botColonizer;
    private Scaner _scaner;

    private int _countResources;
    private int _countResourcesToBuildBase = 5;
    private int _countResourcesToCreateBot = 3;
    private int _maxUnits = 5;
    private int _currentCountBots = 0;

    private bool _canBuildBase;
    private bool _isBaseSelect;

    public int CountResources => _countResources;

    public event Action ResourcesChange;

    private void Awake()
    {
        _baseCreator = GetComponent<BaseCreator>();
        _transform = transform;
        SetParentBot();
        FindBotsInBase();
        _botColonizer = _bots.Peek();
    }

    private void OnEnable() => _botColonizer.FlagReached += _baseCreator.OnBuildBase;
    private void OnDisable() => _botColonizer.FlagReached -= _baseCreator.OnBuildBase;

    private void Start()
    {
        _canBuildBase = false;
        _isBaseSelect = false;
        _countResources = 0;
        ResourcesChange?.Invoke();
        _currentCountBots = _bots.Count;
    }

    private void Update()
    {
        _canBuildBase = _baseCreator.IsFlagCreated;

        if (_currentCountBots < _maxUnits && _canBuildBase == false)
            CreateNewUnit();

        SetTask();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnMouseDown()
    {
        _baseCreator.SelectBase();
        _isBaseSelect = true;
        print("Selected base");
    }

    public void TakeResource()
    {
        _countResources++;
        ResourcesChange?.Invoke();
    }

    private void SetTask()
    {
        if (_bots == null)
            return;

        if (_resources == null)
            return;

        SetAvailableResource();
        FindBotsInBase();

        for (int i = 0; i < _bots.Count; i++)
        {
            if (_bots.Peek().IsFree)
            {
                if (_resources.TryPeek(out Resource resource))
                {
                    _bots.Peek().SetTargetPosition(resource.transform);
                    _resources.Dequeue();
                }

                if (_canBuildBase && _isBaseSelect && _countResources >= _countResourcesToBuildBase)
                {
                    SendBotToBuildBase(_botColonizer);
                }

                _bots.Dequeue();
            }
        }
    }

    private void CreateNewUnit()
    {
        if (_countResources >= _countResourcesToCreateBot)
        {
            _countResources -= _countResourcesToCreateBot;
            ResourcesChange?.Invoke();
            Instantiate(_prefab, transform.position, Quaternion.identity, transform);
            FindBotsInBase();
            _currentCountBots = _bots.Count;
        }
    }

    private void FindBotsInBase()
    {
        _bots.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Bot bot))
            {
                if (Vector3.Distance(bot.transform.position, _transform.position) <= _radius)
                    _bots.Enqueue(bot);
            }
        }
    }

    private void SetAvailableResource()
    {
        Collider[] resources = _scaner.GetColliders();

        if (resources != null)
        {
            _resources.Clear();

            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i].TryGetComponent(out Resource resource) && resource.TryGetComponent(out Collider collider))
                {
                    _resources.Enqueue(resource);
                    Destroy(collider);
                }
            }
        }
    }

    private void SetParentBot()
    {
        Collider[] bots = Physics.OverlapSphere(transform.position, _radius, _botLayer);

        foreach (Collider item in bots)
        {
            if (item.TryGetComponent(out Bot bot))
            {
                bot.transform.SetParent(transform);
                return;
            }
        }
    }

    private void SendBotToBuildBase(Bot bot)
    {
        _canBuildBase = false;

        bot.SetTargetPosition(_baseCreator.Flag.transform);

        _countResources -= _countResourcesToBuildBase;
        ResourcesChange?.Invoke();

        _isBaseSelect = false;
        SetNewBotColonizer();
    }

    private void SetNewBotColonizer()
    {
        FindBotsInBase();
        _botColonizer = _bots.Peek();
    }
}