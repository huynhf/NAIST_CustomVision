﻿#define OBJDETECT

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity.SpatialMapping;
using System;

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

    [SerializeField]
    internal SpatialMappingManager SpatialMapping;

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
    internal float probabilityThreshold = 0.3f;

#if OBJDETECT
    
    /// <summary>
    /// The quad object hosting the imposed image captured
    /// </summary>
    private GameObject quad;

    /// <summary>
    /// Renderer of the quad object
    /// </summary>
    internal Renderer quadRenderer;

#endif

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

        // Create the camera status indicator label, and place it above where Predictions
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
            string textColor = "white";

            switch (statusText.ToLower())
            {
                case "loading":
                    textColor = "yellow";
                    break;

                case "ready":
                    textColor = "green";
                    break;

                case "uploading image":
                    textColor = "red";
                    break;

                case "looping capture":
                    textColor = "yellow";
                    break;

                case "analysis":
                    textColor = "red";
                    break;
            }

            cameraStatusIndicator.GetComponent<TextMeshPro>().text = $"Camera Status:\n<color={textColor}>{statusText}..</color>";
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
#if OBJDETECT
            label.text = hitInfo.distance.ToString(); // label.transform.position.ToString(); //Uncomment only to know position while testing
#endif
            lastLabelPlaced = label;
        }

#if OBJDETECT
        // Create a GameObject to which the texture can be applied
        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        Material m = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse")); 
        quadRenderer.material = m;

        // Here you can set the transparency of the quad. Useful for debugging
        // Allows you to see the picture taken in the real world
        float transparency = 0.0f;
        quadRenderer.material.color = new Color(1, 1, 1, transparency);

        //Set the position and scale of the quad depending on user position
        quad.transform.SetParent(transform);
        //quad.transform.rotation = transform.rotation;

        //Set the quad (screen for picture) at the label position
        quad.transform.position = CursorManager.Instance.GetCursorPositionOnMesh();
        quad.transform.rotation = Quaternion.LookRotation(gazeDirection);

        //The quad is positioned slightly forward in font of the user
        //quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        // The quad scale as been set with the following value following experimentation,  
        // to allow the image on the quad to be as precisely imposed to the real world as possible
        quad.transform.localScale = new Vector3(hitInfo.distance, 1.65f / 3f * hitInfo.distance, 1f); //3f/2.5f and 1.65f/2.5f*hitInfo.distance

        //BoundingBoxManager.Instance.MakeBoundingBoxInteractible(quad, HoloToolkit.Unity.UX.BoundingBox.FlattenModeEnum.FlattenAuto);

        quad.transform.parent = null;
