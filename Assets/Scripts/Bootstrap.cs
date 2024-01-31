using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    //[SerializeField] private Bot _bot;
    //[SerializeField] private Mover _mover;
    [SerializeField] private Base _base;
    [SerializeField] private BaseCreator _baseCreator;

    private void Awake()
    {
        _base.Initialize();
        
    }
}
