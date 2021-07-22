using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    /// <summary>
    /// A marker for valid teleport destinations
    /// </summary>
    public class TeleportDestination : MonoBehaviour {

        /// <summary>
        /// Where the player will be teleported to
        /// </summary>
        public Transform DestinationTransform;

        /// <summary>
        /// Snap player to this rotation?
        /// </summary>
        public bool ForcePlayerRotation = false;
    }
}