using System.Collections;
using UnityEngine;

namespace OfficeFood.Enemy
{
    public class EnemyDetectionTimer : MonoBehaviour
    {
        public float fadeTime;
        public Vector2 coverStart;
        public Vector2 coverEnd;
        public Transform cover;
        public Color detectColor;
        public Color chaseColor;
        public SpriteRenderer colorRenderer;

        private Enemy _enemy;
        private Coroutine _fadeCoroutine;
        private bool _faded;

        private void Start()
        {
            _enemy = GetComponentInParent<Enemy>();

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
                colorRenderer.color = new Color(chaseColor.r, chaseColor.g, chaseColor.b, colorRenderer.color.a);
            }
            else
            {
                colorRenderer.color = new Color(detectColor.r, detectColor.g, detectColor.b, colorRenderer.color.a);
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
            cover.localPosition = Vector2.Lerp(coverStart, coverEnd, time);
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

                foreach (SpriteRenderer renderer in renderers)
                {
                    renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
                }

                yield return null;
            }

            _fadeCoroutine = null;
        }
    } 
}