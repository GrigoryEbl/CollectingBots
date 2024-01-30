using System;
using UnityEngine;

public class BaseCreator : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private float _minDistanceToOtherBase;
    [SerializeField] private LayerMask _layerMask;

    public Bot BotColonizer;
    private Base _base;
    private Camera _camera;
    private Flag _tempFlag;
    private bool _isSelectedBase;
    private bool _isFlagCreated;

    public bool IsFlagCreated => _isFlagCreated;
    public Flag Flag => _tempFlag;

    public event Action BaseBuilded;

    private void Awake()
    {
        _base = GetComponent<Base>();
        BotColonizer = _base.GetBotColonizer();
        _camera = FindObjectOfType<Camera>();
        _isSelectedBase = false;
        _isFlagCreated = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isSelectedBase && _isFlagCreated == false)
                ScanPresenceOtherBase();
        }
    }

    private void OnEnable()
    {
        BotColonizer.FlagReached += OnBuildBase;
    }

    private void OnDisable()
    {
        BotColonizer.FlagReached -= OnBuildBase;
    }

    public void SelectBase()
    {
        if (_isSelectedBase && _isFlagCreated)
        {
            print("Установите флаг в новое место");
            TransposeFlag();
        }
        else
        {
            _isSelectedBase = true;
        }
    }
        
    private void ScanPresenceOtherBase()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Collider[] colliders = Physics.OverlapSphere(hitInfo.point, _minDistanceToOtherBase, _layerMask);

            foreach (Collider item in colliders)
            {
                if (item.TryGetComponent(out Base otherBase))
                {
                    print("рядом есть другая база");
                    return;
                }
            }

            CreateFlag(hitInfo);
        }
    }

    private void CreateFlag(RaycastHit hitInfo)
    {
        var flag = Instantiate(_flagPrefab, hitInfo.point, Quaternion.identity);

        _tempFlag = flag;
        _isFlagCreated = true;
        _isSelectedBase = false;

        print("Created flag");
    }

    private void OnBuildBase()
    {
        var newBase = Instantiate(_basePrefab, _tempFlag.transform.position, Quaternion.identity);
        _tempFlag.DestroyObject();
        _tempFlag = null;
        _isFlagCreated = false;

        BotColonizer.SetTargetPosition(newBase.transform);
        BaseBuilded?.Invoke();

        print("Created new base");
    }

    private void TransposeFlag()
    {
        if (_tempFlag != null)
        {
            _tempFlag.DestroyObject();
            print("Destroyed Flag");
            _tempFlag = null;
            _isFlagCreated = false;
        }

        ScanPresenceOtherBase();
    }
}

