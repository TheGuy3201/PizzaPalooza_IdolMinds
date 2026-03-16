using UnityEngine;
using UnityEngine.Rendering;

namespace PizzaPalooza.Phase1
{
    /// <summary>
    /// Runtime optimization to disable shadows on punctual (point/spot) lights.
    /// This prevents "Reduced additional punctual light shadows resolution" spam in console
    /// when the shadow atlas becomes overloaded.
    /// </summary>
    public class PP_ShadowOptimization : MonoBehaviour
    {
        private void Awake()
        {
            OptimizeLighting();
        }

        private void OptimizeLighting()
        {
            Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);

            foreach (Light light in allLights)
            {
                // Only keep shadows on directional lights (main sun/key light)
                if (light.type != LightType.Directional)
                {
                    if (light.shadows != LightShadows.None)
                    {
                        light.shadows = LightShadows.None;
                    }
                }
            }
        }
    }
}
