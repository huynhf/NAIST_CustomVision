using System.Collections;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;
using System;

public class AfterGestureRecognition : MonoBehaviour {

    public static AfterGestureRecognition Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        
	}

    /// <summary>
    /// Respond to Tap Input.
    /// </summary>
    public void ExtraTapped()
    {
        //Debug.Log($"Extra Tapped called");

        //Vector3 cursorPosition = CursorManager.Instance.GetCursorPositionOnMesh();
        //Debug.Log($"Cursor position = {cursorPosition}");
        //DrawInSpace.Instance.DrawStaticRectangle(CursorManager.Instance.GetCursorPositionOnMesh(), 0.1f, 0.1f);
    }

    public void DoubleTapped() //not working in unity but works with Hololens
    {
        Debug.Log($"Double Tapped called");
        //DialogManager.Instance.LaunchBasicDialog(1, "Debug", "Double Tapped called");

        RaycastHit hit;
        Ray landingRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        int layer = LayerMask.NameToLayer("Holograms"); //SpatialMappingManager.Instance.LayerMask;
        int layerMask = (1 << layer);
        // This would cast rays only against colliders in SpatialMapping layer.
        // But instead we want to collide against everything except SpatialMapping layer. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        if (Physics.Raycast(landingRay, out hit, Mathf.Infinity, layerMask))
        {
            //DialogManager.Instance.LaunchBasicDialog(1, "Tag", $"{hit.collider.tag} and layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            if (hit.collider.tag == "BoundingBox")
            {
                //SpatialMappingManager.Instance.DrawVisualMeshes = true;
                hit.collider.gameObject.transform.position = CursorManager.Instance.GetCursorPositionOnMesh();
            }
        }
    }

    public void ExtraHoldStarted()
    {
        //StartCoroutine(Wait(2f));
        //AudioPlay.Instance.Play("Bell");
        ImageCapture.Instance.ExecuteImageCaptureAndAnalysis();
    }

    public void ExtraHoldCompleted()
    {
        //cursorAnim.SetBool(waitingHash, false);
        
    }

    public void ExtraHoldCanceled()
    {
        
    }

    private IEnumerator Wait(float sec)
    {
        yield return new WaitForSeconds(sec);
    }
}
