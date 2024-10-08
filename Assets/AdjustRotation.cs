using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class AdjustRotation : MonoBehaviour
{
    public GameObject trackedPrefab;

    void Start()
    {
        var instance = Instantiate(trackedPrefab);
        instance.transform.rotation = Quaternion.Euler(0, 180, 0); // Adjust as needed
    }
}
