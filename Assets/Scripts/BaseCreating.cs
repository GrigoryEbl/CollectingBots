using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BaseCreating : MonoBehaviour
{
    [SerializeField] private GameObject _flag;

    private Camera _camera;
    private Base _base;
    private float _minDistnceToOtherBase = 30f;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _base = FindObjectOfType<Base>();
    }

    private void Update()
    {
        Create();
    }

    private void Create()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (Input.GetMouseButtonDown(0) && hitInfo.collider.TryGetComponent(out Base station) ==false)
                Instantiate(_flag, hitInfo.point, Quaternion.identity);//new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z)
        }
    }
}

