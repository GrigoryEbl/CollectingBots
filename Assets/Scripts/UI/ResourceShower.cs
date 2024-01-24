using TMPro;
using UnityEngine;

public class ResourceShower : MonoBehaviour
{
    private Base _base;
    private TMP_Text _text;

    private void Start()
    {
        _base = GetComponentInParent<Base>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void FixedUpdate()
    {
        _text.text = ($"Resources: {_base.CountResources}");
    }
}
