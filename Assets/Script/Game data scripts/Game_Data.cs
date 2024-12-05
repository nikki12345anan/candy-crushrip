using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] IsActive;
    public int[] Highscores;
    public int[] stars;
}
public class Game_Data : MonoBehaviour
{
    public static Game_Data gamedata;
    public SaveData savedata;

    void Awake()
    {
        if (gamedata == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gamedata = this;

        }
        else
        {
            Destroy(this.gameObject);
        }
        load();

    }

    private void Start()
    {

    }

    public void save()
    {
        //create a binary that can read binary file
        BinaryFormatter formatter = new BinaryFormatter();

        // Create a route betwwen file anf program
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);

        //create a copy of save data
        SaveData data = new SaveData();
        data = savedata;

        //save the data in save
        formatter.Serialize(file, data);

        //close the data stream
        file.Close();

        Debug.Log("game saved");
    }

    public void load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            //create a binary formatter
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            savedata = formatter.Deserialize(file) as SaveData;
            file.Close();
            Debug.Log("game loaded");
        }
        else
        {
            savedata = new SaveData();
            savedata.IsActive = new bool[21];
            savedata.stars = new int[21];
            savedata.Highscores = new int[21];
            savedata.IsActive[0] = true;
        }
    }

    private void OnApplicationQuit()
    {
        save();
    }

    private void OnDisable()
    {
        save();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
