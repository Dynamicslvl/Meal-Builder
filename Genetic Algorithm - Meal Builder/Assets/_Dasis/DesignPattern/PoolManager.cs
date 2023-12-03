using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Dasis.Utility;
using Dasis.Enum;

namespace Dasis.DesignPattern
{
    [HideMonoScript]
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField]
        private List<GameObject> prefabs;

        private readonly List<Pool> pools = new List<Pool>();

        [Button(SdfIconType.Recycle), GUIColor(0, 1, 0, 1)]
        public void Reorganize()
        {
            EnumGenerator.Generate("Entity", Stringify.ToStringList(prefabs));
        }

        public override void OnInitialization()
        {
            pools.Clear();
            for (int i = 0; i < System.Enum.GetValues(typeof(Entity)).Length; i++)
            {
                pools.Add(new Pool(prefabs[i], transform));
            }
        }

        public GameObject Summon(Entity entity, Vector3 position = default, Vector3 angle = default)
        {
            GameObject gameObject = pools[(int)entity].Pull();
            gameObject.transform.position = position;
            gameObject.transform.eulerAngles = angle;
            return gameObject;
        }

        public void Recall(Entity entity, GameObject gameObject)
        {
            pools[(int)entity].Push(gameObject);
        }

        public void RecallAll(Entity entity)
        {
            pools[(int)entity].PushAll();
        }
    }

    public class Pool
    {
        private readonly Stack<GameObject> _pooledObjects = new Stack<GameObject>();
        private readonly List<GameObject> _releasedObjects = new List<GameObject>();
        private readonly Transform _parent;
        private readonly GameObject _prefab;

        public int PooledCount => _pooledObjects.Count;

        public Pool(GameObject prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public GameObject Pull()
        {
            GameObject gameObject;
            if (PooledCount > 0)
                gameObject = _pooledObjects.Pop();
            else
                gameObject = Object.Instantiate(_prefab, _parent);
            gameObject.SetActive(true);
            _releasedObjects.Add(gameObject);
            return gameObject;
        }

        public void Push(GameObject gameObject)
        {
            _pooledObjects.Push(gameObject);
            _releasedObjects.Remove(gameObject);
            gameObject.SetActive(false);
        }

        public void PushAll()
        {
            while (_releasedObjects.Count > 0)
            {
                Push(_releasedObjects[0]);
            }
        }
    }
}
