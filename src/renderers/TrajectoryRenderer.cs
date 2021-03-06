﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibraMage
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

        public enum DistanceMeasure
        {
            Actual, Horizontal, Vertical
        }

        private enum State
        {
            Playing, Paused, Stopped
        }

        private State state;

        public FadeType FadeInType { get; set; }

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

        public FadeType FadeOutType { get; set; }

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

        public FadeType LineOfSightFadeType { get; set; }

        private float lineOfSight;
        public float LineOfSight
        {
            get
            {
                return lineOfSight;
            }

            set
            {
                lineOfSight = value >= 0 ? value : 0;
            }
        }

        public DistanceMeasure LineOfSightConstraint { get; set; }

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

                foreach (SpriteRenderer spriteRenderer in spriteRenderers.Values)
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

                if (IsVisible)
                {
                    foreach (SpriteRenderer spriteRenderer in spriteRenderers.Values)
                    {
                        var color = spriteRenderer.color;
                        color.a = opacity;
                        spriteRenderer.color = color;
                    }
                }
            }
        }

        public Vector2 Gravity { get; set; }

        public Vector2 InitialVelocity { get; set; }

        public bool IsVisible { get; private set; }

        private List<GameObject> activePellets;
        private List<GameObject> pooledPellets;
        private Dictionary<GameObject, SpriteRenderer> spriteRenderers;
        private Dictionary<SpriteRenderer, float> savedPelletOpacities;

        new Transform transform;

        private void Awake()
        {
            activePellets = new List<GameObject>();
            pooledPellets = new List<GameObject>();
            spriteRenderers = new Dictionary<GameObject, SpriteRenderer>();
            savedPelletOpacities = new Dictionary<SpriteRenderer, float>();

            transform = GetComponent<Transform>();

            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());
            PoolPellet(CreatePellet());

            FadeInType = DEFAULT_FADE_IN_TYPE;
            FadeOutType = DEFAULT_FADE_OUT_TYPE;

            fadeInTime = DEFAULT_FADE_IN_TIME;
            fadeOutTime = DEFAULT_FADE_OUT_TIME;

            IsVisible = false;

            opacity = 1f;

            lineOfSight = 10f;
            LineOfSightConstraint = DistanceMeasure.Actual;
            LineOfSightFadeType = FadeType.None;

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
            if (!IsVisible)
            {
                IsVisible = true;
                StopAllCoroutines();
                StartCoroutine(Fade_Coroutine(true));
            }
        }

        public void Hide()
        {
            if (IsVisible)
            {
                IsVisible = false;
                StopAllCoroutines();
                StartCoroutine(Fade_Coroutine(false));
            }
        }

        private IEnumerator Fade_Coroutine(bool isFadingIn)
        {
            var fadeType = isFadingIn ? FadeInType : FadeOutType;
            var fadeTime = isFadingIn ? fadeInTime : fadeOutTime;

            if (fadeType == FadeType.None || fadeTime == 0f)
            {
                foreach (SpriteRenderer spriteRenderer in spriteRenderers.Values)
                {
                    var savedOpacity = savedPelletOpacities[spriteRenderer];
                    SetPelletOpacity(spriteRenderer.gameObject, isFadingIn ? savedOpacity : 0f, false);
                }

                yield return null;
            }

            else
            {
                float timeElapsed = 0f;

                while (timeElapsed <= fadeTime)
                {
                    foreach (SpriteRenderer spriteRenderer in spriteRenderers.Values)
                    {
                        var savedOpacity = savedPelletOpacities[spriteRenderer];
                        SetPelletOpacity(spriteRenderer.gameObject, savedOpacity * (isFadingIn ? timeElapsed / fadeTime : 1 - (timeElapsed / fadeTime)), false);
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

            spriteRenderers.Add(pellet, spriteRenderer);
            savedPelletOpacities.Add(spriteRenderer, 0f);

            SetPelletOpacity(pellet, IsVisible ? opacity : 0f);

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

        private void PoolPellet(GameObject pellet)
        {
            pellet.SetActive(false);

            pooledPellets.Add(pellet);
        }

        public void OnInitialVelocityUpdate(Vector2 initialVelocity)
        {
            InitialVelocity = initialVelocity;

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
                AdjustPelletOpacity(activePellets[i], distanceCovered);

                timeCovered += timeStep;

                if (i != 0)
                {
                    switch (LineOfSightConstraint)
                    {
                        case DistanceMeasure.Actual:
                            distanceCovered += Mathga.GetVectorMagnitude2D(activePellets[i].transform.position - activePellets[i - 1].transform.position);
                            break;
                        case DistanceMeasure.Horizontal:
                            distanceCovered += activePellets[i].transform.position.x - activePellets[i - 1].transform.position.x;
                            break;
                        case DistanceMeasure.Vertical:
                            distanceCovered += activePellets[i].transform.position.y - activePellets[i - 1].transform.position.y;
                            break;
                    }
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

        private void PositionPellet(GameObject pellet, float time)
        {
            float x = Gravity.x * time * time * 0.5f + InitialVelocity.x * time + transform.position.x;
            float y = Gravity.y * time * time * 0.5f + InitialVelocity.y * time + transform.position.y;
            
            Vector3 position = new Vector3(x, y, pellet.transform.position.z);

            pellet.transform.position = position;
        }

        private void AdjustPelletOpacity(GameObject pellet, float distanceCovered)
        {
            SpriteRenderer spriteRenderer = spriteRenderers[pellet];

            switch (LineOfSightFadeType)
            {
                case FadeType.None:
                    SetPelletOpacity(pellet, opacity);
                    break;
                case FadeType.Linear:
                    var effectiveOpacity = 1 - (distanceCovered / lineOfSight);
                    effectiveOpacity = Mathf.Clamp(effectiveOpacity, 0f, 1f);
                    SetPelletOpacity(pellet, effectiveOpacity);
                    break;
                default:
                    SetPelletOpacity(pellet, opacity);
                    break;
            }
        }

        private void SetPelletOpacity(GameObject pellet, float opacity, bool saveOpacity = true)
        {
            SpriteRenderer spriteRenderer = spriteRenderers[pellet];

            var color = spriteRenderer.color;
            color.a = opacity;
            spriteRenderer.color = color;

            if (saveOpacity)
            {
                savedPelletOpacities[spriteRenderer] = opacity;
            }
        }
    }
}