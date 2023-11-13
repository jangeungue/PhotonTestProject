using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BGMManager : MonoBehaviour
{
    [SerializeField]
    AudioClip[] BGMClips;

    [SerializeField]
    AudioSource bgmPlayer;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (BGMClips != null)
        {
            var randomMusic = Random.Range(0, BGMClips.Length);
            SetBGM(BGMClips[randomMusic].name);
        }
    }

    void SetBGM(string bgmClipName)
    {
        if (PV.IsMine)
        {
            // ���� �÷��̾ BGM�� �����ϰ� RPC�� �ٸ� �÷��̾�� ����
            PV.RPC("RPC_SetBGM", RpcTarget.All, bgmClipName);
        }
    }

    [PunRPC]
    void RPC_SetBGM(string bgmClipName)
    {
        AudioClip bgmClip = System.Array.Find(BGMClips, clip => clip.name == bgmClipName);
        bgmPlayer.clip = bgmClip;
        bgmPlayer.Play();
    }

}

