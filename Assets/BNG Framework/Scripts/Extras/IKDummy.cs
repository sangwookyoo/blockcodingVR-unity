using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class IKDummy : MonoBehaviour {

        public Transform PlayerTransform;

        public Transform HeadFollow;
        public Transform RightHandFollow;
        public Transform LeftHandFollow;

        public Vector3 HandRotationOffset = Vector3.zero;

        Animator animator;
        Transform headBone;

        Transform leftHandDummy;
        Transform rightHandDummy;
        Transform lookatDummy;

        Vector3 localPos;
        Quaternion localRot;

        Transform cam;

        // Start is called before the first frame update
        void Start() {
            cam = Camera.main.transform;
            animator = GetComponent<Animator>();
            headBone = animator.GetBoneTransform(HumanBodyBones.Head);

            leftHandDummy = new GameObject().transform;
            rightHandDummy = new GameObject().transform;
            lookatDummy = new GameObject().transform;

           
        }

        // Update is called once per frame
        void LateUpdate() {
            transform.LookAt(cam);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

            lookatDummy.position = HeadFollow.position;
            lookatDummy.parent = PlayerTransform;
            localPos = lookatDummy.localPosition;

            lookatDummy.parent = transform;
            lookatDummy.localPosition = localPos;

            // Left Hand
            leftHandDummy.position = LeftHandFollow.position;
            leftHandDummy.rotation = LeftHandFollow.rotation;
            leftHandDummy.parent = PlayerTransform;
            localPos = leftHandDummy.localPosition;
            localRot = leftHandDummy.localRotation;

            leftHandDummy.parent = transform;
            leftHandDummy.localPosition = localPos;
            leftHandDummy.localRotation = localRot;
            leftHandDummy.localEulerAngles += HandRotationOffset;

            // Right Hand
            rightHandDummy.position = RightHandFollow.position;
            rightHandDummy.rotation = RightHandFollow.rotation;
            rightHandDummy.parent = PlayerTransform;
            localPos = rightHandDummy.localPosition;
            localRot = rightHandDummy.localRotation;

            rightHandDummy.parent = transform;
            rightHandDummy.localPosition = localPos;
            rightHandDummy.localRotation = localRot;
            rightHandDummy.localEulerAngles -= HandRotationOffset;
        }

        void OnAnimatorIK() {
            // Set weights for all IK goals
            animator.SetLookAtWeight(1);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

            animator.SetLookAtPosition(cam.position);
            animator.SetLookAtPosition(lookatDummy.position);

            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandDummy.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandDummy.rotation);

            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandDummy.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandDummy.rotation);
        }
    }
}

