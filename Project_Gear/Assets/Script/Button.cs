using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    // UnityEvent allows you to assign functions to be called in the Inspector
    [SerializeField]
    private UnityEvent onClick;

    // Optional: Scene name for scene transition
    [SerializeField]
    private string targetScene;

    // This method can be called when the button is clicked
    public void TriggerEvent()
    {
        // Invoke custom events
        if (onClick != null)
        {
            onClick.Invoke();
        }

        // Handle scene transition if a target scene is specified
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    // Example: Detect mouse click on the button
    private void OnMouseDown()
    {
        TriggerEvent();
    }
}
