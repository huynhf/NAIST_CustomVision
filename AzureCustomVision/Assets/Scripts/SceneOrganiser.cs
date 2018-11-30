using UnityEngine;
using TMPro;

public class SceneOrganiser : MonoBehaviour {

    /// <summary>
    /// Allows this class to behave like a singleton
    /// </summary>
    public static SceneOrganiser Instance;

    /// <summary>
    /// Reference to the label currently used to display the analysis on the objects in the real world
    /// Equal to null if analysis completed
    /// </summary>
    internal GameObject currentLabel = null;

    /// <summary>
    /// Object providing the current status of the camera.
    /// </summary>
    [SerializeField]
    internal TextMeshPro cameraStatusIndicator;

    /// <summary>
    /// Name of the recognized object /!\ Only in AppModes.Smart mode
    /// </summary>
    internal string RecognizedObject { get; set; }

    internal int recognizedObjects;

    /// <summary>
    /// Reference to the last label positioned
    /// </summary>
    public TextMeshPro lastLabelPlaced = null;

    /// <summary>
    /// Current threshold accepted for displaying the label
    /// Reduce this value to display the recognition more often
    /// </summary>
    internal float probabilityThreshold = 0.5f;

    /// <summary>
    /// Called on initialization
    /// </summary>
    private void Awake()
    {
        // Use this class instance as singleton
        Instance = this;

        // Add the ImageCapture class to this GameObject
        gameObject.AddComponent<ImageCapture>();

        // Add the CustomVisionAnalyser class to this GameObject
        gameObject.AddComponent<CustomVisionAnalyser>();

        // Add the CustomVisionTrainer class to this GameObject
        gameObject.AddComponent<CustomVisionTrainer>();

        // Add the VoiceRecogniser class to this GameObject
        gameObject.AddComponent<VoiceRecognizer>();

        // Add the CustomVisionObjects class to this GameObject
        gameObject.AddComponent<CustomVisionObjects>();

        // Create the camera status indicator label, and place it above where predictions
        // and training UI will appear.
        

        // Set camera status indicator to loading.
        SetCameraStatus("Loading");
    }

    void Start()
    {
        
    }

    void Update()
    {
        cameraStatusIndicator.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    /// <summary>
    /// Set the camera status to a provided string. Will be coloured if it matches a keyword.
    /// </summary>
    /// <param name="statusText">Input string</param>
    public void SetCameraStatus(string statusText)
    {
        if (string.IsNullOrEmpty(statusText) == false)
        {
            string message = "white";

            switch (statusText.ToLower())
            {
                case "loading":
                    message = "yellow";
                    break;

                case "ready":
                    message = "green";
                    break;

                case "uploading image":
                    message = "red";
                    break;

                case "looping capture":
                    message = "yellow";
                    break;

                case "analysis":
                    message = "red";
                    break;
            }

            cameraStatusIndicator.GetComponent<TextMeshPro>().text = $"Camera Status:\n<color={message}>{statusText}..</color>";
        }
    }

    /// <summary>
    /// Instantiate a label in the appropriate location relative to the Main Camera.
    /// </summary>
    public void PlaceAnalysisLabel()
    {
        GameObject panel = new GameObject("Label");
        TextMeshPro label = panel.AddComponent<TextMeshPro>();
        label.fontSizeMin = 1;
        label.fontSizeMax = 70;
        label.enableAutoSizing = true;
        label.rectTransform.sizeDelta = new Vector2(5, 1f);
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
            label.transform.Translate(Vector3.back * 0.1f, Space.World);
            label.transform.Translate(Vector3.up * 0.1f, Space.World);
            label.transform.rotation = Quaternion.LookRotation(gazeDirection);// * Quaternion.Euler(0, 90, 0);
            //label.text = label.transform.position.ToString(); //Uncomment only to know position while testing

            lastLabelPlaced = label;
        }
    }

    /// <summary>
    /// Set the Tags as Text of the last label created. 
    /// </summary>
    public void SetTagsToLastLabel(AnalysisObject analysisObject)
    {
        //lastLabelPlaced = lastLabelPlaced.GetComponent<TextMeshPro>();

        if (analysisObject.Predictions != null && lastLabelPlaced != null)
        {
            recognizedObjects = 0;
            
            foreach (Prediction p in analysisObject.Predictions)
            {
                if (p.Probability > 0.02)
                {
                    lastLabelPlaced.text += $"Detected: {p.TagName} {p.Probability.ToString("0.00 \n")}";
                    Debug.Log($"Detected: {p.TagName} {p.Probability.ToString("0.00 \n")}");

                    RecognizedObject = p.TagName;
                    recognizedObjects++;
                }      
            }

            if (recognizedObjects == 0)
                lastLabelPlaced.text = "Unknown";

            //Make a sound to notify the user of the new label
            AudioPlay.Instance.PlayWithVolume("Bell", 80);

            if (ImageCapture.Instance.AppMode == ImageCapture.AppModes.Smart)
            {
                if (recognizedObjects == 1)
                    ImageCapture.Instance.UploadPhotoAfterAnalysis(RecognizedObject);
                else if (recognizedObjects == 0)
                    ImageCapture.Instance.UploadPhotoAfterAnalysis(null);
                else
                    ImageCapture.Instance.UploadPhotoAfterAnalysis("More than one");
            }
            else //In AppMode.Analysis
            {
                ImageCapture.Instance.ResetImageCapture();
            }
        }
        
    }

    /// <summary>
    /// Create a 3D Text Mesh in scene, with various parameters.
    /// </summary>
    /// <param name="name">name of object</param>
    /// <param name="scale">scale of object (i.e. 0.04f)</param>
    /// <param name="yPos">height above the cursor (i.e. 0.3f</param>
    /// <param name="zPos">distance from the camera</param>
    /// <param name="setActive">whether the text mesh should be visible when it has been created</param>
    /// <returns>Returns a 3D text mesh within the scene</returns>
    //internal TextMeshPro CreateTrainingUI(string name, float scale, float yPos, float zPos, bool setActive)
    //{
    //    GameObject display = new GameObject(name, typeof(TextMesh));
    //    display.transform.parent = Camera.main.transform;
    //    display.transform.localPosition = new Vector3(0, yPos, zPos);
    //    display.SetActive(setActive);
    //    display.transform.localScale = new Vector3(scale, scale, scale);
    //    display.transform.rotation = new Quaternion();
    //    TextMeshPro textMesh = display.GetComponent<TextMesh>();
    //    textMesh.anchor = TextAnchor.MiddleCenter;
    //    textMesh.alignment = TextAlignment.Center;
    //    return textMesh;
    //}
}
