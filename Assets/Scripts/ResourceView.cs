using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceView : MonoBehaviour
{
    private Base _base;
    private TMP_Text _text;

    private void Start()
    {
        _base = FindObjectOfType<Base>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        _text.text = ($"Resources: {_base.CountResources} \n Free bots: {_base.FreeBots}");
    }
}
