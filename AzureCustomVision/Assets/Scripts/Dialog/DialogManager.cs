﻿using HoloToolkit.Unity.Buttons;
using HoloToolkit.UX.Dialog;
using System.Collections;
using UnityEngine;

public class DialogManager : MonoBehaviour {

    public static DialogManager Instance;

    [SerializeField]
    private Dialog dialogPrefab = null;

    [SerializeField]
    private bool isDialogLaunched;

    [SerializeField]
    public GameObject resultText;
    /// <summary>
    /// Used to report the dialogResult. OK, Cancel etc.
    /// The button that was clicked to respond to the Dialog.
    /// </summary>
    public GameObject ResultText
    {
        get
        {
            return resultText;
        }

        set
        {
            resultText = value;
        }
    }

    [SerializeField]
    [Range(0, 2)]
    private int numButtons = 1;

    private TextMesh resultTextMesh;

    // Dialog button that will be clicked by user
    private Button button;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// This function is called to set the settings for the dialog and then open it.
    /// </summary>
    /// <param name="buttons">Enum describing the number of buttons that will be created on the Dialog</param>
    /// <param name="title">This string will appear at the top of the Dialog</param>
    /// <param name="message">This string will appear in the body of the Dialog</param>
    /// <returns>IEnumerator used for Coroutine funtions in Unity</returns>
    public IEnumerator LaunchDialog(DialogButtonType buttons, string title, string message)
    {
        isDialogLaunched = true;

        //Open Dialog by sending in prefab
        Dialog dialog = Dialog.Open(dialogPrefab.gameObject, buttons, title, message);

        if (dialog != null)
        {
            //listen for OnClosed Event
            dialog.OnClosed += OnClosed;
        }

        // Wait for dialog to close
        while (dialog.State < DialogState.InputReceived)
        {
            yield return null;
        }

        //only let one dialog be created at a time
        isDialogLaunched = false;

        yield break;
    }

    private void OnEnable()
    {
        resultTextMesh = ResultText.GetComponent<TextMesh>();
        //resultTextMesh = new TextMesh();
        button = GetComponent<Button>();
        if (button != null)
        {
            button.OnButtonClicked += OnButtonClicked;
        }
    }

    public void LaunchBasicDialog(int nbButtons, string title, string message)
    {
        if (nbButtons != 0)
            numButtons = nbButtons;

        if (isDialogLaunched == false)
        {
            if (numButtons == 1)
            {
                // Launch Dialog with single button
                StartCoroutine(LaunchDialog(DialogButtonType.OK, title, message));
            }
            else if (numButtons == 2)
            {
                // Launch Dialog with two buttons
                StartCoroutine(LaunchDialog(DialogButtonType.Yes | DialogButtonType.No, title, message));
            }
        }
    }

    private void OnButtonClicked(GameObject obj)
    {
        //Do nothing
    }

    /// <summary>
    /// Event Handler that fires when Dialog is closed- when a button on the Dialog is clicked.
    /// </summary>
    /// <param name="result">Returns a description of the result, which button was clicked</param>
    protected void OnClosed(DialogResult result)
    {
        // Get the result text from the Dialog
        resultTextMesh.text = result.Result.ToString();
        //CursorManager.Instance.LoadingStop();
    }
}
