using System;
using UnityEngine;

public class BaseCreator : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Base _basePrefab;
    [SerializeField] private float _minDistanceToOtherBase;
    [SerializeField] private LayerMask _baseLayer;

    private Camera _camera;
    private bool _isSelectedBase;
    private bool _isFlagCreated;

    public bool IsFlagCreated => _isFlagCreated;
    public Flag Flag { get; private set; }

    private void Awake()
    {
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

    public void OnBuildBase()
    {
         Instantiate(_basePrefab, Flag.transform.position, Quaternion.identity);
        Flag.DestroyObject();
        Flag = null;
        _isFlagCreated = false;
        _isSelectedBase = false;
        print("Created new base");
    }

    private void ScanPresenceOtherBase()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Collider[] colliders = Physics.OverlapSphere(hitInfo.point, _minDistanceToOtherBase, _baseLayer);

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
        Flag = Instantiate(_flagPrefab, hitInfo.point, Quaternion.identity);

        _isFlagCreated = true;


        print("Created flag");
    }

    private void TransposeFlag()
    {
        if (Flag != null)
        {
            Flag.DestroyObject();
            print("Destroyed Flag");
            Flag = null;
            _isFlagCreated = false;
        }

        ScanPresenceOtherBase();
    }
}

