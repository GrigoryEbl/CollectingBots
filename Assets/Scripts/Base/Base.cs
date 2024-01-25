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
    private BaseCreator _baseCtreator;

    private int _countResources;
    private int _countResourcesToBuildBase = 5;
    private int _countResourcesToCreateBot = 3;
    private int _maxUnits = 5;
    private int _currentCountBots = 0;

    private float _scanDelay = 2f;

    private bool _canBuildBase;
    private bool _isBotSendedToBuildNewBase;

    public int CountResources => _countResources;

    public event Action ResourcesChange;

    private void Awake()
    {
        _countResources = 0;
        ResourcesChange?.Invoke();
        _transform = transform;
        _baseCtreator = FindObjectOfType<BaseCreator>();
        _canBuildBase = false;
        _isBotSendedToBuildNewBase = false;
    }

    private void Start()
    {
        SetParentBot();
        GetBots();
        _currentCountBots = _bots.Count;
        StartCoroutine(Scan());
    }

    private void Update()
    {
        if (_isBotSendedToBuildNewBase == false)
            _canBuildBase = _baseCtreator.IsFlagCreatred;

        if (_currentCountBots < _maxUnits && _canBuildBase == false)
            CreateNewUnit();

        GetBots();
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
        print("Selected base");
        _baseCtreator.SelectBase();
    }

    public void TakeResource()
    {
        _countResources++;
        ResourcesChange?.Invoke();
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
        for (int i = 0; i < _bots.Count; i++)
        {
            if (_bots.Peek().IsFree)
            {
                if (_resources.TryPeek(out Resource resource))
                {
                    _bots.Peek().SetTargetPosition(resource.transform);
                    _resources.Dequeue();
                }

                if (_canBuildBase && _countResources >= _countResourcesToBuildBase)
                {
                    SendBotToBuildBase(_bots.Peek());
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
            GetBots();
            _currentCountBots = _bots.Count;
        }
    }

    private void GetBots()
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
        Collider[] bots = Physics.OverlapSphere(_transform.position, _radius, _botLayer);

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
        print("Send bot to build");
        _canBuildBase = false;
        _isBotSendedToBuildNewBase = true;
        _countResources -= _countResourcesToBuildBase;
        ResourcesChange?.Invoke();
        bot.SetTargetPosition(_baseCtreator.Flag.transform);
        _baseCtreator.Bot = bot;

        for (int i = 0; i < bot.transform.childCount; i++)
        {
            if (bot.transform.GetChild(i).TryGetComponent(out Resource resource))
            {
                Destroy(resource);
            }
        }
    }
}