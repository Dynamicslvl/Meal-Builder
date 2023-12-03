using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

namespace CrazyArrow
{
    [RequireComponent(typeof(Button))]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        [SerializeField]
        private Sprite onSprite, offSprite;

        [SerializeField]
        private bool enable;

        [SerializeField]
        private UnityEvent onToggle;

        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;
                UpdateDisplay();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateDisplay();
        }
#endif

        public void OnClicked()
        {
            Enable = !Enable;
            onToggle?.Invoke();
        }

        public void UpdateDisplay()
        {
            if (enable)
            {
                icon.sprite = onSprite;
                return;
            }
            icon.sprite = offSprite;
        }
    }
}
