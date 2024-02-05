using TMPro;
using UnityEngine;

public class ResourceShower : MonoBehaviour
{
    private Base _base;

    private TMP_Text _text;

    private void Awake()
    {
        _base = GetComponentInParent<Base>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable() => _base.ResourcesChange += OnResourcesChange;

    private void OnDisable() => _base.ResourcesChange -= OnResourcesChange;

    private void OnResourcesChange()
    {
        _text.text = ($"Resources: {_base.CountResources}");
    }
}
