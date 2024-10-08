using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;
    public GameObject[] ArPrefabs;
    public float prefabScale = 0.1f;
    private Dictionary<string, GameObject> ARObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, Coroutine> deactivateCoroutines = new Dictionary<string, Coroutine>();

    void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // For newly detected images
        foreach (var trackedImage in eventArgs.added)
        {
            if (!ARObjects.ContainsKey(trackedImage.referenceImage.name))
            {
                foreach (var arPrefab in ArPrefabs)
                {
                    if (trackedImage.referenceImage.name == arPrefab.name)
                    {
                        // Instantiate prefab at image's position and rotation
                        var newPrefab = Instantiate(arPrefab, trackedImage.transform);
                        newPrefab.name = arPrefab.name;
                        newPrefab.transform.localScale = Vector3.one * prefabScale; // Apply scale
                        ARObjects[arPrefab.name] = newPrefab;
                    }
                }
            }
        }

        // For images that are updated or tracking state changes
        foreach (var trackedImage in eventArgs.updated)
        {
            if (ARObjects.TryGetValue(trackedImage.referenceImage.name, out var existingObject))
            {
                existingObject.SetActive(trackedImage.trackingState == TrackingState.Tracking);

                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    // Attach prefab back to image location when tracking is regained
                    existingObject.transform.position = trackedImage.transform.position;
                    existingObject.transform.rotation = trackedImage.transform.rotation * Quaternion.Euler(0, 180, 0);

                    if (deactivateCoroutines.ContainsKey(trackedImage.referenceImage.name))
                    {
                        StopCoroutine(deactivateCoroutines[trackedImage.referenceImage.name]);
                        deactivateCoroutines.Remove(trackedImage.referenceImage.name);
                    }
                }
                else if (!deactivateCoroutines.ContainsKey(trackedImage.referenceImage.name))
                {
                    // Start delayed deactivation coroutine
                    deactivateCoroutines[trackedImage.referenceImage.name] = StartCoroutine(DeactivateAfterDelay(existingObject, trackedImage.referenceImage.name, 10f));
                }
            }
        }
    }

    private IEnumerator DeactivateAfterDelay(GameObject obj, string imageName, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        deactivateCoroutines.Remove(imageName);
    }
}
