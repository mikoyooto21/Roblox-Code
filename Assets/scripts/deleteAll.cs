using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class deleteAll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        YandexGame.ResetSaveProgress();
        YandexGame.SaveProgress();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
