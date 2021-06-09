using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntentoryItem {
    public string objectName;
    public Sprite icon;
    public GameObject PlacebleObjPrefab;
}

public class PlacementInventory : MonoBehaviour
{
    [SerializeField]
    public List<IntentoryItem> inventoryItems;

    private int selectedIndex = 0;

    public GameObject GetSelectedPlaceableObject() {
        return inventoryItems[selectedIndex].PlacebleObjPrefab;
    }

    public void SetSelectedPlaceableObject(int index) {
        if (index >= 0 && index < inventoryItems.Count)
            selectedIndex = index;
    }
}
