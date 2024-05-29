using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OfficeFood.Enemy
{
    [RequireComponent(typeof(Slider))]
    public class EnemyDetectionTimer : MonoBehaviour
    {
        [SerializeField]
        private Enemy _enemy = null;
        [SerializeField]
        private Image _fill = null;
        [SerializeField]
        private Image _background = null;

        public float fadeTime;
        public Color detectColor;
        public Color chaseColor;

        private Coroutine _fadeCoroutine;
        private bool _faded;

        private Slider _slider = null;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void Start()
        {
            _enemy.OnDetectionChange += OnDetectionChange;
        }

        private void OnDestroy()
        {
            _enemy.OnDetectionChange -= OnDetectionChange;
        }

        private void Update()
        {
            if (_enemy.State == EnemyState.Following || 
                (_enemy.State == EnemyState.Paused && _enemy.LastState == EnemyState.Following))
            {
                _fill.color = new Color(chaseColor.r, chaseColor.g, chaseColor.b, _fill.color.a);
            }
            else
            {
                _fill.color = new Color(detectColor.r, detectColor.g, detectColor.b, _fill.color.a);
            }
        }

        private void OnDetectionChange(Enemy enemy, float time)
        {
            if (_fadeCoroutine == null)
            {
                if (time == 0 && !_faded)
                {
                    // Hide sprite when zero
                    _fadeCoroutine = StartCoroutine(Fade());
                    return;
                }
                else if (time != 0 && _faded)
                {
                    // Show sprite when not 0
                    _fadeCoroutine = StartCoroutine(Unfade());
                }

            }
            _slider.value = time;
        }

        private IEnumerator Fade()
        {
            yield return FadeSprite(1, 0);
            _faded = true;
        }

        private IEnumerator Unfade()
        {
            yield return FadeSprite(0, 1);
            _faded = false;
        }

        private IEnumerator FadeSprite(float a, float b)
        {
            float start = Time.time;
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            while (Time.time - start < fadeTime)
            {
                float alpha = Mathf.Lerp(a, b, (Time.time - start) / fadeTime);
                _fill.color = new Color(_fill.color.r, _fill.color.g, _fill.color.b, alpha);
                _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, alpha);
                yield return null;
            }
            _fadeCoroutine = null;
        }
    }
}
