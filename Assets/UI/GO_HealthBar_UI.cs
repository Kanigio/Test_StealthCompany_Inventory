using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GO_HealthBar_UI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    private GO_HealthComponent_FirstPerson healthComponent;

    private void Start()
    {
        StartCoroutine(WaitForHealthComponent());
    }

    private IEnumerator WaitForHealthComponent()
    {
        // Wait until GameMode and PlayerCharacter are available
        while (Sgl_GameMode_FirstPerson.Instance == null ||
               Sgl_GameMode_FirstPerson.Instance.PlayerCharacter == null)
        {
            yield return null;
        }

        var player = Sgl_GameMode_FirstPerson.Instance.PlayerCharacter;
        healthComponent = player.GetComponent<GO_HealthComponent_FirstPerson>();

        if (healthComponent == null)
        {
            Debug.LogError("GO_HealthComponent_FirstPerson not found on player.");
            yield break;
        }

        // Set max value once
        healthSlider.maxValue = healthComponent.maxHealth;

        // Initial update
        UpdateHealthBar();

        // Bind to health changed event
        healthComponent.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        // Unbind the event to avoid memory leaks
        if (healthComponent != null)
        {
            healthComponent.OnHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthComponent != null)
        {
            healthSlider.value = healthComponent.CurrentHealth;
        }
    }
}
