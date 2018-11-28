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
        //DialogManager.Instance.launchDialog(1);
    }

    public void ExtraHoldStarted()
    {
        //StartCoroutine(Wait(2f));
        //DialogManager.Instance.launchDialog(1);
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
