using UnityEngine;
using UnityEngine.Video;
using System.Threading.Tasks;

public class VideoPlay : MonoBehaviour
{
    public VideoPlayer videoPlayer; //オーディオ情報の格納
    public VideoClip[] videoClips;
    public bool randomMode = false; //trueのときランダムにビデオを再生
    bool videoPlayFlag = false;
    float playTime = 0; //ビデオ再生時間
    float startTime;

    void Update()
    {
        if (randomMode)
        {
            if (playTime - Time.time + startTime < 0)
            {
                Debug.Log("New Video Autoplayed.");
                playTime = Random.Range(3, 8);
                startTime = Time.time;
                PlayVideo(Random.Range(0, videoClips.Length));
            }
        }

        if (videoPlayer.isPrepared && videoPlayFlag)
        {
            videoPlayer.Play();
            return;
        }
    }

    public void PlayVideo(int num)
    {
        videoPlayFlag = false;
        videoPlayer.clip = videoClips[num];
        videoPlayer.Prepare();
        videoPlayFlag = true;
    }
}
