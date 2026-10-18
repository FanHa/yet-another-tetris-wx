using UnityEngine;

[CreateAssetMenu(fileName = "BattleColors", menuName = "Game Data/UI/Battle Colors")]
public sealed class BattleColors : ScriptableObject
{
    [SerializeField] private Color ally = new(0.325f, 0.722f, 1f);
    [SerializeField] private Color enemy = new(1f, 0.502f, 0.431f);

    [SerializeField] private Color health = new(0.447f, 0.839f, 0.627f);
    [SerializeField] private Color lowHealth = new(1f, 0.502f, 0.431f);
    [SerializeField] private Color buff = new(0.655f, 0.545f, 0.984f);
    [SerializeField] private Color healthBackground = new(0.271f, 0.286f, 0.341f);

    public Color Ally => ally;
    public Color Enemy => enemy;
    public Color Health => health;
    public Color LowHealth => lowHealth;
    public Color Buff => buff;
    public Color HealthBackground => healthBackground;
}