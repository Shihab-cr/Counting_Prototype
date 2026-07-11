using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGameLogic : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject optionsMenuUI;
    
    private bool isPaused = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPaused = false;
        pauseMenuUI.SetActive(isPaused);
        if(optionsMenuUI != null)
            optionsMenuUI.SetActive(isPaused);
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            TogglePauseMenuUI(!isPaused);
        }
        

        
    }
    public void TogglePauseMenuUI(bool pauseState)
    {
        isPaused = pauseState;
        pauseMenuUI.SetActive(isPaused);

        if (!isPaused && optionsMenuUI != null) {
            optionsMenuUI.SetActive(false);
        }

        Time.timeScale = (!isPaused) ? 1.0f : 0.0f;
        
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState= CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void ResumeGame()
    {
        TogglePauseMenuUI(false);
    }
    public void ReturnToMainMenu()
    {
        //SceneManager.LoadSceneAsync()
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
    public void ToggleOptionsMenu()
    {
        bool optionsAreVisible = optionsMenuUI.activeSelf;
        optionsMenuUI.SetActive(optionsAreVisible);
        pauseMenuUI.SetActive(!optionsAreVisible);
    }
    public void ExitGame()
    {
        //implement exit game code;
        Application.Quit();
    }
}
