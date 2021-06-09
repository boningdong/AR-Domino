using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableSelector : MonoBehaviour
{
    [SerializeField]
    private GameState gameState;
    [SerializeField]
    private PlacementInventory inventory;
    [SerializeField]
    private GameObject buttonPrefab;

    private void SelectPlaceableItem(int index) {
        Debug.Log("[SelectPlaceableItem] Index: " + index);
        inventory.SetSelectedPlaceableObject(index);
        gameState.OnPlacebleObjectSelected();
    }

    public void AddItem(int index, string name, Sprite icon) {
        var buttonObject = Instantiate(buttonPrefab, transform.Find("Viewport").Find("Content"));
        var button = buttonObject.GetComponentInChildren<Button>();
        button.transform.GetComponentInChildren<Text>().text = name;

        if (icon != null)
            button.transform.Find("Image").GetComponent<Image>().sprite = icon;

        button.onClick.AddListener(delegate {
            SelectPlaceableItem(index);
        });
    }
}
