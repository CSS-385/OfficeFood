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

        private Enemy _enemy;
        private Coroutine _coroutine;
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

        private void OnDetectionChange(Enemy enemy, float time)
        {
            if (_coroutine == null)
            {
                if (time == 0 && !_faded)
                {
                    // Hide sprite when zero
                    _coroutine = StartCoroutine(Fade());
                    return;
                }
                else if (time != 0 && _faded)
                {
                    // Show sprite when not 0
                    _coroutine = StartCoroutine(Unfade());
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

            _coroutine = null;
        }
    } 
}