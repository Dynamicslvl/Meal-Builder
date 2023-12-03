using UnityEngine;
using UnityEngine.EventSystems;
using Dasis.DesignPattern;
using System;

namespace Dasis.Input
{
    public class InputReceiver : Singleton<InputReceiver>
    {
        [SerializeField]
        private Camera _camera;

        private bool isWordspaceMouseHolding = false;
        private bool isCanvasMouseHolding = false;
        private Vector2 mousePosition;

        public Vector2 MousePostion => mousePosition;
        public Camera Camera => _camera;

        public static Action<Vector2> OnWorldspaceMouseDown;
        public static Action<Vector2> OnWorldspaceMouseUp;
        public static Action<Vector2> OnWorldspaceMouseHold;

        public static Action<Vector2> OnCanvasMouseDown;
        public static Action<Vector2> OnCanvasMouseUp;
        public static Action<Vector2> OnCanvasMouseHold;

        private void TrackMousePosition()
        {
            mousePosition = _camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        }

        public void WorldspaceMouseDownEvent()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null)
            {
                if (!isWordspaceMouseHolding)
                {
                    isWordspaceMouseHolding = true;
                    OnWorldspaceMouseDown?.Invoke(mousePosition);
                }
            }
        }

        public void WorldspaceMouseHoldEvent()
        {
            if (isWordspaceMouseHolding)
            {
                OnWorldspaceMouseHold?.Invoke(mousePosition);
            }
        }

        public void WorldspaceMouseUpEvent()
        {
            if ((UnityEngine.Input.touchCount < 2 && UnityEngine.Input.GetMouseButtonUp(0))
            || (UnityEngine.Input.touchCount >= 2 && UnityEngine.Input.touches[0].phase == TouchPhase.Ended))
            {
                if (isWordspaceMouseHolding)
                {
                    isWordspaceMouseHolding = false;
                    OnWorldspaceMouseUp?.Invoke(mousePosition);
                }
            }
        }

        public void CanvasMouseDownEvent()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (!isCanvasMouseHolding)
                {
                    isCanvasMouseHolding = true;
                    OnCanvasMouseDown?.Invoke(mousePosition);
                }
            }
        }

        public void CanvasMouseHoldEvent()
        {
            if (isCanvasMouseHolding)
            {
                OnCanvasMouseHold?.Invoke(mousePosition);
            }
        }

        public void CanvasMouseUpEvent()
        {
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (isCanvasMouseHolding)
                {
                    isCanvasMouseHolding = false;
                    OnCanvasMouseUp?.Invoke(mousePosition);
                }
            }
        }

        private void Update()
        {
            TrackMousePosition();
            //
            WorldspaceMouseUpEvent();
            WorldspaceMouseHoldEvent();
            WorldspaceMouseDownEvent();
            //
            CanvasMouseUpEvent();
            CanvasMouseHoldEvent();
            CanvasMouseDownEvent();
        }
    }

    public interface IWorldspaceInputable
    {
        public void OnWorldspaceMouseDown(Vector2 mousePos);
        public void OnWorldspaceMouseHold(Vector2 mousePos);
        public void OnWorldspaceMouseUp(Vector2 mousePos);
    }

    public interface ICanvasInputable
    {
        public void OnCanvasMouseDown(Vector2 mousePos);
        public void OnCanvasMouseHold(Vector2 mousePos);
        public void OnCanvasMouseUp(Vector2 mousePos);
    }
}
