using UnityEngine;
using UnityEngine.Video;
using UniVRMedia;

public class Context : MonoBehaviour {
    [SerializeField]
    VRMediaPlayer.Configuration options;

    public void Start () {
        VRMediaPlayer media = new VRMediaPlayer(options);

        media.Video.source = VideoSource.Url;
        media.Video.url = "http://www.vatsalambastha.com/downloads/sample-360-video.mp4";
        media.Video.isLooping = true;
        media.Video.loopPointReached += delegate (VideoPlayer source) {
            if (!media.Video.isLooping)
                media.Video.Stop();
        };

        media.Play();
    }
}
