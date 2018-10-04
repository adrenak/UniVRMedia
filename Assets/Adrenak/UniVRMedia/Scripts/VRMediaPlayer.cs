using System;
using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

namespace Adrenak.UniVRMedia {
    public class VRMediaPlayer {
        [Serializable]
        public class Configuration {
            /// <summary>
            /// The position of the sphere video player and the camera in world space
            /// </summary>
            public Vector3 position;

            /// <summary>
            /// The color of the video player surface while the stream initializes
            /// </summary>
            public Color bgColor;

            /// <summary>
            /// The layers that the player camera should render
            /// </summary>
            public LayerMask cameraCullilngMask;

            /// <summary>
            /// The layer to which the video player surface belongs
            /// </summary>
            [SerializeField, Layer]
            public int playerLayer;
        }

		/// <summary>
		/// The <see cref="VideoPlayer"/> component that is playing the video
		/// </summary>
        public VideoPlayer Video { get; private set; }

		/// <summary>
		/// The <see cref="AudioSource"/> component that is playing the audio
		/// </summary>
        public AudioSource Audio { get; private set; }

		/// <summary>
		/// The <see cref="GameObject"/> that is the parent of all the player setup
		/// </summary>
        public GameObject Host { get; private set; }

		/// <summary>
		/// The <see cref="Camera"/> that is used for rendering the video sphere
		/// </summary>
        public GameObject Cam { get; private set; }

		/// <summary>
		/// The <see cref="Configuration"/> object that is used to appl the settings
		/// </summary>
        public Configuration Config { get; private set; }

        Dictionary<Camera, bool> m_CameraStatus = new Dictionary<Camera, bool>();
        float m_TimeScaleMemento;

        // ================================================
        // PUBLIC METHODS
        // ================================================
        /// <summary>
        /// Creates an instance using the configuration given
        /// </summary>
        /// <param name="config"></param>
        public VRMediaPlayer(Configuration config) {
            Config = config;
            CreateHostObject();
            ManageComponents();
            InvertNormals();
            SetColor(Config.bgColor);
            SetupCamera();
        }

        /// <summary>
        /// Use this instead of VideoPlayer.Play
        /// </summary>
        public void Play() {
            SaveCameras();
            StopCameras();
            Cam.GetComponent<Camera>().enabled = true;
            m_TimeScaleMemento = Time.timeScale;
            Time.timeScale = 0;
            Video.Play();
        }
        /// <summary>
        /// Use this instead of VideoPlayer.Pause
        /// </summary>
        public void Pause() {
            Video.Pause();
        }

        /// <summary>
        /// Use this instead of VideoPlayer.Stop
        /// </summary>
        public void Stop() {
            ResetCameras();
            Video.Stop();
            MonoBehaviour.Destroy(Host);
            Time.timeScale = m_TimeScaleMemento;
        }

        // ================================================
        // PRIVATE METHODS
        // ================================================
        void CreateHostObject() {
            Host = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Host.layer = Config.playerLayer;
            Host.name = "UniVRMediaObject";
            Host.transform.localScale = new Vector3(-100F, 100F, 100F);
            Host.transform.position = Config.position;
        }

        void ManageComponents() {
            var collider = Host.GetComponent<Collider>();
            if (collider != null) UnityEngine.Object.Destroy(collider);

            Video = Host.AddComponent<VideoPlayer>();
            Audio = Host.AddComponent<AudioSource>();

            Video.audioOutputMode = VideoAudioOutputMode.AudioSource;
            Video.SetTargetAudioSource(0, Audio);
        }

        void InvertNormals() {
            var filter = Host.GetComponent<MeshFilter>();
            filter.mesh.InvertNormals();
        }

        void SetColor(Color color) {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();

            var renderer = Host.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("Unlit/Texture");
            renderer.material.mainTexture = tex;
        }

        void SetupCamera() {
            var cameraRoot = new GameObject("UniVRMediaCameraRoot");
            cameraRoot.transform.position = Config.position;
            // Create a new camera at the center of the player
            Cam = new GameObject("UniVRMediaCamera");
            Cam.AddComponent<CameraControls>();
            Cam.transform.SetParent(cameraRoot.transform);
            Cam.transform.localPosition = Vector3.zero;

            // Initialize the camera component. Ignore the layers and set the clipping planes
            var cam = Cam.AddComponent<Camera>();
            cam.cullingMask = Config.cameraCullilngMask;

            cam.nearClipPlane = 1F;
            cam.farClipPlane = 1000F;
        }

        void SaveCameras() {
            // We store the current state of all the cameras and disable them
            var cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (var c in cameras) 
                m_CameraStatus.Add(c, c.enabled);
        }

        void ResetCameras() {
            // We reset all the old cameras to their origial states and clear the dictionary
            foreach (var p in m_CameraStatus)
                p.Key.enabled = p.Value;
            m_CameraStatus.Clear();
        }

        void StopCameras() {
            foreach (var pair in m_CameraStatus)
                pair.Key.enabled = false;
        }
    }
}
