using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgroundtile : MonoBehaviour
{

    public int HitPoints;
    public SpriteRenderer sprite;
    
    public void TakeDamage(int Damage)
    {
        HitPoints -= Damage;
        MakeLighter();
    }

    void MakeLighter()
    {
        //take the current color
        Color color = sprite.color;
        //Get color xurrent color alpha
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HitPoints <= 0)
        {
            Destroy(this.gameObject);
        }

    }
}