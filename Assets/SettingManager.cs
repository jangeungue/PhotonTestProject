using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviourPunCallbacks
{
    public static SettingManager Instance;

    [SerializeField]
    GameObject settingPopup;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        settingPopup.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetSettingPopup();
        }
    }

    void SetSettingPopup()
    {
        if (settingPopup.activeSelf)
        {
            settingPopup.SetActive(false);
        }
        else
        {
            settingPopup.SetActive(true);
        }
    }
}
