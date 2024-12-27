#if !DISABLE_OBSOLETE_CLASSES

using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private List<Tetri> deck;
    private List<Tetri> used;

    void Start()
    {
        deck = new List<Tetri>();
        used = new List<Tetri>();

        // 初始化牌库
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        // 示例初始化一些 Tetri 对象
        int[,] shape1 = new int[,]
        {
            { 1, 1, 0 },
            { 0, 1, 1 },
            { 0, 0, 0 }
        };
        int[,] shape2 = new int[,]
        {
            { 1, 1, 1 },
            { 0, 1, 0 },
            { 0, 0, 0 }
        };
        int[,] shape3 = new int[,]
        {
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 }
        };
        int[,] shape4 = new int[,]
        {
            { 1, 1 },
            { 1, 1 }
        };
        int[,] shape5 = new int[,]
        {
            { 0, 1, 1 },
            { 1, 1, 0 },
            { 0, 0, 0 }
        };
        int[,] shape6 = new int[,]
        {
            { 1, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 1 }
        };

        deck.Add(new Tetri(shape1));
        deck.Add(new Tetri(shape2));
        deck.Add(new Tetri(shape3));
        deck.Add(new Tetri(shape4));
        deck.Add(new Tetri(shape5));
        deck.Add(new Tetri(shape6));
    }

    public List<Tetri> DrawCards(int count)
    {
        List<Tetri> drawnCards = new List<Tetri>();

        for (int i = 0; i < count; i++)
        {
            if (deck.Count == 0)
            {
                ReshuffleUsedCards();
            }

            if (deck.Count > 0)
            {
                int index = Random.Range(0, deck.Count);
                Tetri drawnCard = deck[index];
                deck.RemoveAt(index);
                drawnCards.Add(drawnCard);
            }
        }

        return drawnCards;
    }

    public void AddUsedCard(Tetri card)
    {
        used.Add(card);
    }

    private void ReshuffleUsedCards()
    {
        deck.AddRange(used);
        used.Clear();
    }


}

#endif