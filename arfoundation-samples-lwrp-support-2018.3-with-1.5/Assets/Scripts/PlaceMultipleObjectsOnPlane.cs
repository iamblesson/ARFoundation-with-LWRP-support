using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceMultipleObjectsOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    /// <summary>
    /// Invoked whenever an object is placed in on a plane.
    /// </summary>
    public static event Action onPlacedObject;

    ARRaycastManager m_RaycastManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = s_Hits[0].pose;
                    //Quaternion myNewRotation = AdjustRotation();
                    Quaternion rotationAmount = Quaternion.Euler(-90, 0, 0);
                    spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation * rotationAmount);
                    StartCoroutine(ChangeSpeed(1.0f, 0.0f, 2f, spawnedObject));
                    if (onPlacedObject != null)
                    {
                        onPlacedObject();
                    }
                }
            }   
        }
    }
    Quaternion AdjustRotation()
    {
        Quaternion rotationAmount = Quaternion.Euler(-90, 0, 0);
        Quaternion postRotation = spawnedObject.transform.rotation * rotationAmount;
        return postRotation;
    }
    float speed;
    IEnumerator ChangeSpeed(float v_start, float v_end, float duration, GameObject myObj)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            speed = Mathf.Lerp(v_start, v_end, elapsed / duration);
            myObj.GetComponent<Renderer>().material.SetFloat("Vector1_D902D054", speed);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
