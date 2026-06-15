using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroUI : MonoBehaviour
{
    public string gameSceneName = "zombie game";
    public TMP_InputField nameInputField; // 이름 입력창

    public void StartGame()
    {
        // 이름 저장 (랭킹 시스템용)
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            PlayerPrefs.SetString("PlayerName", nameInputField.text);
            PlayerPrefs.Save();
        }
        else
        {
            // 이름이 비어있으면 기본값 저장
            PlayerPrefs.SetString("PlayerName", "Unknown");
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenRanking()
    {
        SceneManager.LoadScene("Ranking");
    }
}