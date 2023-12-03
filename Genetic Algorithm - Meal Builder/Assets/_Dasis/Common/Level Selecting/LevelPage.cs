using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dasis.DesignPattern;
using System;
using UnityEngine.UI;

namespace Dasis.Common
{
    public class LevelPage : MonoBehaviour, IPoolable<LevelPage>
    {
        [SerializeField] 
        private LevelButton prefabButton;
        private ObjectPool<LevelButton> buttonPool;
        private LevelSelector levelSelector;
        private readonly List<LevelButton> buttons = new List<LevelButton>();
        public LevelSelector LevelSelector
        {
            get { return levelSelector; }
            set { levelSelector = value; }
        }

        [field: SerializeField]
        public int Index { get; set; }
        public Action<LevelPage> ReturnAction { get; set; }

        public void InitializeButtons((int, int) levelPageInfor)
        {
            if (buttonPool == null)
            {
                buttonPool = new ObjectPool<LevelButton>(prefabButton.gameObject, transform);
            }
            buttonPool.PushAll();
            buttons.Clear();
            for (int i = levelPageInfor.Item1; i <= levelPageInfor.Item2; i++)
            {
                buttons.Add(buttonPool.Pull());
                LevelButton levelButton = buttons[i - levelPageInfor.Item1];
                levelButton.ID = i - 1;
                levelButton.NumberText.text = i.ToString();
                levelButton.LevelPage = this;
                levelButton.gameObject.SetActive(true);
                levelButton.transform.SetAsLastSibling();
            }
            UpdateButtonsState();
        }

        private void OnEnable()
        {
            if (buttons.Count == 0) 
                return;
            UpdateButtonsState();
        }

        public void OnLevelButtonClick(LevelButton button)
        {
            if (button.State == LevelState.Locked)
            {
                return;
            }
            levelSelector.LevelManager.LoadLevel(button.ID + 1);
            levelSelector.OnLoadLevel?.Invoke();
            UpdateButtonsState();
        }

        public void UpdateButtonsState()
        {
            int unlockLevel = levelSelector.LevelManager.UnlockLevel;

            foreach (var button in buttons)
            {
                if (button.LevelID < unlockLevel)
                {
                    button.SetState(LevelState.Completed);
                }
                if (button.LevelID == unlockLevel)
                {
                    button.SetState(LevelState.CurrentPlay);
                }
                if (button.LevelID > unlockLevel)
                {
                    button.SetState(LevelState.Locked);
                }
            }
        }

        public void Initialize(Action<LevelPage> returnAction)
        {
            ReturnAction = returnAction;
        }

        public void ReturnToPool()
        {
            ReturnAction?.Invoke(this);
        }
    }

    public interface ILevelManagable
    {
        public int UnlockLevel { get; set; }
        public int MaxLevel { get; }
        public void LoadLevel(int levelIndex);
    }
}
