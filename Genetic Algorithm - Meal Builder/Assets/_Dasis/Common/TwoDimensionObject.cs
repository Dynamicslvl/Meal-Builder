using UnityEngine;

namespace Dasis.Common
{
    public class TwoDimensionObject : MonoBehaviour
    {
        protected SpriteRenderer _renderer;
        protected Transform _transform;
        protected Rigidbody2D _rigidbody2D;

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = transform;
                }
                return _transform;
            }
        }

        public Rigidbody2D Rigidbody2D
        {
            get
            {
                if (_rigidbody2D == null)
                {
                    _rigidbody2D = GetComponent<Rigidbody2D>();
                }
                return _rigidbody2D;
            }
        }

        public Vector2 Size
        {
            get
            {
                return SpriteSize * Transform.localScale;
            }
            set
            {
                if (SpriteSize.x == 0 || SpriteSize.y == 0) return;
                Transform.localScale = value / SpriteSize;
                OnChangedSize();
            }
        }

        public Vector2 SpriteSize
        {
            get
            {
                return Renderer.sprite.bounds.size;
            }
        }

        public SpriteRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = GetComponent<SpriteRenderer>();
                }
                return _renderer;
            }
        }

        public Sprite Sprite
        {
            get { return Renderer.sprite; }
            set
            {
                Renderer.sprite = value;
            }
        }

        public float Alpha
        {
            get { return Renderer.color.a; }
            set
            {
                Color color = Renderer.color;
                color.a = value;
                Renderer.color = color;
            }
        }

        public float ZPosition
        {
            get { return Transform.position.z; }
            set
            {
                Vector3 position = Transform.position;
                position.z = value;
                Transform.position = position;
            }
        }

        public virtual void OnChangedSize() { }
    }
}
