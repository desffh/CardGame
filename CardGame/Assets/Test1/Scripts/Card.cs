using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

// Card에 들어갈 스크립트
public class Card : MonoBehaviour
{
    [SerializeField] SpriteRenderer card;
    [SerializeField] Sprite cardFront;
    [SerializeField] Sprite cardBack;

    public Item item;
    bool isFront;

    public PRS originPRS; // 카드 원본위치를 담은 PRS 클래스

    public void Setup(Item item, bool isFront)
    {
        this.item = item;
        this.isFront = isFront;

        // 앞면이라면
        if (this.isFront)
        {
            card.sprite = cardFront;
        }
        else
        {
            card.sprite = cardBack;
        }
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {

            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }
}
