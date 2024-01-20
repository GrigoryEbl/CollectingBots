using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class BaseCreator : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _minDistanceToOtherBase;
    [SerializeField] private Bot _bot;

    private Camera _camera;
    private bool _isSelectedBase;

    public Transform Flag => _flagPrefab.transform;

    public event UnityAction CreatedNewFlag;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _isSelectedBase = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isSelectedBase)
                ScanPresenceOtherBase();
        }
    }

    private void OnEnable()
    {
        _bot.ReachedFlag += OnBuildBase;
    }

    private void OnDisable()
    {
        _bot.ReachedFlag -= OnBuildBase;
    }

    public void SelectBase()
    {
        if (_isSelectedBase)
        {
            print("База уже выбрана, поставтье флаг");
            return;
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
                if (item.TryGetComponent<Base>(out Base basee))
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
        _flagPrefab = Instantiate(_flagPrefab, hitInfo.point, Quaternion.identity);
        CreatedNewFlag?.Invoke();
        _isSelectedBase = false;
        print("Created flag");
    }

    private void OnBuildBase()
    {
        _basePrefab = Instantiate(_basePrefab, _flagPrefab.transform.position, Quaternion.identity);
        Destroy(_flagPrefab.gameObject);
        print("Created new base");
    }
}

