using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void ContinueGame()
    {
        GameManager.instance.PauseSwitch();
    }

    public void DisableObject(GameObject @object)
    {
        @object.SetActive(false);
    }

    public void EnableObject(GameObject @object)
    {
        @object.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
