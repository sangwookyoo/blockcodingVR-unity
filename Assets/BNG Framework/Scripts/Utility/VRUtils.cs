using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNG {

    /// <summary>
    /// Static Utilities to help with development, such as logging to World Space
    /// </summary>
    public class VRUtils : MonoBehaviour {

        public static VRUtils Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<VRUtils>();
                    if (_instance == null) {
                        _instance = new GameObject("VRUtils").AddComponent<VRUtils>();
                    }
                }
                return _instance;
            }
        }
        private static VRUtils _instance;

        // Where to put our text messages
        public Color DebugTextColor = Color.white;
        public Transform DebugTextHolder;
        /// <summary>
        /// Maximum number of Text lines before we start removing them
        /// </summary>
        float MaxTextEntries = 10;

        void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(this);
                return;
            }

            _instance = this;
        }                    
        
        /// <summary>
        /// Log to a WorldSpace object if available
        /// </summary>
        /// <param name="msg"></param>
        public void Log(string msg) {
            Debug.Log(msg);

            // Add to Holder if available
            if(DebugTextHolder != null) {
                GameObject go = new GameObject();
                go.transform.parent = DebugTextHolder;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.name = "Debug Text";

                Text textLine = go.AddComponent<Text>();
                textLine.text = msg;
                textLine.horizontalOverflow = HorizontalWrapMode.Wrap;
                textLine.verticalOverflow = VerticalWrapMode.Overflow;
                textLine.color = DebugTextColor;
                textLine.fontSize = 32;
                textLine.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                textLine.raycastTarget = false;

                RectTransform rect = go.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.identity;

                // Remove Text Line if we've exceed max
                if(DebugTextHolder.childCount > MaxTextEntries) {
                    DestroyImmediate(DebugTextHolder.GetChild(0).gameObject);
                }
            }
        }

        public AudioSource PlaySpatialClipAt(AudioClip clip, Vector3 pos, float volume, float spatialBlend = 1f, float randomizePitch = 0) {

            if(clip == null) {
                return null;
            }

            GameObject go = new GameObject("SpatialAudio - Temp");
            go.transform.position = pos;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;

            // Currently only Oculus Integration supports spatial audio
#if OCULUS_INTEGRATION
            source.spatialize = true;
#endif
            source.pitch = getRandomizedPitch(randomizePitch);
            source.spatialBlend = spatialBlend;
            source.volume = volume;
            source.Play();

            Destroy(go, clip.length);

            return source;
        }

        float getRandomizedPitch(float randomAmount) {

            if(randomAmount != 0) {
                float randomPitch = Random.Range(-randomAmount, randomAmount);
                return Time.timeScale + randomPitch;
            }

            return Time.timeScale;
        }
    }
}

