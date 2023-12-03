using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Dasis.DesignPattern;
using Dasis.Input;
using System;
using UnityEngine.Events;

namespace Dasis.Common
{
    public class LevelSelector : MonoBehaviour, ICanvasInputable
    {
        [SerializeField] private RectTransform self;
        [SerializeField] private GameObject levelManager;
        [SerializeField] private GameObject levelPagePrefab;
        [SerializeField] private TextMeshProUGUI pageIndexText;
        [SerializeField] private int levelsPerPage;
        [SerializeField] private UnityEvent onLoadLevel;
        [SerializeField] private UnityEvent playTapSound;

        public ILevelManagable LevelManager => levelManager.GetComponent<ILevelManagable>();
        public UnityEvent OnLoadLevel => onLoadLevel;
        public UnityEvent PlayTapSound => playTapSound;

        private ObjectPool<LevelPage> levelPagePool;
        private readonly List<(int, int)> levelPagesInfor = new List<(int, int)>();
        private readonly List<LevelPage> levelPages = new List<LevelPage>();
        private int currPageIndex = 0, lastPageIndex;
        private int numberOfPage = 0;
        private readonly float minDistance = 0.2f;
        private readonly float slidingDuration = 0.3f;
        private Vector2 lastMousePos, currMousePos;
        private Vector2 centerPos;
        private Vector2 leftPos, rightPos;

        private int lastInc = 0;
        private Sequence sq;

        private void OnEnable()
        {
            InputReceiver.OnWorldspaceMouseUp += OnCanvasMouseUp;
            InputReceiver.OnWorldspaceMouseDown += OnCanvasMouseDown;
        }

        private void OnDisable()
        {
            InputReceiver.OnWorldspaceMouseUp -= OnCanvasMouseUp;
            InputReceiver.OnWorldspaceMouseDown -= OnCanvasMouseDown;
        }

        public void Initialize(int currentLevel, int maxLevel)
        {
            SetUpProperties();
            GeneratePages(currentLevel, maxLevel);
        }

        public void SetUpProperties()
        {
            centerPos = levelPagePrefab.transform.localPosition;
            leftPos = centerPos - new Vector2(self.rect.width, 0);
            rightPos = centerPos + new Vector2(self.rect.width, 0);
            if (levelPagePool == null)
            {
                levelPagePool = new ObjectPool<LevelPage>(levelPagePrefab, transform);
            }
            levelPagePool.PushAll();
            levelPages.Clear();
            levelPagesInfor.Clear();
        }

        public void GeneratePages(int currentLevel, int maxLevel)
        {
            int l = 1, r;
            numberOfPage = 0;
            int firstShowPageIndex = 0;

            while (l <= maxLevel)
            {
                numberOfPage++;
                r = l + levelsPerPage - 1;
                if (r > maxLevel)
                    r = maxLevel;
                if (r < currentLevel)
                    firstShowPageIndex++;

                levelPagesInfor.Add((l, r));
                l += levelsPerPage;
            }
            SetFirstShowPage(firstShowPageIndex);
        }

        public void RemoveRedundantPages()
        {
            List<int> workingPagesIndex = new List<int>
            {
                lastPageIndex, currPageIndex
            };
            for (int i = levelPages.Count - 1; i >= 0; i--)
            {
                LevelPage page = levelPages[i];
                if (!workingPagesIndex.Contains(page.Index)) {
                    page.ReturnToPool();
                    levelPages.RemoveAt(i);
                }
            }
        }

        private LevelPage GetLevelPage(int index)
        {
            foreach (var page in levelPages)
            {
                if (page.Index == index) // If already exist in list then no need to create new
                    return page;
            } 
            // Create new
            LevelPage levelPagePanel = levelPagePool.Pull();
            levelPagePanel.LevelSelector = this;
            index = Mathf.Min(index, levelPagesInfor.Count - 1);
            levelPagePanel.Index = index;
            levelPagePanel.InitializeButtons(levelPagesInfor[index]);
            levelPages.Add(levelPagePanel);
            return levelPagePanel;
        }

        public void SetFirstShowPage(int pageIndex)
        {
            currPageIndex = Mathf.Min(numberOfPage - 1, pageIndex);
            LevelPage levelPagePanel = GetLevelPage(pageIndex);
            levelPagePanel.gameObject.SetActive(true);
            levelPagePanel.transform.localPosition = centerPos;
            pageIndexText.text = (currPageIndex + 1) + "/" + numberOfPage;
        }

        public void MoveToRelativePage(int inc)
        {
            playTapSound?.Invoke();

            if (lastInc == inc)
            {
                GetLevelPage(lastPageIndex).gameObject.SetActive(false);
            }

            pageIndexText.text = (currPageIndex + 1) + "/" + numberOfPage;
            int nextPageIndex = (currPageIndex + inc + numberOfPage) % numberOfPage;
            GetLevelPage(nextPageIndex).gameObject.SetActive(true);

            /* Animation */
            if (sq != null)
            {
                sq.Kill();
                RemoveRedundantPages();
                pageIndexText.text = (currPageIndex + 1) + "/" + numberOfPage;
            }

            sq = DOTween.Sequence();

            if (inc < 0)
            {
                GetLevelPage(nextPageIndex).transform.localPosition = leftPos;
                sq.Append(GetLevelPage(nextPageIndex).transform.DOLocalMove(centerPos, slidingDuration));
                sq.Join(GetLevelPage(currPageIndex).transform.DOLocalMove(rightPos, slidingDuration));
            }
            else
            {
                GetLevelPage(nextPageIndex).transform.localPosition = rightPos;
                sq.Append(GetLevelPage(nextPageIndex).transform.DOLocalMove(centerPos, slidingDuration));
                sq.Join(GetLevelPage(currPageIndex).transform.DOLocalMove(leftPos, slidingDuration));
            }

            lastInc = inc;
            lastPageIndex = currPageIndex;
            currPageIndex = nextPageIndex;

            sq.OnComplete(() =>
            {
                RemoveRedundantPages();
                pageIndexText.text = (currPageIndex + 1) + "/" + numberOfPage;
            });
        }

        public void OnCanvasMouseDown(Vector2 mousePos)
        {
            lastMousePos = mousePos;
        }

        public void OnCanvasMouseHold(Vector2 mousePos) {}

        public void OnCanvasMouseUp(Vector2 mousePos)
        {
            currMousePos = mousePos;
            // Sliding to Right
            if (currMousePos.x - lastMousePos.x > minDistance)
            {
                MoveToRelativePage(-1);
            }
            // Sliding to Left
            if (currMousePos.x - lastMousePos.x < -minDistance)
            {
                MoveToRelativePage(+1);
            }
        }
    }
}
