/*
MIT License

Copyright (c) 2017 Vatsal Ambastha

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

public class Uni360Video {
    [System.Serializable]
    public class Options {
        /// <summary>
        /// Defines the layers that will be rendered by the 
        /// </summary>
        public LayerMask cullingMask;

        /// <summary>
        /// The position of the sphere video player and the camera in world space
        /// </summary>
        public Vector3 playerPosition;

        /// <summary>
        /// Whether the video sound plays
        /// </summary>
        public bool mute;

        /// <summary>
        /// The color of the video player surface while the stream initializes
        /// </summary>
        public Color defaultColor;

        /// <summary>
        /// The Source of the video. If being streamed from Url or local path, don't forget to set <see cref="videoPath"/> as well.
        /// </summary>
        public VideoSource videoSource;

        /// <summary>
        /// Whether the video content should keep looping. In this case the video playback can be stopped using 
        /// </summary>
        public bool isLooping;

        /// <summary>
        /// The path to the video that is to be streamed. 
        /// </summary>
        public string videoPath;

        /// <summary>
        /// The video clip to be played if source chosen is VideoClip
        /// </summary>
        public VideoClip videoClip;
    }

    /// <summary>
    /// The component atteched to the 360 video sphere. Use this to direclty modify video playback parameters.
    /// </summary>
    public static VideoPlayer videoPlayer;

    static GameObject playerObject;
    static GameObject cameraObject;
    static Dictionary<Camera, bool> cameraStatusDict = new Dictionary<Camera, bool>();
    static float timeScaleMemento;

    public static void Init(Options options) {
        CreatePlayerObject(options.playerPosition);
        PrepPlayerSurface(options.defaultColor);
        InitializeVideoPlayerComponent(options);
        SetupCamera();

        ScaleDownTime();        
    }

    public static void Stop() {
        MonoBehaviour.Destroy(playerObject);
        ResetCameras();

        ScaleBackTime();
    }

    static void CreatePlayerObject(Vector3 position) {
        // Create a primitice sphere at the given location. The x axis scale is inverted
        // as the UV coordinates of the sphere would other wise invert the video (try it to confirm)
        playerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        playerObject.name = "Uni360VideoSurface";
        playerObject.transform.localScale = new Vector3(-.1F, .1F, .1F);
        playerObject.transform.position = position;

        // Remove collider component from it
        var collider = playerObject.GetComponent<Collider>();
        if (collider != null) MonoBehaviour.Destroy(collider);

        videoPlayer = playerObject.AddComponent<VideoPlayer>();
    }

    static void PrepPlayerSurface(Color defaultColor) {
        // Invret normals so the faces look inside the sphere
        var filter = playerObject.GetComponent<MeshFilter>();
        MeshFilterHelper.InvertNormals(filter);

        // Create a simple 1 pixel texture with the given color
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, defaultColor);
        tex.Apply();

        // Change the renderer shader to unlit and apply the texture
        var renderer = playerObject.GetComponent<Renderer>();
        renderer.material.shader = Shader.Find("Unlit/Texture");
        renderer.material.mainTexture = tex;
    }

    static void InitializeVideoPlayerComponent(Options options) {
        // Add and initialize audiosource if not mute
        if (!options.mute) {
            var audioSource = playerObject.AddComponent<AudioSource>();
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        // Handle video source and looping
        videoPlayer.source = options.videoSource;
        videoPlayer.url = options.videoPath;
        videoPlayer.isLooping = options.isLooping;
        videoPlayer.loopPointReached += delegate (VideoPlayer player) {
            if (!player.isLooping)
                Stop();
        };
    }

    static void SetupCamera() {
        // We store the current state of all the cameras and disable them
        var cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (var c in cameras) {
            cameraStatusDict.Add(c, c.enabled);
            c.enabled = false;
        }

        // Create a new camera at the center of the player
        cameraObject = new GameObject("Uni360Video_Camera");
        cameraObject.transform.parent = playerObject.transform;
        cameraObject.transform.localPosition = Vector3.zero;

        // Initialize it's camera component. Ignore the layers and set the clipping planes
        var cam = cameraObject.AddComponent<Camera>();
        cam.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

        cam.nearClipPlane = .01F;
        cam.farClipPlane = .1F;
    }

    static void ResetCameras() {
        // We reset all the old cameras to their origial states and clear the dictionary
        foreach (var p in cameraStatusDict)
            p.Key.enabled = p.Value;
        cameraStatusDict.Clear();
    }

    static void ScaleDownTime() {
        timeScaleMemento = Time.timeScale;
        Time.timeScale = 0;
    }

    static void ScaleBackTime() {
        Time.timeScale = timeScaleMemento;
    }
}
