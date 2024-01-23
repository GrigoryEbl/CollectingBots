using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private LayerMask _botLayer;
    [SerializeField] private LayerMask _resourceLayer;
    [SerializeField] private Bot _prefab;
    [SerializeField] private float _baseRadius;
    [SerializeField] private float _scanRadius;

    private Queue<Bot> _bots = new();
    private Queue<Resource> _resources = new();
    private Transform _transform;
    private BaseCreator _baseCtreator;
    private int _countResources;
    private int _countResourcesToBuildBase = 5;
    private int _minCountResourcesToCreate = 3;
    private int _maxUnits = 5;
    private int _currentCountBots = 0;
    private float _scanDelay = 2f;

    private bool _canBuildBase;

    public int CountResources => _countResources;

    private void Awake()
    {
        _countResources = 0;
        _transform = transform;
        _baseCtreator = FindObjectOfType<BaseCreator>();
        _canBuildBase = false;
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
        if (_currentCountBots < _maxUnits && _canBuildBase == false)
            CreateNewUnit();

        GetBots();
        SetTask();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _baseRadius);
        Gizmos.DrawWireSphere(transform.position, _scanRadius);
    }

    private void OnMouseDown()
    {
        if (_canBuildBase == false)
        {
            print("Select base");
            _baseCtreator.SelectBase();
        }
    }

    private void OnEnable()
    {
        _baseCtreator.FlagCreated += OnBuildNewBase;
    }

    private void OnDisable()
    {
        _baseCtreator.FlagCreated -= OnBuildNewBase;
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

    public void TakeResource()
    {
        _countResources++;
    }

    private void SetTask()
    {
        for (int i = 0; i < _bots.Count; i++)
        {
            if (_bots.Peek().IsFree)
            {
                if (_resources.Peek() != null && _resources.Peek().IsOrdered == false)
                {
                    _bots.Peek().GetTargetPosition(_resources.Peek().transform);
                    _resources.Dequeue().IsOrdered = true;
                }

                if (_canBuildBase && _countResources >= _countResourcesToBuildBase)
                {
                    SendBotToBuildBase(_bots.Peek());
                    return;
                }
            }

            _bots.Dequeue();
        }
    }

    private void CreateNewUnit()
    {
        if (_countResources >= _minCountResourcesToCreate)
        {
            _countResources -= _minCountResourcesToCreate;
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
                if (Vector3.Distance(bot.transform.position, _transform.position) <= _baseRadius)
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
                if (resources[i].TryGetComponent(out Resource resource))
                {
                    _resources.Enqueue(resource);
                }
            }
        }
    }

    private void SetParentBot()
    {
        Collider[] bots = Physics.OverlapSphere(_transform.position, _baseRadius, _botLayer);

        foreach (Collider item in bots)
        {
            if (item.TryGetComponent(out Bot bot))
            {
                bot.transform.parent = transform;
                return;
            }
        }
    }

    private void OnBuildNewBase()
    {
        _canBuildBase = true;
    }

    private void SendBotToBuildBase(Bot bot)
    {
        _canBuildBase = false;
        _countResources -= _countResourcesToBuildBase;
        bot.GetTargetPosition(_baseCtreator.Flag.transform);

        for (int i = 0; i < bot.transform.childCount; i++)
        {
            if (bot.transform.GetChild(i).TryGetComponent(out Resource resource))
            {
                Destroy(resource);
            }
        }
    }
}