using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
* This file should be attached to the domino model on the surface indicator.
*/
public class PlacementIndicator : MonoBehaviour
{
    public const string indicatorMaterialPath = "Material/PlaceableIndicator.mat";

    public Color placementValidColor = new Color32(140, 220, 190, 120);
    public Color placementInvalidColor = new Color32(220, 140, 140, 120);

    private bool isPlacementValid = true;

    public bool PlacementValid {
        get {
            return isPlacementValid;
        }

        set {
            isPlacementValid = value;
            UpdateDominoIndicatorColor();
        }
    }

    void Awake() {
        InitializeIndicator();
    }


    private void InitializeIndicator() {
        // attach material when initialized
        Material indicatorMaterial = Resources.Load(indicatorMaterialPath, typeof(Material)) as Material;
        GetComponent<Renderer>().material = indicatorMaterial;
        UpdateDominoIndicatorColor();

        // remove physics
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody != null) {
            rigidBody.useGravity = false; // needed because rigidbody will not be destroyed right away.
            Destroy(rigidBody);
        }

        // remove placeable component
        var placeableComponent = GetComponent<Placeable>();
        Destroy(placeableComponent);

        // config collision
        var collider = GetComponent<Collider>();
        if (collider != null)
            collider.isTrigger = true;


        var contactSoundSource = GetComponent<ContactSoundSource>();
        if (contactSoundSource != null)
            Destroy(contactSoundSource);
    }

    private void UpdateDominoIndicatorColor() {
        var renderer = transform.GetComponent<Renderer>();

        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.DisableKeyword("_ALPHATEST_ON");
        renderer.material.DisableKeyword("_ALPHABLEND_ON");
        renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer.material.renderQueue = 3000;

        if (isPlacementValid) {
            renderer.material.SetColor("_Color", placementValidColor);
        } else {
            renderer.material.SetColor("_Color", placementInvalidColor);
        }
    }

    private void OnTriggerEnter(Collider other) {
        // TODO: use a list to check true or false
        PlacementValid = false;
    }

    private void OnTriggerExit(Collider other) {
        PlacementValid = true;
    }

}
