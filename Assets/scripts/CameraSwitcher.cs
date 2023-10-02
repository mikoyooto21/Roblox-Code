using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] GameObject pcCam;
    [SerializeField] GameObject pcCinemaCam;
    [SerializeField] GameObject mobileCam;

    // Start is called before the first frame update
    void Start()
    {
        if (YandexGame.EnvironmentData.isMobile)
        {
            pcCam.SetActive(false);
            pcCinemaCam.SetActive(false);
        }
        if (YandexGame.EnvironmentData.isDesktop)
        {
            mobileCam.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
