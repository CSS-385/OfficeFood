using UnityEngine;
using UnityEngine.UI;

public class MenuIcon : MonoBehaviour
{
    [SerializeField]
    private float _hoverDistance = 0.25f;
    [SerializeField]
    private float _hoverDuration = 1.0f;
    private float _hoverSpeed = 0.0f;
    [SerializeField]
    private float _hoverSpeedMax = 0.125f;
    [SerializeField]
    private float _hoverThreshold = 0.01f;
    private bool _hoverDirection = false;

    private Material _material = null;

    private int _materialTextureID = 0;

    private void Awake()
    {
        _material = GetComponent<Image>().material;
        _materialTextureID = Shader.PropertyToID("_MainTex");
    }

    private void Update()
    {
        Vector2 offset = _material.GetTextureOffset(_materialTextureID);
        if (_hoverDirection)
        {
            offset.y = Mathf.SmoothDamp(offset.y, _hoverDistance, ref _hoverSpeed, _hoverDuration, _hoverSpeedMax, Time.deltaTime);
            if (Mathf.Abs(offset.y - _hoverDistance) < _hoverThreshold)
            {
                _hoverDirection = false;
            }
        }
        else
        {
            offset.y = Mathf.SmoothDamp(offset.y, -_hoverDistance, ref _hoverSpeed, _hoverDuration, _hoverSpeedMax, Time.deltaTime);
            if (Mathf.Abs(offset.y - -_hoverDistance) < _hoverThreshold)
            {
                _hoverDirection = true;
            }
        }
        _material.SetTextureOffset(_materialTextureID, offset);
    }
}
