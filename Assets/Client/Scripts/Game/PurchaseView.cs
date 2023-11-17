using UnityEngine;

public class PurchaseView : MonoBehaviour
{
    public void EnableView()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableView()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
