using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviourPunCallbacks
{

    public float masterVolumeBGM = 1f;
    public float masterVolumeSFX = 1f;

    [SerializeField] AudioClip[] audioClip; // ����� �ҽ��� ����.

    Dictionary<string, AudioClip> audioClipsDic;
    AudioSource bgmPlayer;
    AudioSource sfxPlayer;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        AwakeAfter();
    }

    void AwakeAfter()
    {
        sfxPlayer = GetComponent<AudioSource>();

        audioClipsDic = new Dictionary<string, AudioClip>();
        foreach (AudioClip a in audioClip)
        {
            audioClipsDic.Add(a.name, a);
        }
    }


    public void PlaySound(string a_name, float a_volume = 1f)
    {
        if (audioClipsDic.ContainsKey(a_name) == false)
        {
            Debug.Log(a_name + " is not Contained audioClipsDic");
            return;
        }
        if (PV.IsMine)
        {
            sfxPlayer.PlayOneShot(audioClipsDic[a_name], a_volume * masterVolumeSFX);
        }
        // 다른 플레이어에게 RPC 호출
        PV.RPC("PlaySoundRPC", RpcTarget.All, a_name, a_volume);
    }

    [PunRPC]
    void PlaySoundRPC(string a_name, float a_volume = 1f)
    {
        // AudioSource 생성 및 설정
        GameObject audioObject = new GameObject("AudioSource_" + a_name);
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        // 소리 재생
        audioSource.PlayOneShot(audioClipsDic[a_name], a_volume * masterVolumeSFX);

        // 소리가 재생된 후 AudioSource를 파괴
        Destroy(audioObject, audioClipsDic[a_name].length);
    }


    public void PlayRandomSound(string[] a_nameArray, float a_volume = 1f)
    {
        string l_playClipName;

        l_playClipName = a_nameArray[Random.Range(0, a_nameArray.Length)];

        if (audioClipsDic.ContainsKey(l_playClipName) == false)
        {
            Debug.Log(l_playClipName + " is not Contained audioClipsDic");
            return;
        }
        sfxPlayer.PlayOneShot(audioClipsDic[l_playClipName], a_volume * masterVolumeSFX);
    }


    public GameObject PlayLoopSound(string a_name)
    {
        if (audioClipsDic.ContainsKey(a_name) == false)
        {
            Debug.Log(a_name + " is not Contained audioClipsDic");
            return null;
        }

        GameObject l_obj = new GameObject("LoopSound");
        AudioSource source = l_obj.AddComponent<AudioSource>();
        source.clip = audioClipsDic[a_name];
        source.volume = masterVolumeSFX;
        source.loop = true;
        source.Play();
        return l_obj;
    }

    
    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    #region SetVolume

    [SerializeField]
    Slider sliderToBGM;
    [SerializeField]
    Slider sliderToSFX;

    public void SetVolumeBGM()
    {
        masterVolumeBGM = sliderToBGM.value;
        bgmPlayer.volume = masterVolumeBGM;
    }

    public void SetVolumeSFX()
    {
        masterVolumeSFX = sliderToSFX.value;

        // 로컬 플레이어의 경우, 자신의 RPC를 먼저 호출합니다.
        if (PV.IsMine)
        {
            SetMyVolumeSFXRPC(masterVolumeSFX);
        }
    }

    void SetMyVolumeSFXRPC(float volume)
    {
        // 로컬 플레이어를 위해 SFX 볼륨을 업데이트합니다.
        masterVolumeSFX = volume;

        sfxPlayer.volume = masterVolumeSFX;

        PV.RPC("SetVolumeSFXRPC", PV.Owner, masterVolumeSFX);
    }

    [PunRPC]
    void SetVolumeSFXRPC(float volume)
    {
        masterVolumeSFX = volume;


        sfxPlayer.volume = masterVolumeSFX;

    }


    #endregion
}