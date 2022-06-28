using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthScript : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(int health)
    {
        slider.highValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
