using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public class ScaleMaterialHelper : MonoBehaviour {

        Renderer ren;

        public Vector2 Tiling = new Vector2(1,1);
        public Vector2 Offset;

        // Start is called before the first frame update
        void Start() {
            ren = GetComponent<Renderer>();
            updateTexture();
        }

        // Update is called once per frame
        void Update() {
            if (Application.isEditor) {
                updateTexture();
            }
        }

        void updateTexture() {
            ren.material.mainTextureScale = Tiling;
            ren.material.mainTextureOffset = Offset;
        }
    }
}

