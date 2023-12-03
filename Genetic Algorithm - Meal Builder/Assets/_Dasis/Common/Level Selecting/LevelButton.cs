
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Dasis.DesignPattern;
using System;

namespace Dasis.Common
{
    public class LevelButton : MonoBehaviour, IPoolable<LevelButton>
    {
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private Image background;
        [SerializeField] private Image lockIcon;
        [SerializeField] Button self;
        [SerializeField] private List<Sprite> backgroundSprites;

        private LevelPage levelPage;
        private LevelState levelState;
        public LevelState State => levelState;

        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public LevelPage LevelPage
        {
            get { return levelPage; }
            set 
            { 
                levelPage = value;
                self.onClick.RemoveAllListeners();
                self.onClick.AddListener(() =>
                {
                    levelPage.OnLevelButtonClick(this);
                });
            }
        }

        public int LevelID => int.Parse(numberText.text);

        public TextMeshProUGUI NumberText => numberText;

        public Action<LevelButton> ReturnAction { get; set; }

        public void SetState(LevelState state)
        {
            levelState = state;
            background.sprite = backgroundSprites[(int)levelState];
            switch (state)
            {
                case LevelState.Completed:
                    numberText.color = TextColor.completed;
                    numberText.fontMaterial.SetColor("_GlowColor", TextColor.completed);
                    lockIcon.gameObject.SetActive(false);
                    break;
                case LevelState.CurrentPlay:
                    numberText.color = TextColor.current;
                    numberText.fontMaterial.SetColor("_GlowColor", TextColor.current);
                    lockIcon.gameObject.SetActive(false);
                    break;
                case LevelState.Locked:
                    numberText.color = Color.clear;
                    numberText.fontMaterial.SetColor("_GlowColor", Color.clear);
                    lockIcon.gameObject.SetActive(true);
                    break;
            }
        }

        public void Initialize(Action<LevelButton> returnAction)
        {
            ReturnAction = returnAction;
        }

        public void ReturnToPool()
        {
            ReturnAction?.Invoke(this);
        }
    }

    public enum LevelState
    {
        Completed = 0,
        CurrentPlay = 1,
        Locked = 2,
    }

    public static class TextColor
    {
        public static Color completed = new Color32(255, 214, 0, 255);
        public static Color current = new Color32(255, 214, 0, 255);
        public static Color locked = new Color32(120, 120, 120, 255);
    }
}
