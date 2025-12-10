using UnityEngine;
using System.Collections.Generic;

namespace TPSBR
{
    /// <summary>
    /// Manages name plates for all agents in the scene
    /// </summary>
    public class NamePlateManager : ContextBehaviour
    {
        [Header("Prefab")]
        [SerializeField]
        private GameObject _namePlatePrefab;

        [Header("Settings")]
        [SerializeField]
        private bool _showLocalPlayerNamePlate = false;

        [SerializeField]
        private Color _defaultColor = Color.white;

        private Dictionary<Agent, NamePlate> _namePlates = new Dictionary<Agent, NamePlate>();
        private Transform _namePlateContainer;

        // PUBLIC METHODS

        /// <summary>
        /// Get color for agent based on type (Marine/Soldier)
        /// This is the single source of truth for name plate colors
        /// </summary>
        public static Color GetColorForAgent(GameObject agentObject)
        {
            if (agentObject == null)
                return Color.white;

            string agentName = agentObject.name.Replace("(Clone)", "").Trim().ToLower();

            if (agentName.Contains("marine"))
            {
                return new Color(1f, 0.3f, 0.2f); // Red
            }
            else if (agentName.Contains("soldier"))
            {
                return new Color(0.2f, 0.3f, 0.8f); // Dark Blue
            }

            return Color.white;
        }

        public NamePlate CreateNamePlate(Agent agent)
        {
            if (agent == null)
            {
                Debug.LogWarning("[NamePlateManager] Cannot create name plate for null agent");
                return null;
            }

            // Remove existing name plate for this agent if it exists
            RemoveNamePlate(agent);

            // Check if we should show name plate for local player
            if (agent.HasInputAuthority && !_showLocalPlayerNamePlate)
            {
                return null;
            }

            // Check if prefab is assigned
            if (_namePlatePrefab == null)
            {
                Debug.LogWarning("[NamePlateManager] Name plate prefab is not assigned");
                return null;
            }

            // Ensure container exists
            if (_namePlateContainer == null)
            {
                _namePlateContainer = new GameObject("NamePlates").transform;
                _namePlateContainer.SetParent(transform);
            }

            // Get camera
            Camera camera = Context?.Camera?.Camera;
            if (camera == null)
            {
                Debug.LogWarning("[NamePlateManager] Camera not found");
                return null;
            }

            // Instantiate name plate
            GameObject namePlateObject = Instantiate(_namePlatePrefab, _namePlateContainer);
            NamePlate namePlate = namePlateObject.GetComponent<NamePlate>();

            if (namePlate == null)
            {
                Debug.LogError("[NamePlateManager] Name plate prefab does not have NamePlate component");
                Destroy(namePlateObject);
                return null;
            }

            // Initialize name plate
            string playerName = GetPlayerName(agent);
            namePlate.Initialize(agent, playerName, camera);

            // Set color based on agent type
            Color namePlateColor = GetNamePlateColor(agent);
            namePlate.SetColor(namePlateColor);

            // Store reference
            _namePlates[agent] = namePlate;

            return namePlate;
        }

        public void RemoveNamePlate(Agent agent)
        {
            if (agent == null)
                return;

            if (_namePlates.TryGetValue(agent, out NamePlate namePlate))
            {
                if (namePlate != null)
                {
                    Destroy(namePlate.gameObject);
                }
                _namePlates.Remove(agent);
            }
        }

        public void RemoveAllNamePlates()
        {
            foreach (var kvp in _namePlates)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
            }
            _namePlates.Clear();
        }

        // PRIVATE METHODS

        private string GetPlayerName(Agent agent)
        {
            if (agent == null)
                return "Unknown";

            // Use the InputAuthority as player name for now
            return agent.Object.InputAuthority.ToString();
        }

        private Color GetNamePlateColor(Agent agent)
        {
            if (agent == null)
                return _defaultColor;

            return GetColorForAgent(agent.gameObject);
        }

        // MonoBehaviour INTERFACE

        private void OnDestroy()
        {
            RemoveAllNamePlates();
        }
    }
}
