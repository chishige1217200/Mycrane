using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundTester : MonoBehaviour
{
    [SerializeField] AudioSource[] audioSources;
    [SerializeField] Text idText;
    [SerializeField] Text pitchText;
    [SerializeField] Slider s;
    private int nowPlaying = 0; // 選択中の音声
    void Start()
    {
        audioSources = GetComponentsInChildren<AudioSource>(); // 子オブジェクトのすべての音声を格納
        Debug.Log(audioSources.Length);
    }

    public void PlayLoop() // BGM/SE用
    {
        audioSources[nowPlaying].loop = true;
        audioSources[nowPlaying].Play();
    }

    public void PlayOnce() // SE用
    {
        audioSources[nowPlaying].loop = false;
        audioSources[nowPlaying].Play();
    }

    public void PlayInstance() // SE用(未使用)
    {
        audioSources[nowPlaying].loop = false;
        audioSources[nowPlaying].PlayOneShot(audioSources[nowPlaying].clip);
    }

    public void Stop() // SE用
    {
        audioSources[nowPlaying].Stop();
    }

    public void StopAll() // BGM用
    {
        for (int i = 0; i < audioSources.Length; i++)
            audioSources[i].Stop();
    }

    public void SelectAudioNum(int num)
    {
        nowPlaying += num;
        if (nowPlaying < 0)
            nowPlaying = audioSources.Length - 1;
        else if (nowPlaying >= audioSources.Length)
            nowPlaying = 0;

        idText.text = nowPlaying.ToString("D3");
        pitchText.text = audioSources[nowPlaying].pitch.ToString("0.000");
        s.value = ((audioSources[nowPlaying].pitch - 0.75f) / 0.025f);
    }

    public void UpdatePitchText()
    {
        pitchText.text = (0.75f + 0.025f * s.value).ToString("0.000");
    }

    public void SetPitch() // ピッチ適用ボタン用
    {
        audioSources[nowPlaying].pitch = 0.75f + 0.025f * s.value;
    }
}
