using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button treeButton;
    [SerializeField] private Button stumpButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Button closeOptionsButton;

    private void Start()
    {
        Debug.Log("MainMenu.Start called");

        treeButton.onClick.AddListener(LoadTreeCalculator);
        stumpButton.onClick.AddListener(LoadStumpCalculator);
        optionsButton.onClick.AddListener(OpenOptions);
        exitButton.onClick.AddListener(ExitApp);
        closeOptionsButton.onClick.AddListener(CloseOptions);

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager not found in scene. Please add an AudioManager GameObject with AudioManager.cs to MainMenu scene.");
        }
        else
        {
            Debug.Log($"AudioManager found on GameObject: {AudioManager.Instance.gameObject.name}");
        }

        // Verify soundToggle assignment
        if (soundToggle == null)
        {
            Debug.LogError("SoundToggle is not assigned in MainMenu.cs");
        }
        else
        {
            bool isMuted = PlayerPrefs.GetInt("SoundMuted", 0) == 1;
            soundToggle.isOn = !isMuted;
            soundToggle.onValueChanged.AddListener(OnSoundToggle);
            Debug.Log($"SoundToggle initialized: isOn = {soundToggle.isOn}, SoundMuted = {PlayerPrefs.GetInt("SoundMuted", 0)}");

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetSoundMuted(isMuted);
                Debug.Log($"AudioManager synced: muted = {isMuted}");
            }
        }

        optionsPanel.SetActive(false);
    }

    private void LoadTreeCalculator()
    {
        SceneManager.LoadScene("Tree Screen");
    }

    private void LoadStumpCalculator()
    {
        SceneManager.LoadScene("Stump Screen");
    }

    private void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    private void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    private void OnSoundToggle(bool isOn)
    {
        Debug.Log($"OnSoundToggle called: isOn = {isOn}");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSoundMuted(!isOn);
            Debug.Log($"AudioManager.SetSoundMuted called: muted = {!isOn}, SoundMuted = {PlayerPrefs.GetInt("SoundMuted", 0)}");
        }
        else
        {
            PlayerPrefs.SetInt("SoundMuted", isOn ? 0 : 1);
            PlayerPrefs.Save();
            Debug.LogWarning($"AudioManager.Instance is null, saved SoundMuted = {(isOn ? 0 : 1)}");
        }
    }

    private void ExitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
        try
        {
            Application.Quit();
            using (var system = new AndroidJavaClass("java.lang.System"))
            {
                system.CallStatic("exit", 0);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to exit app: " + e.Message);
        }
#else
        Application.Quit();
#endif
    }
}