using UnityEngine;
using UnityEngine.Events;

namespace OfficeFood.Highlight
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Highlightable : MonoBehaviour
    {
        public UnityEvent HighlightStarted = new UnityEvent();
        public UnityEvent HighlightStopped = new UnityEvent();

        public float pulseRate = 3.0f;// Pulses per second.
        private float _pulseDirection = 1.0f;
        private float _pulseFactor = 0.0f;// ping pong 0.0-1.0
        public Color pulseColorA = Color.clear;
        public Color pulseColorB = Color.white;

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
                // for now, just use simple pulsing color. TODO: shader?
                Color color = _spriteRenderer.color;
                _pulseFactor = Mathf.Clamp01(_pulseFactor + (_pulseDirection * pulseRate * Time.deltaTime));
                _spriteRenderer.color = Color.Lerp(pulseColorA, pulseColorB, _pulseFactor);
                if (_pulseFactor == 0.0f)
                {
                    _pulseDirection = 1.0f;
                }
                if (_pulseFactor == 1.0f)
                {
                    _pulseDirection = -1.0f;
                }
            }
            else
            {
                _spriteRenderer.color = Color.clear;
            }
        }
    }
}
