using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    /// <summary>
    /// Keeps track of Grabbables that are within it's Trigger
    /// </summary>
    public class RemoteGrabber : MonoBehaviour {

        // Grabber we can hand objects off to
        public GrabbablesInTrigger ParentGrabber;

        void OnTriggerEnter(Collider other) {

            //  We will let this grabber know we have remote objects available            
            if (ParentGrabber == null) {
                return;
            }
            
            // Ignore Raycast Triggers
            if(other.gameObject.layer == 2) {
                return;
            }

            Grabbable grabObject = other.GetComponent<Grabbable>();
            if(grabObject != null && ParentGrabber != null) {
                ParentGrabber.AddValidRemoteGrabbable(other, grabObject);
                return;
            }

            // Check for Grabbable Child Object Last
            GrabbableChild gc = other.GetComponent<GrabbableChild>();
            if (gc != null && ParentGrabber != null) {
                ParentGrabber.AddValidRemoteGrabbable(other, gc.ParentGrabbable);
                return;
            }
        }

        void OnTriggerExit(Collider other) {

            Grabbable grabObject = other.GetComponent<Grabbable>();
            if (grabObject != null && ParentGrabber != null) {
                ParentGrabber.RemoveValidRemoteGrabbable(other, grabObject);
                return;
            }

            // Check for Grabbable Child Object Last
            GrabbableChild gc = other.GetComponent<GrabbableChild>();
            if (gc != null && ParentGrabber != null) {
                ParentGrabber.RemoveValidRemoteGrabbable(other, gc.ParentGrabbable);
                return;
            }
        }
    }
}