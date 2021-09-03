using UnityEngine;

[System.Serializable]
public class Background
{
    // Holds background values for BackgroundScript
    public string name;
    public Sprite sprite;
    public int sortingOrder;
    public Vector3 offset;
    public bool infiniteX;
    public bool infiniteY;
    public Vector2 parralaxMultiplier;
}
