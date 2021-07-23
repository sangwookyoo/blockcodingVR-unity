using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BNG {

    [CustomEditor(typeof(Grabbable))]
    [CanEditMultipleObjects]
    public class GrabbableEditor : Editor {

        /// <summary>
        /// Set this to false if you don't want to use the custom editor :)
        /// </summary>
        public bool UseCustomEditor = true;

        Grabbable grabbable;

        SerializedProperty grabButton;
        SerializedProperty grabType;
        SerializedProperty grabPhysics;
        SerializedProperty grabMechanic;
        SerializedProperty grabSpeed;
        SerializedProperty remoteGrabbable;
        SerializedProperty remoteGrabDistance;
        SerializedProperty throwForceMultiplier;
        SerializedProperty throwForceMultiplierAngular;
        SerializedProperty breakDistance;
        SerializedProperty hideHandGraphics;
        SerializedProperty parentToHands;
        SerializedProperty parentHandModel;
        SerializedProperty snapHandModel;
        SerializedProperty canBeDropped;
        SerializedProperty CanBeSnappedToSnapZone;
        SerializedProperty ForceDisableKinematicOnDrop;
        SerializedProperty CustomHandPose;
        SerializedProperty SecondaryGrabBehavior;
        SerializedProperty OtherGrabbableMustBeGrabbed;
        SerializedProperty SecondaryGrabbable;
        SerializedProperty SecondHandLookSpeed;
        SerializedProperty CollisionSpring;
        SerializedProperty CollisionSlerp;
        SerializedProperty CollisionLinearMotionX;
        SerializedProperty CollisionLinearMotionY;
        SerializedProperty CollisionLinearMotionZ;
        SerializedProperty CollisionAngularMotionX;
        SerializedProperty CollisionAngularMotionY;
        SerializedProperty CollisionAngularMotionZ;
        SerializedProperty ApplyCorrectiveForce;
        SerializedProperty MoveVelocityForce;
        SerializedProperty MoveAngularVelocityForce;
        SerializedProperty GrabPoints;
        SerializedProperty collisions;


        void OnEnable() {
            grabButton = serializedObject.FindProperty("GrabButton");
            grabType = serializedObject.FindProperty("Grabtype");
            grabPhysics = serializedObject.FindProperty("GrabPhysics");
            grabMechanic = serializedObject.FindProperty("GrabMechanic");
            grabSpeed = serializedObject.FindProperty("GrabSpeed");
            remoteGrabbable = serializedObject.FindProperty("RemoteGrabbable");
            remoteGrabDistance = serializedObject.FindProperty("RemoteGrabDistance");
            throwForceMultiplier = serializedObject.FindProperty("ThrowForceMultiplier");
            throwForceMultiplierAngular = serializedObject.FindProperty("ThrowForceMultiplierAngular");
            breakDistance = serializedObject.FindProperty("BreakDistance");
            hideHandGraphics = serializedObject.FindProperty("HideHandGraphics");
            parentToHands = serializedObject.FindProperty("ParentToHands");
            parentHandModel = serializedObject.FindProperty("ParentHandModel");
            snapHandModel = serializedObject.FindProperty("SnapHandModel");
            canBeDropped = serializedObject.FindProperty("CanBeDropped");
            CanBeSnappedToSnapZone = serializedObject.FindProperty("CanBeSnappedToSnapZone");
            ForceDisableKinematicOnDrop = serializedObject.FindProperty("ForceDisableKinematicOnDrop");
            CustomHandPose = serializedObject.FindProperty("CustomHandPose");
            SecondaryGrabBehavior = serializedObject.FindProperty("SecondaryGrabBehavior");
            OtherGrabbableMustBeGrabbed = serializedObject.FindProperty("OtherGrabbableMustBeGrabbed");
            SecondaryGrabbable = serializedObject.FindProperty("SecondaryGrabbable");
            SecondHandLookSpeed = serializedObject.FindProperty("SecondHandLookSpeed");
            CollisionSpring = serializedObject.FindProperty("CollisionSpring");
            CollisionSlerp = serializedObject.FindProperty("CollisionSlerp");
            CollisionLinearMotionX = serializedObject.FindProperty("CollisionLinearMotionX");
            CollisionLinearMotionY = serializedObject.FindProperty("CollisionLinearMotionY");
            CollisionLinearMotionZ = serializedObject.FindProperty("CollisionLinearMotionZ");
            CollisionAngularMotionX = serializedObject.FindProperty("CollisionAngularMotionX");
            CollisionAngularMotionY = serializedObject.FindProperty("CollisionAngularMotionY");
            CollisionAngularMotionZ = serializedObject.FindProperty("CollisionAngularMotionZ");
            ApplyCorrectiveForce = serializedObject.FindProperty("ApplyCorrectiveForce");
            MoveVelocityForce = serializedObject.FindProperty("MoveVelocityForce");
            MoveAngularVelocityForce = serializedObject.FindProperty("MoveAngularVelocityForce");
            GrabPoints = serializedObject.FindProperty("GrabPoints");
            collisions = serializedObject.FindProperty("collisions");
        }

        public override void OnInspectorGUI() {

            grabbable = (Grabbable)target;

            // Don't use Custom Editor
            if (UseCustomEditor == false || grabbable.UseCustomInspector == false) {
                base.OnInspectorGUI();
                return;
            }

            EditorGUILayout.PropertyField(grabButton);
            EditorGUILayout.PropertyField(grabType, new GUIContent("Grab Type"));
            EditorGUILayout.PropertyField(grabMechanic);

            EditorGUILayout.PropertyField(grabPhysics);
            if (grabbable.GrabPhysics == GrabPhysics.PhysicsJoint) {
                EditorGUILayout.PropertyField(CollisionSpring);
                EditorGUILayout.PropertyField(CollisionSlerp);
                EditorGUILayout.PropertyField(CollisionLinearMotionX);
                EditorGUILayout.PropertyField(CollisionLinearMotionY);
                EditorGUILayout.PropertyField(CollisionLinearMotionZ);
                EditorGUILayout.PropertyField(CollisionAngularMotionX);
                EditorGUILayout.PropertyField(CollisionAngularMotionY);
                EditorGUILayout.PropertyField(CollisionAngularMotionZ);
            }

            if (grabbable.GrabPhysics == GrabPhysics.PhysicsJoint || grabbable.GrabPhysics == GrabPhysics.FixedJoint) {
                EditorGUILayout.PropertyField(ApplyCorrectiveForce);
            }

            if (grabbable.GrabPhysics == GrabPhysics.Velocity) {
                EditorGUILayout.PropertyField(MoveVelocityForce);
                EditorGUILayout.PropertyField(MoveAngularVelocityForce);
            }

            EditorGUILayout.PropertyField(grabSpeed);
            EditorGUILayout.PropertyField(throwForceMultiplier);
            EditorGUILayout.PropertyField(throwForceMultiplierAngular);
            EditorGUILayout.PropertyField(remoteGrabbable);
            EditorGUILayout.PropertyField(remoteGrabDistance);

            EditorGUILayout.PropertyField(hideHandGraphics);
            EditorGUILayout.PropertyField(parentToHands);
            EditorGUILayout.PropertyField(parentHandModel);
            EditorGUILayout.PropertyField(snapHandModel);

            EditorGUILayout.PropertyField(canBeDropped);
            EditorGUILayout.PropertyField(CanBeSnappedToSnapZone);
            EditorGUILayout.PropertyField(ForceDisableKinematicOnDrop);
            EditorGUILayout.PropertyField(breakDistance);

            EditorGUILayout.PropertyField(CustomHandPose);

            EditorGUILayout.PropertyField(SecondaryGrabBehavior);
            EditorGUILayout.PropertyField(OtherGrabbableMustBeGrabbed);
            EditorGUILayout.PropertyField(SecondaryGrabbable);
            EditorGUILayout.PropertyField(SecondHandLookSpeed);

            EditorGUILayout.PropertyField(GrabPoints);

            // Grab Point Button
            if(GUILayout.Button("Auto Populate Grab Points")) {
                AutoPopulateGrabPoints();
            }

            // Only show Debug Fields when playing in editor
            if (Application.isPlaying) {
                EditorGUILayout.PropertyField(collisions);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        public void AutoPopulateGrabPoints() {
            if(grabbable) {
                var newPoints = new List<Transform>();

                foreach(var gp in grabbable.GetComponentsInChildren<GrabPoint>()) {
                    if(gp != null && gp.gameObject.activeInHierarchy) {
                        newPoints.Add(gp.transform);
                    }
                }

                grabbable.GrabPoints = newPoints;
            }
        }

        //[MenuItem("GameObject/VRIF/Grabbable")]
        //private static void AddGrabbable() {
        //    // Create and add a new Grabbable Object in the Scene
        //    Debug.Log("Not yet implemented");
        //}
    }
}
