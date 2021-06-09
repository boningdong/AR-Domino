using UnityEngine;

public class Placeable : MonoBehaviour
{

    public bool isBase = false;
    [SerializeField]
    private Color normalColor = new Color32(255, 255, 255, 200);
    [SerializeField]
    private Color normalOutlineColor = new Color32(0, 0, 0, 200);
    [SerializeField]
    private Color selectedColor = new Color32(255, 222, 121, 200);
    [SerializeField]
    private Color selectedOutlineColor = new Color32(255, 194, 0, 255);

    private bool isSelect;
    private AudioSource audioSource;

    private void Awake() {
        Selected = false;
        audioSource = GetComponent<AudioSource>();
        var collider = GetComponent<Collider>();
        if (collider)
            collider.contactOffset = 0.0005f;
    }

    public bool Selected 
    { 
        get 
        {
            return this.isSelect;
        }
        set 
        {
            isSelect = value;
            UpdateDominoSelectedColor();
        }
    }

    public void UpdateDominoSelectedColor() {
        var renderer = transform.GetComponent<Renderer>();
        if (isSelect) {
            renderer.material.SetVector("_OtlColor", selectedOutlineColor);
            renderer.material.SetVector("_Color", selectedColor);
        } else {
            renderer.material.SetVector("_OtlColor", normalOutlineColor);
            renderer.material.SetVector("_Color", normalColor);
        }
    }
}