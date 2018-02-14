using System;
using UnityEngine;

public enum SwipeDirection
{
    None,
    Up,
    Down,
    Right,
    Left
};

public struct SwipeParameter
{
    public Vector2 startPosition;
    public Vector2 endPosition;
    public SwipeDirection direction;
}

public class SwipeScript : MonoBehaviour
{
    public static event Action<SwipeParameter> OnSwiped;
    public float minSwipeDistY;
    public float minSwipeDistX;

    private SwipeParameter swipeParameter;

    public void Clear()
    {
        swipeParameter = new SwipeParameter();
    }

    private void Update()
    {
        HandleTouch();
        HandleMouse();
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 0) {
            return;
        }

        var touch = Input.touches[0];
        switch (touch.phase) {

            case TouchPhase.Began:
                swipeParameter.startPosition = touch.position;
                swipeParameter.direction = SwipeDirection.None;
                break;

            case TouchPhase.Ended:

                swipeParameter.endPosition = touch.position;

                var swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, swipeParameter.startPosition.y, 0)).magnitude;
                var swipeDistHorizontal = (new Vector3(touch.position.x, 0, 0) - new Vector3(swipeParameter.startPosition.x, 0, 0)).magnitude;

                if ((swipeDistVertical > swipeDistHorizontal) &&
                    (swipeDistVertical > minSwipeDistY)) {

                    var swipeValue = Mathf.Sign(touch.position.y - swipeParameter.startPosition.y);
                    if (swipeValue > 0) {
                        swipeParameter.direction = SwipeDirection.Up;
                    }
                    else if (swipeValue < 0) {
                        swipeParameter.direction = SwipeDirection.Down;
                    }

                    if (OnSwiped != null) {
                        OnSwiped(swipeParameter);
                    }
                    return;
                }

                if ((swipeDistHorizontal > swipeDistVertical) &&
                    (swipeDistHorizontal > minSwipeDistX)) {

                    var swipeValue = Mathf.Sign(touch.position.x - swipeParameter.startPosition.x);
                    if (swipeValue > 0) {
                        swipeParameter.direction = SwipeDirection.Right;
                    }
                    else if (swipeValue < 0) {
                        swipeParameter.direction = SwipeDirection.Left;
                    }

                    if (OnSwiped != null) {
                        OnSwiped(swipeParameter);
                    }
                    return;
                }

                Clear();
                break;
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0)) {

            swipeParameter.startPosition = Input.mousePosition;
            swipeParameter.direction = SwipeDirection.None;
            return;
        }

        if (Input.GetMouseButtonUp(0)) {

            swipeParameter.endPosition = Input.mousePosition;
            var swipeDistVertical = (new Vector3(0, Input.mousePosition.y, 0) - new Vector3(0, swipeParameter.startPosition.y, 0)).magnitude;
            var swipeDistHorizontal = (new Vector3(Input.mousePosition.x, 0, 0) - new Vector3(swipeParameter.startPosition.x, 0, 0)).magnitude;

            if ((swipeDistVertical > swipeDistHorizontal) &&
                (swipeDistVertical > minSwipeDistY)) {

                var swipeValue = Mathf.Sign(Input.mousePosition.y - swipeParameter.startPosition.y);
                if (swipeValue > 0) {
                    swipeParameter.direction = SwipeDirection.Up;
                }
                else if (swipeValue < 0) {
                    swipeParameter.direction = SwipeDirection.Down;
                }

                if (OnSwiped != null) {
                    OnSwiped(swipeParameter);
                }
                return;
            }

            if ((swipeDistHorizontal > swipeDistVertical) &&
                (swipeDistHorizontal > minSwipeDistX)) {

                var swipeValue = Mathf.Sign(Input.mousePosition.x - swipeParameter.startPosition.x);
                if (swipeValue > 0) {
                    swipeParameter.direction = SwipeDirection.Right;
                }
                else if (swipeValue < 0) {
                    swipeParameter.direction = SwipeDirection.Left;
                }

                if (OnSwiped != null) {
                    OnSwiped(swipeParameter);
                }
                return;
            }

            Clear();
        }
    }
}
