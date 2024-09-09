using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestructibleBlock : MonoBehaviour
{
    private int bonusChance = 10;
    public int myBonus = 0;
    private SpriteRenderer _renderer;
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (Random.Range(0, 101) < bonusChance)
        {
            myBonus = Random.Range(1, 3);
            switch (myBonus)
            {
                case 1:
                    _renderer.color = Color.blue;
                    break;
                case 2:
                    _renderer.color = Color.green;
                    break;
            }
        }
    }
}
