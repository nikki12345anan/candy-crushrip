using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header ("tahta boy genis")]
    public int width;
    public int height;


    [Header("start tiles")]
    public TileType[] boardlayout;

    [Header("oynanabilir parca")]
    public GameObject[] dots;

    [Header("scoregoals")]
    public int[] scoregoals;

    [Header("endgame req")]
    public EndgameRequirements endgameRequirements;
    public BlankGoal[] levelGoals;

}
