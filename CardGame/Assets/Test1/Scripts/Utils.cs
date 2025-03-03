using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PRS // 카드 원본 위치를 담는 클래스
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;


    public PRS(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}


public class Utils : MonoBehaviour
{
    public static Quaternion QI => Quaternion.identity;
}
