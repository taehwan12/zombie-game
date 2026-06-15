using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionUI : MonoBehaviour
{
    public void SelectPark()
    {
        SceneManager.LoadScene("zombie game");
    }

    public void SelectDesert()
    {
        SceneManager.LoadScene("ZombieGame_NewMap");
    }

    public void BackToIntro()
    {
        SceneManager.LoadScene("Intro");
    }
}
