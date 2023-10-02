using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class NewOrCont : MonoBehaviour
{
    [SerializeField] GameObject newButton;
    [SerializeField] GameObject continueButton;


    // Start is called before the first frame update
    void Start()
    {
        if(YandexGame.savesData.isExited == 1 || PlayerPrefs.GetInt("PlayerExited") == 1)
        {
            newButton.SetActive(false);
            continueButton.SetActive(true);
        }
        if (YandexGame.savesData.isExited == 0 || PlayerPrefs.GetInt("PlayerExited") == 0)
        {
            newButton.SetActive(true);
            continueButton.SetActive(false);
        }
    }
}
