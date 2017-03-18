using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibraMage.Renderers
{
    public class TrajectoryRenderer : MonoBehaviour
    {
        private const FadeType DEFAULT_FADE_IN_TYPE = FadeType.Linear;
        private const float DEFAULT_FADE_IN_TIME = 0.25f;
        private const FadeType DEFAULT_FADE_OUT_TYPE = FadeType.Linear;
        private const float DEFAULT_FADE_OUT_TIME = 0.25f;

        public enum FadeType
        {
            None, Linear, Sinusoidal
        }

        private enum State
        {
            Playing, Paused, Stopped
        }

        private State state;

        private FadeType fadeInType;
        public FadeType FadeInType
        {
            get
            {
                return fadeInType;
            }

            set
            {
                fadeInType = value;
            }
        }

        private float fadeInTime;
        public float FadeInTime
        {
            get
            {
                return fadeInTime;
            }

            set
            {
                fadeInTime = value >= 0 ? value : 0;
            }
        }

        private FadeType fadeOutType;
        public FadeType FadeOutType
        {
            get
            {
                return fadeOutType;
            }

            set
            {
                fadeOutType = value;
            }
        }

        private float fadeOutTime;
        public float FadeOutTime
        {
            get
            {
                return fadeOutTime;
            }

            set
            {
                fadeOutTime = value >= 0 ? value : 0;
            }
        }

        private float lineOfSight;
        private float lineOfSightSquared;
        public float LineOfSight
        {
            get
            {
                return lineOfSight;
            }

            set
            {
                lineOfSight = value >= 0 ? value : 0;
                lineOfSightSquared = lineOfSight * lineOfSight;
            }
        }

        private float timeStep;
        public float TimeStep
        {
            get
            {
                return timeStep;
            }

            set
            {
                timeStep = value >= 0 ? value : 0;
            }
        }

        private Sprite sprite;
        public Sprite Sprite
        {
            get
            {
                return sprite;
            }

            set
            {
                sprite = value;

                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.sprite = sprite;
                }
            }
        }

        private float opacity;
        public float Opacity
        {
            get
            {
                return opacity;
            }

            set
            {
                opacity = Mathf.Clamp(value, 0f, 1f);

                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    var color = spriteRenderer.color;
                    color.a = opacity;
                    spriteRenderer.color = color;
                }
            }
        }

        private float gravity;
        public float Gravity
        {
            get
            {
                return gravity;
            }

            set
            {
                gravity = value;
            }
        }

        private Vector2 initialVelocity;
        public Vector2 InitialVelocity
        {
            get
            {
                return initialVelocity;
            }

            set
            {
                initialVelocity = value;
            }
        }

        private bool isVisible;

        private List<GameObject> activePellets;
        private List<GameObject> pooledPellets;
        private List<SpriteRenderer> spriteRenderers;

        private void Awake()
        {
            activePellets = new List<GameObject>();
            pooledPellets = new List<GameObject>();
            spriteRenderers = new List<SpriteRenderer>();

            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());

            fadeInType = DEFAULT_FADE_IN_TYPE;
            fadeOutType = DEFAULT_FADE_OUT_TYPE;

            fadeInTime = DEFAULT_FADE_IN_TIME;
            fadeOutTime = DEFAULT_FADE_OUT_TIME;

            isVisible = false;

            opacity = 1f;

            state = State.Stopped;
        }

        public void Play()
        {
            if (state == State.Playing)
            {
                return;
            }
        }

        public void Stop()
        {
            if (state == State.Stopped)
            {
                return;
            }
        }

        public void Show()
        {
            if (!isVisible)
            {
                StopAllCoroutines();
                StartCoroutine(Fade_Coroutine(true));
                isVisible = true;
            }
        }

        public void Hide()
        {
            if (isVisible)
            {
                StopAllCoroutines();
                StartCoroutine(Fade_Coroutine(false));
                isVisible = false;
            }
        }

        private IEnumerator Fade_Coroutine(bool isFadingIn)
        {
            var fadeType = isFadingIn ? fadeInType : fadeOutType;
            var fadeTime = isFadingIn ? fadeInTime : fadeOutTime;

            if (fadeType == FadeType.None || fadeTime == 0f)
            {
                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    var color = spriteRenderer.color;
                    color.a = isFadingIn ? opacity : 0f;
                    spriteRenderer.color = color;
                }

                yield return null;
            }

            else
            {
                float timeElapsed = 0f;

                while (timeElapsed <= fadeTime)
                {
                    foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                    {
                        var color = spriteRenderer.color;
                        color.a = isFadingIn ? timeElapsed / fadeTime : 1 - (timeElapsed / fadeTime);
                        color.a *= opacity;
                        spriteRenderer.color = color;
                    }

                    timeElapsed += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        private GameObject CreatePellet()
        {
            GameObject pellet = new GameObject();
            pellet.transform.parent = transform;
            SpriteRenderer spriteRenderer = pellet.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

            var color = spriteRenderer.color;
            color.a = opacity;
            spriteRenderer.color = color;

            spriteRenderers.Add(spriteRenderer);

            return pellet;
        }

        private GameObject GetPooledPelletIfAvailable()
        {
            GameObject pellet;

            if (pooledPellets.Count != 0)
            {
                pellet = pooledPellets[pooledPellets.Count - 1];
                pooledPellets.RemoveAt(pooledPellets.Count - 1);
            }

            else
            {
                pellet = CreatePellet();
            }

            pellet.SetActive(true);

            return pellet;
        }

        public void PoolPellet(GameObject pellet)
        {
            pellet.SetActive(false);

            pooledPellets.Add(pellet);
        }

        public void PositionPellet(GameObject pellet, float time)
        {
            float y = gravity * time * time * 0.5f + initialVelocity.y * time + transform.position.y;
            float x = initialVelocity.x * time + transform.position.x;

            Vector3 position = new Vector3(x, y, pellet.transform.position.z);

            pellet.transform.position = position;
        }

        public void OnForceUpdate(Vector2 initialVelocity)
        {
            this.initialVelocity = initialVelocity;

            float distanceCovered = 0f;
            float timeCovered = 0f;

            int i = 0;

            if (activePellets.Count == 0)
            {
                activePellets.Add(GetPooledPelletIfAvailable());
            }

            for (; i < activePellets.Count; i++)
            {
                PositionPellet(activePellets[i], timeCovered);

                timeCovered += timeStep;

                if (i != 0)
                {
                    distanceCovered += Vector3.Distance(activePellets[i].transform.position, activePellets[i - 1].transform.position);
                }

                if (distanceCovered >= lineOfSight)
                {
                    break;
                }

                if (i == activePellets.Count - 1)
                {
                    activePellets.Add(GetPooledPelletIfAvailable());
                }
            }

            for (; i < activePellets.Count; i++)
            {
                PoolPellet(activePellets[i]);
                activePellets.RemoveAt(i);
                i--;
            }
        }
    }
}