using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DayNightCycle : MonoBehaviour
{
    public float cycleDurationInMinutes = 20f; // Total duration of the day-night cycle in minutes
    public Color dayColor = Color.white; // Color of the scene during daytime
    public Color nightColor = Color.black; // Color of the scene during nighttime

    private float currentTime = 0f; // Current time elapsed in the cycle
    private float currentCycleProgress = 0f; // Current progress of the cycle (0 to 1)

    // Update is called once per frame
    void Update()
    {
        // Update current time
        currentTime += Time.deltaTime;

        // Calculate current cycle progress
        currentCycleProgress = currentTime / (cycleDurationInMinutes * 60f);

        // Ensure current cycle progress stays within 0 to 1
        currentCycleProgress = Mathf.Clamp01(currentCycleProgress);

        // Interpolate global light level based on the current cycle progress
        // Use sine function to smoothly transition between day and night
        float t = Mathf.Sin(currentCycleProgress * Mathf.PI); // Value between -1 and 1
        float normalizedT = (t + 1f) / 2f; // Normalize t to range from 0 to 1
        float globalLightLevel = Mathf.Lerp(0.1f, 0.9f, normalizedT); // Interpolate between darkest and lightest values

        // Update global light level
        SetGlobalLightLevel(globalLightLevel);

        // Update scene color based on the current time of day
        Color sceneColor = Color.Lerp(nightColor, dayColor, normalizedT);
        RenderSettings.ambientLight = sceneColor;
    }

    // Function to set the global light level
    void SetGlobalLightLevel(float lightLevel)
    {
        Shader.SetGlobalFloat("GlobalLightLevel", lightLevel);
    }
}