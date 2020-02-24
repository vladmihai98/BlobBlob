using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeeSharpHUDController : MonoBehaviour
{
    [SerializeField] private SeeSharpController controller;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;

    void Start()
    {
        healthText.text = $"{controller.MaxHealth} / {controller.MaxHealth}";
    }

    void Update()
    {
        UpdateHealth();
    }

    void UpdateHealth()
    {
        int currentHealth = controller.GetCurrentHealth();
        healthText.text = $"{currentHealth} / {controller.MaxHealth}";
        healthBar.fillAmount = (float)currentHealth / controller.MaxHealth;
    }
}
