using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSPlash : MonoBehaviour
{
    public string Scenetoload;
    public void OK()
    {
        SceneManager.LoadScene(Scenetoload);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
