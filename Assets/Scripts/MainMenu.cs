using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text highScoreUI;
    string newGameScene = "SampleScene";

    public AudioClip bg_music;
    public AudioSource main_channel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main_channel.PlayOneShot(bg_music);

        // Set the high score text
        int highScore = SaveLoadManager.Instance.LoadHighScore();

        // highScoreUI = GameObject.FindGameObjectWithTag("HighScoreUI").GetComponent<TMP_Text>();
        highScoreUI.text = $"Top Wave Survived: {highScore}";
    }

    public void StartNewGame()
    {
        main_channel.Stop();

        SceneManager.LoadScene(newGameScene);
    }

    public void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
