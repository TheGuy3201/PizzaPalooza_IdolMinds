using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PP_LightingTools
{
    [MenuItem("PizzaPalooza/Lighting/Fix Shadow Atlas Warnings (Disable Punctual Shadows)")]
    public static void DisablePunctualShadows()
    {
        Light[] lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        int changed = 0;

        for (int i = 0; i < lights.Length; i++)
        {
            Light light = lights[i];
            if (light == null)
            {
                continue;
            }

            if (light.type == LightType.Directional)
            {
                continue;
            }

            if (light.shadows == LightShadows.None)
            {
                continue;
            }

            Undo.RecordObject(light, "Disable punctual light shadows");
            light.shadows = LightShadows.None;
            EditorUtility.SetDirty(light);
            changed++;
        }

        if (changed > 0)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(activeScene);
        }

        Debug.Log($"[PP_LightingTools] Disabled shadows on {changed} point/spot lights.");
    }
}
