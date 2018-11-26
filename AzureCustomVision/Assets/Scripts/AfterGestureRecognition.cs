using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AfterGestureRecognition : MonoBehaviour {

    public static AfterGestureRecognition Instance;

    [SerializeField]
    private Transform cursorPrefab;

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
        //CursorManager.Instance.LoadingStart();
        CreateLabelInSpace("Sample text");
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

    private void CreateLabelInSpace(string text)
    {
        GameObject panel = new GameObject("Label");
        TextMeshPro label = panel.AddComponent<TextMeshPro>();
        //label.fontSize = 1;
        label.fontSizeMin = 1;
        label.fontSizeMax = 70;
        label.enableAutoSizing = true;
        label.rectTransform.sizeDelta = new Vector2 (5,1f);
        label.transform.localScale = new Vector3(0.1f, 0.1f, 1f);

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            label.transform.position = hitInfo.point;
            label.transform.rotation = Quaternion.LookRotation(gazeDirection);// * Quaternion.Euler(0, 90, 0);
            label.transform.Translate(Vector3.back * 0.1f, Space.World);
            label.transform.Translate(Vector3.up * 0.2f, Space.World);
            label.text = label.transform.position.ToString();
        }
    }
}
