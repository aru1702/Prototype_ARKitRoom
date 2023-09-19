//using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    const string MAIN_MENU = "MainMenuScene";

    [SerializeField]
    GameObject m_BackButton;
    public GameObject backButton
    {
        get => m_BackButton;
        set => m_BackButton = value;
    }

    void Start()
    {
        if (Application.CanStreamedLevelBeLoaded(MAIN_MENU))
            m_BackButton.SetActive(true);
        else
            m_BackButton.SetActive(false);
    }

    void Update()
    {
        //if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        //BackButtonPressed();
    }

    public void BackButtonPressed()
    {
        if (Application.CanStreamedLevelBeLoaded(MAIN_MENU))
            SceneManager.LoadScene(MAIN_MENU, LoadSceneMode.Single);
    }
}
