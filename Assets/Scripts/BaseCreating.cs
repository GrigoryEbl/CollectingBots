using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BaseCreating : MonoBehaviour
{
    [SerializeField] private GameObject _flag;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _minDistnceToOtherBase = 15f;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void Create()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Collider[] colliders = Physics.OverlapSphere(hitInfo.point, _minDistnceToOtherBase, _layerMask);

                foreach (Collider item in colliders)
                {
                    print(item.name);

                    if (item.TryGetComponent<Base>(out Base basee))
                    {
                        print("NOT");
                        return;
                    }
                    else
                    {
                        Instantiate(_flag, hitInfo.point, Quaternion.identity);
                        print("rich create flag");
                    }
                }
            }
        }
    }
}

