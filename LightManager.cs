using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightManager : MonoBehaviour
{
    [SerializeField] protected GameObject fireFlies;
    protected Light2D light2D;

    [SerializeField] protected Gradient gradient;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    private void Start() {
        fireFlies.SetActive(false);
    }
    private void Update()
    {
        UpdateLightColor();
        if( DateTime.Now.Hour >= 19 || DateTime.Now.Hour < 5)
        {
            fireFlies.SetActive(true); 
        }
        else
        {
            fireFlies.SetActive(false);
        }
    }

    private void UpdateLightColor()
    {
        // Lấy thời gian hiện tại của hệ thống
        DateTime now = DateTime.Now;

        // Tính toán thời gian trong ngày (0.0 đến 1.0)
        float timeOfDay = (float)now.TimeOfDay.TotalSeconds / 86400.0f; // 86400 giây trong một ngày

        // Lấy màu từ gradient dựa trên thời gian trong ngày
        Color color = gradient.Evaluate(timeOfDay);
        light2D.color = color;
    }

}