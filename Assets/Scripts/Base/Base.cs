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
        _baseCreator = FindObjectOfType<BaseCreator>();
        _scaner = GetComponent<Scaner>();
        _transform = transform;
        SetParentBot();
        FindBotsInBase();
        _botColonizer = _bots.Peek();
        _bots.Peek().Base = this;
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
                    _bots.Peek().SetTargetPosition(_resources.Dequeue().transform);

                if (_canBuildBase && _countResources >= _countResourcesToBuildBase && _isBaseSelect)
                {
                    _botColonizer.FlagReached += _baseCreator.OnBuildBase;
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
            if (_canBuildBase == false && _countResources >= _countResourcesToCreateBot)
            {
                _countResources -= _countResourcesToCreateBot;
                ResourcesChange?.Invoke();

                Instantiate(_prefab, transform.position, Quaternion.identity, transform);

                FindBotsInBase();
                _currentCountBots++;
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

        if (colliders == null)
            return;

        colliders = GetSortedToDistanceColliders(colliders);

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
        _currentCountBots--;
        bot.SetTargetPosition(_baseCreator.Flag.transform);
        _countResources -= _countResourcesToBuildBase;
        ResourcesChange?.Invoke();
        _isBaseSelect = false;

        for (int i = 0; i < bot.transform.childCount; i++)
        {
            if (bot.transform.GetChild(i).TryGetComponent(out Resource resource))
            {
                Destroy(resource.gameObject);
                //resource = null;
            }
        }
    }

    private void OnSetNewBotColonizer()
    {
        FindBotsInBase();
        _botColonizer = _bots.Peek();
        _botColonizer.FlagReached += _baseCreator.OnBuildBase;
    }

    private Collider[] GetSortedToDistanceColliders(Collider[] colliders)
    {
        Collider temp = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            for (int j = 0; j < colliders.Length-1; j++)
            {
                if (Vector3.Distance(_transform.position, colliders[j].transform.position) > Vector3.Distance(_transform.position, colliders[j + 1].transform.position))
                {
                    temp = colliders[j + 1];
                    colliders[j + 1] = colliders[j];
                    colliders[j] = temp;
                }
            }
        }

        return colliders;
    }
}