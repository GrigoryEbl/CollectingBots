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

    private Bot[] _bots;
    private Resource[] _resources;
    private Transform _transform;
    private BaseCreator _baseCtreator;
    private int _countResources;
    private int _countResourcesToBuildBase = 5;
    private int _minCountResourcesToCreate = 3;
    private int _maxUnits = 5;
    private float _scanDelay = 3f;

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
        StartCoroutine(Scan());
    }

    private void FixedUpdate()
    {
        if (_bots.Length < _maxUnits && _canBuildBase == false)
            CreateNewUnit();

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
        Collider[] botColliders = Physics.OverlapSphere(_transform.position, _baseRadius, _botLayer);
        Collider[] resourceColliders = Physics.OverlapSphere(_transform.position, _scanRadius, _resourceLayer);

        if (botColliders != null && resourceColliders != null)
        {
            GetBots();
            GetAvailableResource(resourceColliders);
        }
            

        yield return new WaitForSeconds(_scanDelay);

    }

    public void TakeResource()
    {
        _countResources++;
    }

    private void SetTask()
    {
        for (int i = 0; i < _bots.Length; i++)
        {
            if (_bots[i].IsFree)
            {
                if (_resources[i].IsOrdered == false && _resources[i] != null)
                {
                    _resources[i].IsOrdered = true;
                    _bots[i].GetTargetPosition(_resources[i].transform);
                    Destroy(_resources[i]);
                }
            }

            if (_canBuildBase && _countResources >= _countResourcesToBuildBase)
            {
                Destroy(_resources[i].gameObject);
                SendBotToBuildBase(_bots[i]);
            }
        }
    }
    private void CreateNewUnit()
    {
        if (_countResources >= _minCountResourcesToCreate)
        {
            _countResources -= _minCountResourcesToCreate;
            Instantiate(_prefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void GetBots()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Bot bot))
            {
                if (Vector3.Distance(bot.transform.position, _transform.position) <= _baseRadius)
                    _bots[i] = bot;
            }
        }
    }

    private void GetAvailableResource(Collider[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i].TryGetComponent(out Resource resource))
            {
                _resources[i] = resource;
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
            }
        }
    }

    private void OnBuildNewBase()
    {
        _canBuildBase = true;
    }

    private void SendBotToBuildBase(Bot bot)
    {
        _countResources -= _countResourcesToBuildBase;
        bot.GetTargetPosition(_baseCtreator.Flag.transform);
        _canBuildBase = false;
    }
}