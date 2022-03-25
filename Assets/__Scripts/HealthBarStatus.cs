using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarStatus : MonoBehaviour
{
    public Slider healthBar;
    public Gradient barGradient;
    public Image barFill;
    public void setMaxHealth (int health)
    {
        healthBar.maxValue = health;
        healthBar.value = health;

        barFill.color = barGradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        healthBar.value = health;
        barFill.color = barGradient.Evaluate(healthBar.normalizedValue);
    }
}
