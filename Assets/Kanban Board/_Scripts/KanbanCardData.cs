using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKC.KanbanSystem
{
    [CreateAssetMenu(fileName = "NewKanbanCard", menuName = "SKC/Kanban/Kanban Card")]
    public class KanbanCardData : ScriptableObject
    {
        public string title = "New Task";
        [TextArea(3, 10)] // Çok satırlı metin girişi için
        public string description = "";
        public KanbanColumnType currentColumn = KanbanColumnType.ToDo;
        public int orderIndex; // Kartın aynı sütun içindeki sıralaması
        
        public DeveloperProfile assignedTo; // YENİ: Atanan Geliştirici
    }
}