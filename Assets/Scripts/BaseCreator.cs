using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class BaseCreator : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _minDistanceToOtherBase;

    private Camera _camera;
    private bool _isSelectedBase;
    private Flag _tempFlag;

    public Flag Flag => _tempFlag;

    public event UnityAction FlagCreated;

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
        Bot.FlagReached += OnBuildBase;
    }

    private void OnDisable()
    {
        Bot.FlagReached -= OnBuildBase;
    }

    public void SelectBase()
    {
        if (_isSelectedBase)
        {
            print("Установите флаг в новое место");
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
       var flag = Instantiate(_flagPrefab, hitInfo.point, Quaternion.identity);
        FlagCreated?.Invoke();
        _isSelectedBase = false;
        _tempFlag = flag;
        print("Created flag");
    }

    private void OnBuildBase()
    {
        var basee = Instantiate(_basePrefab, _tempFlag.transform.position, Quaternion.identity);
        Destroy(_tempFlag);

        print("Created new base");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, _tempFlag.transform.position);
        
    }
}