#endif
        
    }

    /// <summary>
    /// Set the Tags as Text of the last label created. 
    /// </summary>
    public void FinaliseLabel(AnalysisObject analysisObject)
    {
#if OBJDETECT
        if (analysisObject.Predictions != null)
        {
            // Sort the Predictions to locate the highest one
            List<Prediction> sortedPredictions = new List<Prediction>();
            sortedPredictions = analysisObject.Predictions.OrderBy(p => p.Probability).ToList();
            Prediction bestPrediction = new Prediction();
            bestPrediction = sortedPredictions[sortedPredictions.Count - 1];

            if (bestPrediction.Probability > probabilityThreshold)
            {
                quadRenderer = quad.GetComponent<Renderer>() as Renderer;
                Bounds quadBounds = quadRenderer.bounds;

                //Draw a visible boundingBox of the recognized object
                GameObject objBoundingBox = DrawInSpace.Instance.DrawCube(new Vector3(0,0,0),
                    (float)bestPrediction.BoundingBox.Width, (float)bestPrediction.BoundingBox.Height);
                objBoundingBox.transform.parent = quad.transform;
                objBoundingBox.transform.localPosition = CalculateBoundingBoxPosition(quadBounds, bestPrediction.BoundingBox);
                //DrawInSpace.Instance.ChooseMaterial(objBoundingBox, "BoundingBoxTransparent"); //optional
                //Set the position and scale of the quad depending on user position
                objBoundingBox.transform.SetParent(transform); //break the link with quad (picture)
                BoundingBoxManager.Instance.MakeBoundingBoxInteractible(objBoundingBox, HoloToolkit.Unity.UX.BoundingBox.FlattenModeEnum.FlattenAuto);
                objBoundingBox.AddComponent<EventTriggerDelegate>(); //to block picture functions while moving bounding box

                lastLabelPlaced.transform.position = objBoundingBox.transform.position;
                //lastLabelPlaced.transform.parent = objBoundingBox.transform; //Link the Text label to the object bounding box
                // Move the label upward in world space just above the boundingBox.
                //lastLabelPlaced.transform.Translate(Vector3.up * (float)bestPrediction.BoundingBox.Height/2, Space.World);

                // Set the tag text
                lastLabelPlaced.text = bestPrediction.TagName;

                //Cast a ray from the user's head to the currently placed label, it should hit the object detected by the Service.
                // At that point it will reposition the label where the ray HL sensor collides with the object,
                // (using the HL spatial tracking)
                Debug.Log("Repositioning Label");
                Vector3 headPosition = Camera.main.transform.position;
                RaycastHit objHitInfo;
                Vector3 objDirection = lastLabelPlaced.transform.position;
                if (Physics.Raycast(headPosition, objDirection, out objHitInfo, 30.0f, SpatialMapping.LayerMask))
                {
                    lastLabelPlaced.transform.position = objHitInfo.point;
                }

                //RecognizedObject = bestPrediction.TagName;

                if (ImageCapture.Instance.AppMode == ImageCapture.AppModes.Smart)
                {
                    ImageCapture.Instance.UploadPhotoAfterAnalysis(RecognizedObject);
                }
            }
            else
            {
                lastLabelPlaced.text = "Unknown";
            }
        }

        //Make a sound to notify the user of the new label
        AudioPlay.Instance.PlayWithVolume("Bell", 80);

        ImageCapture.Instance.ResetImageCapture();

#else

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
#endif

    }

    private void CreateCube(Vector3 position)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        //Create a material with transparent diffuse shader
        Material material = new Material(Shader.Find("Transparent/Diffuse"));
        material.color = Color.gray;
        // assign the material to the renderer
        cube.GetComponent<Renderer>().material = material;
    }

#if OBJDETECT

    /// <summary>
    /// This method hosts a series of calculations to determine the position 
    /// of the Bounding Box on the quad created in the real world
    /// by using the Bounding Box received back alongside the Best Prediction
    /// </summary>
    public Vector3 CalculateBoundingBoxPosition(Bounds b, BoundingBox2D boundingBox)
    {
        Debug.Log($"BB: left {boundingBox.Left}, top {boundingBox.Top}, width {boundingBox.Width}, height {boundingBox.Height}");

        double centerFromLeft = boundingBox.Left + (boundingBox.Width / 2);
        double centerFromTop = boundingBox.Top + (boundingBox.Height / 2);
        Debug.Log($"BB CenterFromLeft {centerFromLeft}, CenterFromTop {centerFromTop}");

        double quadWidth = b.size.normalized.x;
        double quadHeight = b.size.normalized.y;
        Debug.Log($"Quad Width {b.size.normalized.x}, Quad Height {b.size.normalized.y}");

        double normalisedPos_X = (quadWidth * centerFromLeft) - (quadWidth / 2);
        double normalisedPos_Y = (quadHeight * centerFromTop) - (quadHeight / 2);

        return new Vector3((float)normalisedPos_X, (float)normalisedPos_Y, 0);
    }

    //public DrawStaticRectangle()
    //{
    //    Graphics G = new Graphics();
    //    System.Drawing.Pen pen = new Pen(Color.red, 2);
    //}

#endif
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
