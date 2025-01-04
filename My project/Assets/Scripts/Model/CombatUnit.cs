using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
[CreateAssetMenu]
public class CombatUnit : ScriptableObject
{
    // [field: SerializeField] public bool IsStackable { get; set; }

    public int ID => GetInstanceID();

    // [field: SerializeField] public int MaxStackSize { get; set; } = 1;

    [field: SerializeField] public string UnitName { get; set; }

    [field: SerializeField] public Sprite UnitSprite { get; set; }
    [field: SerializeField] public GameObject Prefab { get; set; }

    [field: SerializeField]
    [field: TextArea]
    public string Description { get; set; }
}
}
