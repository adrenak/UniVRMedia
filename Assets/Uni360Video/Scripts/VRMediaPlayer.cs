using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

namespace UniVRMedia {

    public class VRMediaPlayer {
        [System.Serializable]
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

        VideoPlayer m_VideoPlayer;
        AudioSource m_AudioSource;
        GameObject m_HostObject;
        GameObject m_CameraObject;
        Dictionary<Camera, bool> m_CameraStatus = new Dictionary<Camera, bool>();
        float m_TimeScaleMemento;
        Configuration m_Config;

        // ================================================
        // PUBLIC METHODS
        // ================================================
        /// <summary>
        /// Creates an instance using the configuration given
        /// </summary>
        /// <param name="config"></param>
        public VRMediaPlayer(Configuration config) {
            m_Config = config;
            CreateHostObject();
            ManageComponents();
            InvertNormals();
            SetColor(m_Config.bgColor);
            SetupCamera();
        }

        /// <summary>
        /// Use this instead of VideoPlayer.Play
        /// </summary>
        public void Play() {
            SaveCameras();
            StopCameras();
            m_CameraObject.GetComponent<Camera>().enabled = true;
            m_TimeScaleMemento = Time.timeScale;
            Time.timeScale = 0;
            m_VideoPlayer.Play();
        }
        /// <summary>
        /// Use this instead of VideoPlayer.Pause
        /// </summary>
        public void Pause() {
            m_VideoPlayer.Pause();
        }

        /// <summary>
        /// Use this instead of VideoPlayer.Stop
        /// </summary>
        public void Stop() {
            ResetCameras();
            m_CameraObject.GetComponent<Camera>().enabled = false;
            Time.timeScale = m_TimeScaleMemento;
            m_VideoPlayer.Stop();
        }

        // ================================================
        // PUBLIC PROPERTIES
        // ================================================
        public Configuration Config {
            get { return m_Config; }
        }

        public VideoPlayer Video {
            get { return m_VideoPlayer; }
        }

        public AudioSource Audio {
            get { return m_AudioSource; }
        }

        public GameObject HostObject {
            get { return m_HostObject; }
        }

        public GameObject CameraObject {
            get { return m_CameraObject; }
        }

        // ================================================
        // PRIVATE METHODS
        // ================================================
        void CreateHostObject() {
            m_HostObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_HostObject.layer = m_Config.playerLayer;
            m_HostObject.name = "UniVRMedia";
            m_HostObject.transform.localScale = new Vector3(-100F, 100F, 100F);
            m_HostObject.transform.position = m_Config.position;
        }

        void ManageComponents() {
            var collider = m_HostObject.GetComponent<Collider>();
            if (collider != null) MonoBehaviour.Destroy(collider);

            m_VideoPlayer = m_HostObject.AddComponent<VideoPlayer>();
            m_AudioSource = m_HostObject.AddComponent<AudioSource>();

            m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            m_VideoPlayer.SetTargetAudioSource(0, m_AudioSource);
        }

        void InvertNormals() {
            var filter = m_HostObject.GetComponent<MeshFilter>();
            filter.mesh.InvertNormals();
        }

        void SetColor(Color color) {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();

            var renderer = m_HostObject.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("Unlit/Texture");
            renderer.material.mainTexture = tex;
        }

        void SetupCamera() {
            // Create a new camera at the center of the player
            m_CameraObject = new GameObject("Camera");
            m_CameraObject.AddComponent<CameraControls>();
            m_CameraObject.transform.parent = m_HostObject.transform;
            m_CameraObject.transform.localPosition = Vector3.zero;

            // Initialize the camera component. Ignore the layers and set the clipping planes
            var cam = m_CameraObject.AddComponent<Camera>();
            cam.cullingMask = m_Config.cameraCullilngMask;

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
