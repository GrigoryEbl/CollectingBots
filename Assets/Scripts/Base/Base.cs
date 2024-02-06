using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scaner))]
public class Base : MonoBehaviour
{
    [SerializeField] private Bot _botPrefab;
    [SerializeField] private float _baseRadius;
    [SerializeField] private LayerMask _botLayer;
    [SerializeField] private Bot _firstBot;

    private Queue<Bot> _bots = new();
    private Queue<Resource> _resources = new();

    private BaseCreator _baseCreator;
    private Transform _transform;
    private Bot _botColonizer;
    private Scaner _scaner;

    private int _countResourcesToBuildBase = 5;
    private int _countResourcesToCreateBot = 3;
    private int _maxUnits = 5;
    private int _currentCountBots = 0;
    private int _countResources;

    private bool _isBaseSelect;

    public int CountResources => _countResources;

    public event Action ResourcesChange;

    private void Awake()
    {
        _baseCreator = FindObjectOfType<BaseCreator>();
        _transform = transform;

        if (_firstBot == null)
        {
            _botColonizer = _baseCreator.GetFirstBot();
            _botColonizer.transform.parent = transform;
        }
        else
            _botColonizer = _firstBot;

        _botColonizer.Base = this;
    }

    private void OnEnable()
    {
        _botColonizer.FlagReached += _baseCreator.OnBuildBase;
        _botColonizer.FlagReached += OnSetNewBotColonizer;
    }

    private void OnDisable()
    {
        _botColonizer.FlagReached -= _baseCreator.OnBuildBase;
        _botColonizer.FlagReached -= OnSetNewBotColonizer;
    }

    private void Start()
    {
        _scaner = GetComponent<Scaner>();
        _isBaseSelect = false;
        _countResources = 0;
        ResourcesChange?.Invoke();
        FindBotsInBase();
        _currentCountBots = _bots.Count;
    }

    private void Update()
    {
        SetTask();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _baseRadius);
    }

    private void OnMouseDown()
    {
        _baseCreator.SelectBase();
        _isBaseSelect = true;
        _botColonizer.FlagReached += _baseCreator.OnBuildBase;
        print("Selected base");
    }

    public void TakeResource()
    {
        _countResources++;
        CreateNewUnit();
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
            if (_baseCreator.IsFlagCreated && _isBaseSelect && _countResources >= _countResourcesToBuildBase 
                && _bots.Peek() == _botColonizer.IsFree)
            {
                SendBotToBuildBase(_botColonizer);
                _bots.Dequeue();
            }
            else if (_bots.Peek().IsFree)
            {
                _bots.Dequeue().SetTargetPosition(_resources.Dequeue().transform);
            }

        }
    }

    private void CreateNewUnit()
    {
        if (_currentCountBots < _maxUnits && _baseCreator.IsFlagCreated == false && _countResources >= _countResourcesToCreateBot)
        {
            _countResources -= _countResourcesToCreateBot;
            ResourcesChange?.Invoke();

            Instantiate(_botPrefab, transform.position, Quaternion.identity, transform);

            FindBotsInBase();
            _currentCountBots++;
        }
    }

    private void FindBotsInBase()
    {
        _bots.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Bot bot))
            {
                if (Vector3.Distance(bot.transform.position, _transform.position) <= _baseRadius)
                    _bots.Enqueue(bot);
            }
        }
    }

    private void SetAvailableResource()
    {
        Collider[] colliders = _scaner.GetScanigItems();

        if (colliders == null)
            return;

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

    private void SendBotToBuildBase(Bot bot)
    {
        bot.SetTargetPosition(_baseCreator.Flag.transform);
        _currentCountBots--;
        _isBaseSelect = false;
        _countResources -= _countResourcesToBuildBase;
        ResourcesChange?.Invoke();
        CreateNewUnit();
    }

    private void OnSetNewBotColonizer(Bot bot)
    {
        FindBotsInBase();
        _botColonizer = _bots.Peek();
        _botColonizer.FlagReached += _baseCreator.OnBuildBase;
    }
}