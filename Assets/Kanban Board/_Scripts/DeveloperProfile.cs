using UnityEngine;

namespace SKC.KanbanSystem
{
    /// <summary>
    /// ScriptableObject representing a developer profile for task assignment.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDeveloper", menuName = "SKC/Kanban/Developer Profile")]
    public class DeveloperProfile : ScriptableObject
    {
        /// <summary>
        /// The name of the developer.
        /// </summary>
        public string developerName = "New Developer";
        // You can add more fields here like public Texture2D avatar; public string email;
    }
}