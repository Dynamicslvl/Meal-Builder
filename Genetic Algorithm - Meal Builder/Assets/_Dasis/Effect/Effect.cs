using UnityEngine;
using System.Collections.Generic;
using Dasis.DesignPattern;
using System;
using Dasis.Utility;
using Sirenix.OdinInspector;

namespace Dasis.Effect
{
    public class Effect : MonoBehaviour, IPoolable<Effect>
    {
        [SerializeField]
        private ParticleSystem effect;

        public Action<Effect> ReturnAction { get; set; }

        public void Initialize(Action<Effect> returnAction)
        {
            ReturnAction = returnAction;
        }

        public void ReturnToPool()
        {
            ReturnAction?.Invoke(this);
        }

        [Button]
        public void PlayOneShot()
        {
            effect.gameObject.SetActive(true);
            effect.Play();
            this.WaitForSeconds(effect.main.duration, () =>
            {
                ReturnToPool();
            });
        }
    }
}
