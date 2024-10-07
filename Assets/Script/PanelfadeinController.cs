using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelfadeinController : MonoBehaviour
{

    public Animator PanelAnim;
    public Animator GameInfoAnim;

    public void OK()
    {
        if(PanelAnim != null && GameInfoAnim != null)
        {
            PanelAnim.SetBool("out", true);
            GameInfoAnim.SetBool("Out", true);
        }
        
    }
}
