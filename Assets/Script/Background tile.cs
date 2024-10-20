using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgroundtile : MonoBehaviour
{

    public int HitPoints;
    public SpriteRenderer sprite;
    private goalManager goalmanager;
    
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
        goalmanager = FindObjectOfType<goalManager>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HitPoints <= 0)
        {
            if(goalmanager != null)
            {
                goalmanager.CompareGoal(this.gameObject.tag);
            }
            Destroy(this.gameObject);
        }

    }
}