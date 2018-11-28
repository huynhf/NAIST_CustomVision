using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HoloToolkit.Unity.InputModule;
using Cursor = HoloToolkit.Unity.InputModule.Cursor;

public class CursorManager : MonoBehaviour {

    public static CursorManager Instance;

    [SerializeField]
    private Cursor cursorPrefab;

    [SerializeField]
    private Animator cursorAnim;
    private int waitingHash = Animator.StringToHash("Waiting");

    private void Awake()
    {
        Instance = this;
    }

    public void LoadingStart()
    {
        cursorAnim.SetBool(waitingHash, true);
    }

    public void LoadingStop()
    {
        cursorAnim.SetBool(waitingHash, false);
    }

    public Vector3 GetCursorPosition()
    {
        return cursorPrefab.Position;
    }
}
