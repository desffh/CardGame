using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item
{
    public string name;
    public int attack;
    public int health;
    public Sprite sprite;
    public float percent;
}



[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
// SO 상속받기
public class ItemSO : ScriptableObject
{
    // Item 타입의 배열
    public Item[] items;
}
