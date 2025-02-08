using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Model.Tetri
{
    [Serializable]
    public abstract class TetriCell
    {
        [SerializeField]
        public string Name { get; }

        // Default constructor
        // public TetriCell() {
        //     Name = "TetriCell";
        // }

        // // Deserialization constructor
        // protected TetriCell(SerializationInfo info, StreamingContext context)
        // {
        //     // Deserialize the Name property
        //     Name = info.GetString("Name");
        // }

        // public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        // {
        //     // Serialize the Name property
        //     info.AddValue("Name", Name);
        // }
    }
}