using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GraphicElement : Graphic
{
    [SerializeField][Range(0.0f, 1.0f)] float opacity = 1.0f;

    void Update()
    {
        Color clr = color;
        clr.a = opacity;
        color = clr;
    }
}
