using UnityEngine;

namespace Model.Reward
{
    public class Item
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Tetri.Tetri GeneratedTetri { get; private set; } // Optional property for Tetri rewards

        public Item(string name, string description, Tetri.Tetri generatedTetri = null)
        {
            Name = name;
            Description = description;
            GeneratedTetri = generatedTetri;
        }
    }
}