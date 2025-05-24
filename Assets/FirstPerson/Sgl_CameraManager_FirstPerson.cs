using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sgl_CameraManager_FirstPerson : MonoBehaviour
{
    public static Camera CurrentCamera { get; private set; }

    private void Awake()
    {
        if (CurrentCamera == null)
        {
            CurrentCamera = GetComponent<Camera>();
        }
        else
        {
            Destroy(gameObject); // Optional: avoid duplicate managers
        }
    }

    public static void SetActiveCamera(Camera cam)
    {
        CurrentCamera = cam;
    }
}
