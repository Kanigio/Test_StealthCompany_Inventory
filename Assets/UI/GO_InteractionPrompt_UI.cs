using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GO_InteractionPrompt_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject background;

    private GO_Interactor_FirstPerson interactor;

    private void Start()
    {
        StartCoroutine(WaitForInteractor());

        // Safe defaults
        if (promptText != null)
        {
            promptText.enableWordWrapping = false;
            promptText.overflowMode = TextOverflowModes.Overflow;
        }

        if (background != null)
            background.SetActive(false);
    }

    private IEnumerator WaitForInteractor()
    {
        while (Sgl_GameMode_FirstPerson.Instance == null ||
               Sgl_GameMode_FirstPerson.Instance.PlayerCharacter == null)
        {
            yield return null;
        }

        interactor = Sgl_GameMode_FirstPerson.Instance.PlayerCharacter.GetComponent<GO_Interactor_FirstPerson>();
    }

    private void Update()
    {
        if (interactor == null || promptText == null || background == null) return;

        var interactable = interactor.CurrentInteractable;

        if (interactable != null)
        {
            promptText.text = interactable.GetInteractionPrompt();
            background.SetActive(true);
        }
        else
        {
            promptText.text = "";
            background.SetActive(false);
        }
    }
}
