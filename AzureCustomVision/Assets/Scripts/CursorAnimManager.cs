using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAnimManager : MonoBehaviour {

    public static CursorAnimManager Instance;

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
}
