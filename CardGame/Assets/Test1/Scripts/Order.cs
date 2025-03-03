using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] Renderer CardRenderer;
    int originOrder;

    // 카드 정렬 (order) -> 카드가 생성될 때 호출될 함수
    public void SetOriginOrder(int originOrder)
    {
        this.originOrder = originOrder;
        SetOrder(originOrder);
    }

    public void SetMostFrontOrder(bool isMostFront)
    {
        SetOrder(isMostFront ? 100 : originOrder);
    }

    // 카드가 생성될 때마다 * 10
    public void SetOrder(int order)
    {
        int mulOrder = order * 10;

        CardRenderer.sortingOrder = mulOrder;
    }
}
