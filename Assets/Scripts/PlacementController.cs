using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

enum TouchObjectType {
    None,
    Placeable,
    Gui
}

[RequireComponent(typeof(ARRaycastManager))]
public class PlacementController : MonoBehaviour {
    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private PlacementInventory inventory;

    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject surfaceIndicator;
    private Pose placementPose;
    private Pose placeableBaseHitPose;
    private bool placementPoseValid = false;
    private bool placeableBaseHit = false;

    private ARRaycastManager arRaycastManager;

    private Vector2 lastTapPosition;
    private float lastTapTime = 0f;

    [SerializeField]
    private GameState gameState = default;

    void Awake() {
        arRaycastManager = GetComponent<ARRaycastManager>();

        // Disable multitouch
        Input.multiTouchEnabled = false;
    }



    void Update() {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            // record locaiton
            Touch touch = Input.GetTouch(0);
            lastTapPosition = touch.position;

            Invoke("HandleSingleTap", 0.2f);
            // Check time difference
            if (Time.time - lastTapTime < 0.18f) {
                // this is a double tap
                CancelInvoke("HandleSingleTap");
                lastTapTime = 0f;
                HandleDoubleTap();
            }
            lastTapTime = Time.time;
        }
    }

    private void HandleSingleTap() {
        Transform touchObj;
        TouchObjectType objType;

        GetHitObject(out touchObj, out objType);

        if (objType == TouchObjectType.Placeable) {
            Placeable domino = touchObj.GetComponent<Placeable>();
            gameState.SelectPlaceable(domino);
        } else if (objType == TouchObjectType.None) {
            gameState.ClearSelection();
        }
    }

    private void GetHitObject(out Transform touchObject, out TouchObjectType objType) {
        touchObject = default;
        objType = TouchObjectType.None;

        Ray ray = arCamera.ScreenPointToRay(lastTapPosition);
        RaycastHit hitObject;

        if (Physics.Raycast(ray, out hitObject)) {
            // pass out the hitObject first
            touchObject = hitObject.transform;

            // parse the hit type
            if (hitObject.transform.GetComponent<Placeable>()) {
                objType = TouchObjectType.Placeable;
            } else if (CheckPositionOverGui(lastTapPosition)) {
                objType = TouchObjectType.Gui;
            } else {
                objType = TouchObjectType.None;
            }
        }
    }

    private void HandleDoubleTap() {
        Debug.Log("Double tap detected...");
        Ray ray = arCamera.ScreenPointToRay(lastTapPosition);
        var bulletObj = Instantiate(bullet, arCamera.transform.position, new Quaternion());
        var bulletRigidbody = bulletObj.GetComponent<Rigidbody>();
        if (bulletRigidbody) {
            bulletRigidbody.velocity += ray.direction * 1.0f;
        }
    }

    private bool CheckTouchInput(out Transform touchObject, out TouchObjectType objType) {
        touchObject = default;
        objType = TouchObjectType.None;

        if (Input.touchCount == 0)
            return false;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
            return false;

        Ray ray = arCamera.ScreenPointToRay(touch.position);
        RaycastHit hitObject;

        if (Physics.Raycast(ray, out hitObject)) {
            // pass out the hitObject first
            touchObject = hitObject.transform;

            // parse the hit type
            if (hitObject.transform.GetComponent<Placeable>()) {
                objType = TouchObjectType.Placeable;
            } else if (CheckPositionOverGui(touch.position)) {
                objType = TouchObjectType.Gui;
            } else {
                objType = TouchObjectType.None;
            }
        }

        return true;
    }

    private bool CheckPositionOverGui(Vector2 touchPosition) {
        PointerEventData eventPosition = new PointerEventData(EventSystem.current);
        eventPosition.position = new Vector2(touchPosition.x, touchPosition.y);

        List<RaycastResult> hitResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventPosition, hitResults);
        return hitResults.Count > 0;
    }


    public void PlaceObject() {
        bool noCollisionDetected = gameState.PlacementValid();
        if (placementPoseValid && noCollisionDetected) {
            GameObject placeablePrefab = inventory.GetSelectedPlaceableObject();

            var dominoHeight = placeablePrefab.transform.localScale.y;
            var offset = new Vector3(0, (float)(dominoHeight / 2 + 0.0002), 0);
            GameObject placedObject = Instantiate(placeablePrefab, placementPose.position + offset, placementPose.rotation);
            gameState.AddPlacedDomino(placedObject.GetComponent<Placeable>());
            gameState.SoundSystem.PlayPlacementAudio();
        } else {
            gameState.SoundSystem.PlayPlaceErrorAudio();
        }
    }

    private void UpdatePlacementPose() {
        // check game object plane first
        UpdateByPlaceableBase();
        UpdateByArPlane();

        Vector3 cameraPosition = Camera.current.transform.position;
        float baseHitDistSqr = (placeableBaseHitPose.position - cameraPosition).sqrMagnitude;
        float arPlaneHitDistSqr = (placementPose.position - cameraPosition).sqrMagnitude;
        bool isBaseHitCloser = baseHitDistSqr < arPlaneHitDistSqr;
        if (placeableBaseHit && isBaseHitCloser) {
            placementPose = placeableBaseHitPose;
        } 
        
    }

    private void UpdateByArPlane() {
        // Transfrom input position from viewport space into screen space.
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        // Cast a ray and get the hit point.
        var hits = new List<ARRaycastHit>();
        placementPoseValid = arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        if (placementPoseValid) {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);

            float manualRotateAngle = gameState.GetPlacementAngle();
            placementPose.rotation *= Quaternion.Euler(Vector3.up * manualRotateAngle);
        }
    }

    private void UpdateByPlaceableBase() {
        // Transfrom input position from viewport space into screen space.
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        // Cast a ray and get the hit point.
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        int layerMask = LayerMask.NameToLayer("PlaceableBase");
        placeableBaseHit = Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << layerMask);
        Debug.Log("[UpdateByPlaceableBase]PlaceableBase: " + layerMask + " hit: " + placeableBaseHit);

        if (placeableBaseHit) {
            //var placeable = hit.transform.GetComponent<Placeable>();
            //if (!placeable || !placeable.isBase) {
            //    placeableBaseHit = false;
            //    return;
            //}

            placeableBaseHitPose.position = hit.point + new Vector3(0, 0.0005f, 0);

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placeableBaseHitPose.rotation = Quaternion.LookRotation(cameraBearing);

            float manualRotateAngle = gameState.GetPlacementAngle();
            placeableBaseHitPose.rotation *= Quaternion.Euler(Vector3.up * manualRotateAngle);
        }
    }


    private void UpdatePlacementIndicator() {
        // Spawn a new surface indicator if it's not initialized
        if (placementPoseValid) {
            surfaceIndicator.SetActive(true);
            surfaceIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        } else {
            surfaceIndicator.SetActive(false);
        }
    }

}