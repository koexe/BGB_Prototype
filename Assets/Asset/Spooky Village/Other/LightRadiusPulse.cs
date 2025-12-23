using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

namespace LightRadiusPulseNyknck
{
    public class LightRadiusPulse : MonoBehaviour
    {
        public Light2D light2D;
        public float minRadius = 1.42f;
        public float maxRadius = 1.52f;
        public float pulseSpeed = 1f;

        private bool increasing = true;

        void Start()
        {
            if (light2D == null)
            {
                light2D = GetComponent<Light2D>();
            }
        }

        void Update()
        {
            if (increasing)
            {
                light2D.pointLightOuterRadius += pulseSpeed * Time.deltaTime;
                if (light2D.pointLightOuterRadius >= maxRadius)
                {
                    increasing = false;
                }
            }
            else
            {
                light2D.pointLightOuterRadius -= pulseSpeed * Time.deltaTime;
                if (light2D.pointLightOuterRadius <= minRadius)
                {
                    increasing = true;
                }
            }
        }
    }
}

