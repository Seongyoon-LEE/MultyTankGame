using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager s_Instance;
    public bool isMute = false;
    void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            Destroy(s_Instance);
    }

    public void PlaySfx(Vector3 pos, AudioClip clip, bool isLooped)
    {
        if (isLooped) return;

        GameObject soundObj = new GameObject("SoundSFX~~");

        soundObj.transform.position = pos;
        AudioSource audioSource = soundObj.AddComponent<AudioSource>(); // 컴퍼넌트가 없으면 새로 생성한다.

        audioSource.clip = clip;
        audioSource.loop = isLooped;
        audioSource.minDistance = 20f;
        audioSource.maxDistance = 100f;
        audioSource.volume = 1.0f;
        audioSource.Play();

        Destroy(soundObj, audioSource.clip.length);
    }
}
