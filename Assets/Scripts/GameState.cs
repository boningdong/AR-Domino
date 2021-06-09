using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PlaceableRecord {
    public Vector3 initPosition;
    public Quaternion initRotation;
    public Placeable dominoObject;
}

[RequireComponent(typeof(PlacementInventory))]
public class GameState : MonoBehaviour {

    [SerializeField]
    private SoundFXSystem soundSystem;
    public SoundFXSystem SoundSystem { get { return soundSystem; } }

    [SerializeField]
    private GameObject FloatMenu;

    [SerializeField]
    private GameObject gameMenu;

    [SerializeField]
    private GameObject surfaceIndicator;

    [SerializeField]
    private PlaceableSelector placeableSelector;

    private PlacementInventory inventory;
    private Placeable selectedPlaceable = default;
    private Dictionary<Placeable, PlaceableRecord> placedObjects = new Dictionary<Placeable, PlaceableRecord>();

    void Awake() {
        inventory = transform.GetComponent<PlacementInventory>();
        ClearSelection();
        OnPlacebleObjectSelected();
        InitializePlaceableSelector();
    }

    private void InitializePlaceableSelector() {
        Debug.Log("InitiaslizePlaceableSelector] Count: " + inventory.inventoryItems.Count);
        var total = inventory.inventoryItems.Count;
        for (int i = 0; i < total; i++) {
            var item = inventory.inventoryItems[i];
            placeableSelector.AddItem(i, item.objectName, item.icon);
        }
    }

    public void OnPlacebleObjectSelected() {

        GameObject placeablePrefab = inventory.GetSelectedPlaceableObject();
        UpdateSurfaceIndicator(placeablePrefab);
    }

    private void UpdateSurfaceIndicator(GameObject placeablePrefab) {
        // destroy the current one if there is one attached
        var currentIndicator = surfaceIndicator.transform.Find("Indicator");
        if (currentIndicator != null)
            Destroy(currentIndicator.gameObject);

        // attach the new one
        var newIndicator = Instantiate(placeablePrefab, surfaceIndicator.transform);

        // change layer so that raycast will not hit it.
        newIndicator.gameObject.layer = 0;

        // attach the indicator script and material
        newIndicator.AddComponent<PlacementIndicator>();

        // Correct parameters
        var height = newIndicator.transform.localScale.y;
        var offset = new Vector3(0, (float)(height / 2 + 0.0005), 0);
        newIndicator.transform.position += offset;
        newIndicator.name = "Indicator";
    }

    public void RefreshDominoLocations() {
        ClearSelection();
        foreach (var record in placedObjects.Values) {
            var t = record.dominoObject.transform;
            t.position = record.initPosition;
            t.rotation = record.initRotation;
        }
    }

    public void RestartGame() {
        ClearSelection();
        // clear all dominos
        foreach (var record in placedObjects.Values) {
            Destroy(record.dominoObject.gameObject);
        }
        placedObjects = new Dictionary<Placeable, PlaceableRecord>();
        ResetPlacementValid();
    }

    public void ResetPlacementValid() {
        var indicator = surfaceIndicator.GetComponentInChildren<PlacementIndicator>();
        indicator.PlacementValid = true;
    }

    public void AddPlacedDomino(Placeable placeable) {
        PlaceableRecord record = new PlaceableRecord();
        record.dominoObject = placeable;
        record.initPosition = placeable.transform.position;
        record.initRotation = placeable.transform.rotation;
        placedObjects.Add(placeable, record);
    }

    public float GetPlacementAngle() {
        Transform angleSlider = gameMenu.transform.Find("AngleSlider");
        var slider = angleSlider.GetComponent<CurvedSlider>();
        float angle = slider.SliderPercent * 180 - 90;
        return -angle;
    }

    public bool PlacementValid() {
        var indicator = surfaceIndicator.GetComponentInChildren<PlacementIndicator>();
        return indicator.PlacementValid;
    }

    public void ClearSelection() {
        if (selectedPlaceable) {
            selectedPlaceable.Selected = false;
            selectedPlaceable = null;
        }
        SetDominoFloatMenuVisibility(false);
    }

    public void RemoveSelectedDomino() {
        if (selectedPlaceable == null)
            return;

        Debug.Log("[RemoveSelectedDomino]Removing: " + selectedPlaceable.gameObject);
        placedObjects.Remove(selectedPlaceable);
        Destroy(selectedPlaceable.gameObject);
        ClearSelection();
    }

    public void SelectPlaceable(Placeable placeable) {
        if (placeable == null)
            return;
        ClearSelection();
        selectedPlaceable = placeable;
        selectedPlaceable.Selected = true;

        UpdateDominoFloatMenuLocation(selectedPlaceable);
        SetDominoFloatMenuVisibility(true);
    }

    private void UpdateDominoFloatMenuLocation(Placeable domino) {
        PlaceableRecord record = placedObjects[domino];
        FloatMenu.transform.position = domino.transform.position + new Vector3(0, 0.065f, 0);
        FloatMenu.transform.rotation = record.initRotation;
    }

    private void SetDominoFloatMenuVisibility(bool visible) {
        FloatMenu.gameObject.SetActive(visible);
    }
}
