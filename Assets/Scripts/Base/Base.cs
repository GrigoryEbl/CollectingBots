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
    public Queue<Resource> _resources = new();

    private Transform _transform;
    private BaseCreator _baseCreator;
    private Bot _botColonizer;
    private Scaner _scaner;

    private int _countResourcesToBuildBase = 5;
    private int _countResourcesToCreateBot = 3;
    private int _maxUnits = 5;
    private int _currentCountBots = 0;
    private int _countResources;

    private float _delaySpawnBots = 1f;
    private bool _canBuildBase;
    private bool _isBaseSelect;

    public int CountResources => _countResources;

    public event Action ResourcesChange;

    private void Awake()
    {
        _baseCreator = GetComponent<BaseCreator>();
        _scaner = GetComponent<Scaner>();
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
        StartCoroutine(CreateNewUnit());
    }

    private void Update()
    {
        _canBuildBase = _baseCreator.IsFlagCreated;
        print(_resources.Count);
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

                if (_resources.TryPeek(out Resource resource) == true)
                    _bots.Peek().SetTargetPosition(_resources.Dequeue().transform);


                if (_canBuildBase && _countResources >= _countResourcesToBuildBase)
                {
                    SendBotToBuildBase(_botColonizer);
                }

                _bots.Dequeue();
            }
        }
    }

    private IEnumerator CreateNewUnit()
    {
        while (_currentCountBots < _maxUnits)
        {
            if (_countResources >= _countResourcesToCreateBot && _canBuildBase == false)
            {
                _countResources -= _countResourcesToCreateBot;
                ResourcesChange?.Invoke();

                Instantiate(_prefab, transform.position, Quaternion.identity, transform);

                FindBotsInBase();
                _currentCountBots = _bots.Count;

            }

            yield return new WaitForSeconds(_delaySpawnBots);
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
        Collider[] colliders = _scaner.GetScanigItems();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null && colliders[i].TryGetComponent(out Resource resource))
            {
                if (_resources.Contains(resource) == false)
                {
                    _resources.Enqueue(resource);
                    Destroy(colliders[i]);
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