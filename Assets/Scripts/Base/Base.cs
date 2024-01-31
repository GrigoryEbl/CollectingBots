using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private Bot _prefab;
    [SerializeField] private float _radius;
    [SerializeField] private float _scanRadius;
    [SerializeField] private LayerMask _botLayer;
    [SerializeField] private LayerMask _resourceLayer;

    private Queue<Bot> _bots = new();
    private Queue<Resource> _resources = new();

    private Transform _transform;

     private BaseCreator _baseCreator;
    private int _countResources;
    private int _countResourcesToBuildBase = 5;
    private int _countResourcesToCreateBot = 3;
    private int _maxUnits = 5;
    private int _currentCountBots = 0;

    private float _scanDelay = 2f;

    private bool _canBuildBase;
    private bool _isBaseSelect;

    public int CountResources => _countResources;

    public event Action ResourcesChange;

    public void Initialize()
    {
        _transform = transform;
        SetParentBot();
        FindBotsInBase();
        _baseCreator.GetComponentInChildren<BaseCreator>();
        _baseCreator.Initialize();
        _baseCreator.BotColonizer = _bots.Peek();

        _canBuildBase = false;
        _isBaseSelect = false;
        _countResources = 0;
        ResourcesChange?.Invoke();
        _currentCountBots = _bots.Count;
        StartCoroutine(Scan());
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
        Gizmos.DrawWireSphere(transform.position, _scanRadius);
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

    private void SetNewBotColonizer()
    {
        FindBotsInBase();
        _baseCreator.BotColonizer = _bots.Peek();
    }

    private IEnumerator Scan()
    {
        while (true)
        {
            Collider[] resourceColliders = Physics.OverlapSphere(_transform.position, _scanRadius, _resourceLayer);
            GetAvailableResource(resourceColliders);

            yield return new WaitForSeconds(_scanDelay);
        }
    }

    private void SetTask()
    {
        if (_bots == null)
            return;

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

                if (_canBuildBase && _isBaseSelect && _baseCreator.BotColonizer == _bots.Peek() && _countResources >= _countResourcesToBuildBase)
                {
                    SendBotToBuildBase(_baseCreator.BotColonizer);
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

    private void GetAvailableResource(Collider[] resources)
    {
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
                bot.transform.parent = transform;
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
}