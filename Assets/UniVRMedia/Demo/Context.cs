using UnityEngine;
using UnityEngine.Video;
using Adrenak.UniVRMedia;

public class Context : MonoBehaviour {
    [SerializeField]
    VRMediaPlayer.Configuration options;

    public void Start () {
        VRMediaPlayer player = new VRMediaPlayer(options);

        player.Video.source = VideoSource.Url;
        player.Video.url = "http://www.vatsalambastha.com/downloads/sample-360-video.mp4";
        player.Video.isLooping = true;
        player.Video.loopPointReached += delegate (VideoPlayer source) {
            if (!player.Video.isLooping)
                player.Video.Stop();
        };

        player.Play();
    }
}
