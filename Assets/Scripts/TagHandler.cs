using System.Collections.Generic;
using UnityEngine;

public class TagHandler : MonoBehaviour
{
    public List<string> tags;

    private void Awake()
    {
        tags = new List<string>();
    }
    public bool HasTag(string tagToFind)
    {
        foreach (string tag in tags)
        {
            if (tag == tagToFind)
            {
                return true;
            }
        }
        return false;
    }

    public static GameObject FindObjectWithCustomTag(string tag)
    {

        GameObject[] objList = GameObject.FindGameObjectsWithTag("CustomTagged");
        foreach (GameObject obj in objList)
        {
            if (obj.GetComponent<TagHandler>().HasTag(tag))
            {
                return obj;
            }
        }
        return null;

    }
    public static GameObject[] FindObjectsWithCustomTag(string tag)
    {
        GameObject[] objList = GameObject.FindGameObjectsWithTag("CustomTagged");
        List<GameObject> taggedObjects = new List<GameObject>();

        foreach (GameObject obj in objList)
        {
            if (obj.GetComponent<TagHandler>().HasTag(tag))
            {
                taggedObjects.Add(obj);
            }
        }

        return taggedObjects.ToArray();
    }
    public static GameObject FindNearestObjectWithCustomTag(Transform point, string tag)
    {
        GameObject[] objList = FindObjectsWithCustomTag(tag);
        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (GameObject obj in objList)
        {
            Vector3 directionToTarget = obj.GetComponent<Transform>().position - point.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closest = obj; // https://forum.unity.com/threads/clean-est-way-to-find-nearest-object-of-many-c.44315/ ish, tweaked variable types and stuff
                               // Want to avoid using Vector3.Distance() as that uses square root math, which is resource-heavy
            }
        }
        return closest;
    }
}