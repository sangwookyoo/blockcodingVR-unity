using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Events that will highlight an object if it is a valid Grabbable
    /// </summary>
    public class GrabbableHighlight : GrabbableEvents {

        public bool HighlightOnGrabbable = true;
        public bool HighlightOnRemoteGrabbable = true;

        Outline outline;

        void Start() {
            outline = GetComponent<Outline>();

            if (HighlightOnGrabbable || HighlightOnRemoteGrabbable) {
                if (outline == null) {
                    // Is there a renderer attached?
                    if (GetComponent<Renderer>()) {
                        outline = gameObject.AddComponent<Outline>();
                        outline.eraseRenderer = false;
                        outline.enabled = false;
                    }
                    else {
                        // Try in child object
                        Renderer childRenderer = GetComponentInChildren<Renderer>();
                        if (childRenderer != null) {
                            outline = childRenderer.gameObject.AddComponent<Outline>();
                            outline.eraseRenderer = false;
                            outline.enabled = false;
                        }
                    }
                }
            }

            // Make sure camera can see outlines
            OutlineEffect oe = Camera.main.GetComponent<OutlineEffect>();
            if (oe == null) {
                oe = Camera.main.gameObject.AddComponent<OutlineEffect>();
                oe.lineThickness = 2;
                oe.lineIntensity = 1f;
                oe.fillAmount = 0;
                oe.lineColor0 = Color.white;
                oe.cornerOutlines = true;
                oe.scaleWithScreenSize = false;
            }
        }

        // Item has been grabbed by a Grabber
        public override void OnGrab(Grabber grabber) {
            UnhighlightItem();
        }

        // Fires if this is the closest grabbable but wasn't in the previous frame
        public override void OnBecomesClosestGrabbable(ControllerHand touchingHand) {
            if (HighlightOnGrabbable) {
                HighlightItem();
            }
        }

        public override void OnNoLongerClosestGrabbable(ControllerHand touchingHand) {
            if (HighlightOnGrabbable) {
                UnhighlightItem();
            }
        }

        public override void OnBecomesClosestRemoteGrabbable(ControllerHand touchingHand) {
            if (HighlightOnRemoteGrabbable) {
                HighlightItem();
            }
        }

        public override void OnNoLongerClosestRemoteGrabbable(ControllerHand touchingHand) {
            if (HighlightOnRemoteGrabbable) {
                UnhighlightItem();
            }
        }
        public void HighlightItem() {
            if (outline != null) {
                outline.enabled = true;
            }
        }

        public void UnhighlightItem() {
            if (outline != null) {
                outline.enabled = false;
            }
        }
    }
}

