using System;
using Unity.Entities;
using UnityEngine;

namespace AuthoringComponents
{
    public class SelectableAuthoring : MonoBehaviour
    {
        [SerializeField] private bool isSelectedInitially;

        public class Baker : Baker<SelectableAuthoring>
        {
            public override void Bake(SelectableAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<Selectable>(entity, new Selectable { IsSelected = authoring.isSelectedInitially });
            }
        }
    }

    [Serializable]
    public struct Selectable : IComponentData
    {
        public bool IsSelected;
    }
}