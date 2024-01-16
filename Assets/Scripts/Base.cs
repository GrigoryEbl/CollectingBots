using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    private Bot[] _bots;
    private Resource[] _resources;
    private int _countResources;

    private void Awake()
    {
        _countResources = 0;
        _bots = GetComponentsInChildren<Bot>();
        _resources = FindObjectsOfType<Resource>();

        for (int i = 0; i < _bots.Length; i++)
        {
            _bots[i].SetResoucrePosition(_resources[i].transform);
        }
    }

    public void TakeResource()
    {
        _countResources++;
    }
}
