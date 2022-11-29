using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square1x1 : MonoBehaviour
{
    [SerializeField] SpriteRenderer thisSprite;
    [SerializeField] SpriteRenderer childSprite;

    public SpriteRenderer ThisSprite => thisSprite;

    public SpriteRenderer ChildSprite => childSprite;
}
