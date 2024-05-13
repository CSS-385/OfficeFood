using UnityEngine;
using UnityEngine.Events;

namespace OfficeFood.Highlight
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Highlightable : MonoBehaviour
    {
        public UnityEvent HighlightStarted = new UnityEvent();
        public UnityEvent HighlightStopped = new UnityEvent();

        public float pulseRate = 2.0f;// Pulses per second.
        private float _pulseDirection = 1.0f;

        private int _highlighterCount = 0;// Account for more than one highlight sources.

        private SpriteRenderer _spriteRenderer = null;

        public void IncrementHighlighterCount()
        {
            int highlighterCount = Mathf.Max(0, _highlighterCount + 1);
            if (_highlighterCount != highlighterCount)
            {
                _highlighterCount = highlighterCount;
                if (_highlighterCount == 1)
                {
                    Debug.Log("Started!");
                    HighlightStarted.Invoke();
                }
            }
        }

        public void DecrementHighlighterCount()
        {
            int highlighterCount = Mathf.Max(0, _highlighterCount - 1);
            if (_highlighterCount != highlighterCount)
            {
                _highlighterCount = highlighterCount;
                if (_highlighterCount == 0)
                {
                    Debug.Log("Stopped!");
                    HighlightStopped.Invoke();
                }
            }
        }

        public bool IsHighlighted()
        {
            return _highlighterCount > 0;
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            // lerp color of spriterenderer
            if (IsHighlighted())
            {
                // some pulsing color?
                Color color = _spriteRenderer.color;
                color.a = Mathf.Clamp01(color.a + _pulseDirection * pulseRate * Time.deltaTime);
                if (color.a == 1.0f)
                {
                    _pulseDirection = -1.0f;
                }
                else if (color.a == 0.0f)
                {
                    _pulseDirection = 1.0f;
                }
                _spriteRenderer.color = color;
            }
            else
            {
                Color color = _spriteRenderer.color;
                color.a = 0.0f;
                _spriteRenderer.color = color;
            }
        }
    }
}
