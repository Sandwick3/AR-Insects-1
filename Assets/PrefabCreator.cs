using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PrefabCreator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 prefabOffset;

    private GameObject hand;
    private ARTrackedImageManager aRTrackedImageManager;

    private void OnEnable()
    {
        aRTrackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs obj)
    {
        foreach (ARTrackedImage image in obj.added)
        {
            if (hand == null) // Only instantiate if 'hand' does not already exist
            {
                hand = Instantiate(prefab, image.transform);
                hand.transform.position += prefabOffset;
            }
        }
    }
}
