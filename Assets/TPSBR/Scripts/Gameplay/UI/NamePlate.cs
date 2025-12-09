using UnityEngine;
using TMPro;

namespace TPSBR
{
    /// <summary>
    /// Player name plate that always faces the camera
    /// </summary>
    public class NamePlate : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI _nameText;
        
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Header("Settings")]
        [SerializeField]
        private Vector3 _offset = new Vector3(0f, 2.5f, 0f);

        [SerializeField]
        private float _fadeDistance = 50f;

        [SerializeField]
        private float _maxDistance = 100f;

        private Transform _target;
        private Camera _camera;
        private Agent _agent;

        // PUBLIC METHODS

        public void Initialize(Agent agent, string playerName, Camera camera)
        {
            _agent = agent;
            _target = agent.Character.ThirdPersonView.HeadTransform;
            _camera = camera;
            
            if (_nameText != null)
            {
                _nameText.text = playerName;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }

        public void SetColor(Color color)
        {
            if (_nameText != null)
            {
                _nameText.color = color;
            }
        }

        public void SetNameText(string playerName)
        {
            if (_nameText != null)
            {
                _nameText.text = playerName;
            }
        }

        // MonoBehaviour INTERFACE

        private void LateUpdate()
        {
            if (_target == null || _camera == null)
            {
                return;
            }

            // Update position
            transform.position = _target.position + _offset;

            // Billboard effect - always face camera
            Vector3 directionToCamera = _camera.transform.position - transform.position;
            if (directionToCamera.sqrMagnitude > 0.001f)
            {
                // Face camera but keep upright
                Quaternion lookRotation = Quaternion.LookRotation(-directionToCamera);
                transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
            }

            // Fade based on distance
            if (_canvasGroup != null && _camera != null)
            {
                float distance = Vector3.Distance(_camera.transform.position, transform.position);
                
                if (distance > _maxDistance)
                {
                    _canvasGroup.alpha = 0f;
                }
                else if (distance > _fadeDistance)
                {
                    float fadeAmount = 1f - ((distance - _fadeDistance) / (_maxDistance - _fadeDistance));
                    _canvasGroup.alpha = Mathf.Clamp01(fadeAmount);
                }
                else
                {
                    _canvasGroup.alpha = 1f;
                }
            }
        }

        private void OnDestroy()
        {
            _target = null;
            _camera = null;
            _agent = null;
        }
    }
}
