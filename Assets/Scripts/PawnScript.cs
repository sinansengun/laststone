using UnityEngine;
using System.Collections;
using System;

public class PawnScript : MonoBehaviour 
{
    public static event Action<Transform> OnPawnSelected;
    public static event Action<Transform, SwipeParameter> OnPawnSwiped;
    public static Transform selectedPawn;

    public int pointX;
    public int pointY;

    static PawnScript()
    {
        SwipeScript.OnSwiped += SwipeScript_OnSwiped;
    }

    public static void Clear()
    {
        selectedPawn = null;
    }

    private static void SwipeScript_OnSwiped(SwipeParameter swipeParameter)
    {
        if (selectedPawn != null &&
            OnPawnSwiped != null) {
            OnPawnSwiped(selectedPawn, swipeParameter);
            Clear();
        }
    }

    private void OnMouseDown()
    {
        selectedPawn = transform;
        if (OnPawnSelected != null) {
            OnPawnSelected(selectedPawn);
        }
    }
}
