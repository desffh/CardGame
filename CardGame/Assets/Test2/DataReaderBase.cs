using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReaderBase : ScriptableObject
{
    [Header("시트의 주소")][SerializeField] public string associatedSheet = "1Wj-NhHOCt8KN1YjZ6-hHP5KF8mozMdXT5zwoziEVQEc";
    [Header("스프레드 시트의 시트 이름")][SerializeField] public string associatedWorksheet = "Sheet1";
    [Header("읽기 시작할 행 번호")][SerializeField] public int START_ROW_LENGTH = 2;
    [Header("읽을 마지막 행 번호")][SerializeField] public int END_ROW_LENGTH = 17;
}
