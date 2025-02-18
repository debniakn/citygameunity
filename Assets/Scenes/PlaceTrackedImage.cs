using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImage : MonoBehaviour
{
    private ARTrackedImageManager _trackedImagesManager;

    [SerializeField]
    private GameObject[] ArPrefabs;

    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _trackedImagesManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        _trackedImagesManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }


    void OnDisable()
    {
        _trackedImagesManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        Debug.Log("Event started");
        foreach (var trackedImage in eventArgs.added)
        {
            Debug.Log("Detected");
            var imageName = trackedImage.referenceImage.name;

            foreach (var prefab in ArPrefabs)
            {
                if (string.Compare(prefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0 && !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    var newPrefab = Instantiate(prefab, trackedImage.transform);
                    _instantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            Debug.Log("updated");
            _instantiatedPrefabs[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            Debug.Log("removed");
            Destroy(_instantiatedPrefabs[trackedImage.Value.referenceImage.name]);
            _instantiatedPrefabs.Remove(trackedImage.Value.referenceImage.name);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}