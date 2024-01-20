using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private LayerMask _botLayer;
    [SerializeField] private LayerMask _resourceLayer;
    [SerializeField] private Bot _prefab;
    [SerializeField] private float _baseRadius = 10f;
    [SerializeField] private float _scanRadius = 50f;

    private Transform _transform;
    private BaseCreator _baseCtreator;
    private int _countResources;
    private int _countResourcesToBuildBase = 5;
    private int _minCountResourcesToCreate = 3;
    private int _maxUnits = 10;

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
    }

    private void FixedUpdate()
    {
        if (_countResources >= _minCountResourcesToCreate)
            CreateNewUnit();

        Collider[] botColliders = Physics.OverlapSphere(_transform.position, _baseRadius, _botLayer);
        Collider[] resourceColliders = Physics.OverlapSphere(_transform.position, _scanRadius, _resourceLayer);

        if (botColliders != null && resourceColliders != null)
            SetTask(botColliders, resourceColliders);
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
        _baseCtreator.CreatedNewFlag += OnBuildNewBase;
    }

    private void OnDisable()
    {
        _baseCtreator.CreatedNewFlag -= OnBuildNewBase;
    }

    public void TakeResource()
    {
        _countResources++;
    }

    private void SetTask(Collider[] bots, Collider[] resources)
    {
        for (int i = 0; i < bots.Length; i++)
        {
            if (bots[i] != null && resources[i] != null )
            {
                if (bots[i].TryGetComponent(out Bot bot) && bot.IsFree)
                {
                    bot.GetTargetPosition(resources[i].transform);
                    Destroy(resources[i]);
                }
                
                if (_canBuildBase && _countResources >= _countResourcesToBuildBase )
                {
                    SendBotToBuildBase(bot);
                    Destroy(resources[i].gameObject);
                }
            }
        }
    }

    private void CreateNewUnit()
    {
        if (GetCountBots() < _maxUnits && _canBuildBase == false)
        {
            _countResources -= _minCountResourcesToCreate;
            Instantiate(_prefab, transform.position, Quaternion.identity, transform);
        }
    }

    private int GetCountBots()
    {
        int bots = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out Bot bot))
            {
                bots++;
            }
        }

        return bots;
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
        bot.GetTargetPosition(_baseCtreator.Flag);
        _canBuildBase = false;
    }
}