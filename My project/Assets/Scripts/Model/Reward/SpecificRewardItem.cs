using UnityEngine;

namespace Model.Reward
{
    public class SpecificRewardItem : Item
    {
        public SpecificRewardItem(string name, string description) : base(name, description)
        {
        }

        public override void ApplyReward()
        {
            Debug.Log("Applying specific reward: " + Name);
        }
    }
}