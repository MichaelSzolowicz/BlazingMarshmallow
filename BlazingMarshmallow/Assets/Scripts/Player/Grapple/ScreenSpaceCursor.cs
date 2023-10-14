using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSpaceCursor : MonoBehaviour
{
    public Image sprite;
    public GameObject target;
    

    void Update()
    {
        Vector3 newPos;
        newPos = Camera.main.WorldToScreenPoint(target.transform.position);
        sprite.rectTransform.position = newPos;
    }
}
