using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sgl_GameMode_FirstPerson : MonoBehaviour
{
    // Singleton instance to allow global access to the GameMode
    public static Sgl_GameMode_FirstPerson Instance { get; private set; }

    [Header("Player Setup")]

    // Prefab that contains both the PlayerController and PlayerCharacter components
    [SerializeField] private GameObject playerPrefab;

    // Reference to the instantiated PlayerController component
    public GO_PlayerController_FirstPerson PlayerController { get; private set; }

    // Reference to the instantiated PlayerCharacter component
    public GO_PlayerCharacter_FirstPerson PlayerCharacter { get; private set; }

    [Header("Gameplay Data")]

    // Reference to the global item database used in the game
    [SerializeField] private SO_ItemDatabase_Inventories itemDatabase;

    // Public accessor for the assigned item database
    public SO_ItemDatabase_Inventories ItemDatabase => itemDatabase;

    // Called as soon as the script instance is being loaded
    private void Awake()
    {
        // Enforce Singleton pattern: destroy duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: keep this object between scenes
        // DontDestroyOnLoad(gameObject);
    }

    // Called on the first frame when the script is active
    private void Start()
    {
        InitializePlayer();
    }

    // Instantiates the Player prefab and retrieves controller and character references
    private void InitializePlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in the GameMode!");
            return;
        }

        // Instantiate the full Player prefab (controller + character)
        GameObject playerGO = Instantiate(playerPrefab);

        // Get references to required components
        PlayerController = playerGO.GetComponent<GO_PlayerController_FirstPerson>();
        PlayerCharacter = playerGO.GetComponent<GO_PlayerCharacter_FirstPerson>();

        // Validate that both components exist on the prefab
        if (PlayerController == null || PlayerCharacter == null)
        {
            Debug.LogError("Player prefab is missing required components: GO_PlayerController_FirstPerson or GO_PlayerCharacter_FirstPerson!");
        }
    }
}
