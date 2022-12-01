using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square1x1 : MonoBehaviour
{
    [SerializeField] SpriteRenderer thisSprite;
    [SerializeField] SpriteRenderer childSprite;

    [SerializeField] Color defaultChildColor;

    void Awake()
    {
        defaultChildColor = childSprite.color;
    }

    public SpriteRenderer ThisSprite => thisSprite;

    public SpriteRenderer ChildSprite => childSprite;

    public Color DefaultChildColor => defaultChildColor;
}
