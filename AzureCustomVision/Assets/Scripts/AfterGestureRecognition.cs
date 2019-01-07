using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        //Vector3 cursorPosition = CursorManager.Instance.GetCursorPositionOnMesh();
        //Debug.Log($"Cursor position = {cursorPosition}");
        //DrawInSpace.Instance.DrawStaticRectangle(CursorManager.Instance.GetCursorPositionOnMesh(), 0.1f, 0.1f);
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
