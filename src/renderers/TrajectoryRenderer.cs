using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibraMage.Renderers
{
    public class TrajectoryRenderer : MonoBehaviour
    {
        public enum FadeType
        {
            Linear, Sinusoidal, None
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
                fadeInTime = value;
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
                fadeOutTime = value;
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

            fadeInType = FadeType.Linear;
            fadeOutType = FadeType.Linear;

            fadeInTime = 0.25f;
            fadeOutTime = 0.25f;

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

        private GameObject CreatePellet()
        {
            GameObject pellet = new GameObject();
            pellet.transform.parent = transform;
            SpriteRenderer spriteRenderer = pellet.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
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