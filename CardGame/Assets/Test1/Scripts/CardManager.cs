using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // �̱���
    public static CardManager Inst { get; private set; }

    private void Awake()
    {
        Inst = this;
    }

    // private���� �����Ϳ��� ���̰�
    [SerializeField] ItemSO ItemSO;

    // Item Ÿ���� ���� List ���� (����Ʈ�� �����迭 - ���� ũ�� �Ҵ� �ȵ� ����)
    List<Item> itemBuffer;

    [SerializeField] List<Card> myCards; // Card Ÿ���� ���� ����Ʈ (�� ī��)

    [SerializeField] Transform cardSpawnPoint; // ī�� ������ġ �������� 


    // ī�� ���� ����, �� ��ġ
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;




    // ����Ʈ�� �������� �־��� �Լ�
    void SetupItemBuffer()
    {
        // ũ�� �����Ҵ�
        itemBuffer = new List<Item>(100); // �̸� �뷮�� 100 ��Ƶα�

        for(int i = 0; i < ItemSO.items.Length; i++)
        {
            Item item = ItemSO.items[i];
            
            // �ۼ�Ʈ ��ŭ �߰�
            for(int j = 0; j < item.percent; j++)
            {
                itemBuffer.Add(item);
            }
        }
        // ������ ���� ���� ī�� ����
        for(int i = 0;i < itemBuffer.Count; i++)
        {
            int rand = Random.Range(i, itemBuffer.Count);
            Item temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
    }


    // ����Ʈ���� ī�� �̱�
    public Item PopItem()
    {
        // �� �̾����� �ٽ� ���� ä��� 
        if (itemBuffer.Count == 0)
        {
            SetupItemBuffer();
        }

        Item item = itemBuffer[0];
        itemBuffer.RemoveAt(0); // ����Ʈ �޼��� (0��° ��� ����)
        return item;
    }

    // �����Ϳ��� ī�� ������ ����
    [SerializeField] GameObject cardPrefabs;

    // �� ī�尡 �´��� Ȯ��
    void AddCard(bool isMine)
    {
        var cardObject = Instantiate(cardPrefabs, cardSpawnPoint.position, Utils.QI); // ���� ������Ʈ Ÿ��
        var card = cardObject.GetComponent<Card>(); // Card Ÿ��
        card.Setup(PopItem(), isMine); // �ո����� �޸����� ������ �ȴ�

        if(isMine == true)
        {
            myCards.Add(card);
        }

        SetOriginOrder(isMine);
        CardAlignment(isMine);
    }

    // ����Ʈ ��ü �����ϱ� (���� �߰��� ī�尡 ���� ���ʿ� ����)
    void SetOriginOrder(bool isMine)
    {
        int count = isMine ? myCards.Count : 0;

        for (int i = 0; i < count; i++)
        {
            var targetCard = isMine ? myCards[i] : null;
            // ? -> targerCard�� null�� �ƴϸ� ������Ʈ ��������
            targetCard?.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    // ī�� ����
    void CardAlignment(bool isMine)
    {
        List<PRS> originCardPRSs = new List<PRS>();

        // �� ī����
        if(isMine)
        {
            originCardPRSs = RoundAlignment(myCardLeft, myCardRight, myCards.Count, 0.5f, Vector3.one * 0.7f);
        }

        var targetCards = isMine ? myCards : null;

        for(int i = 0;i < targetCards.Count;i++)
        {
            var targetCard = targetCards[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount); // objCount ��ŭ �뷮 �̸� �Ҵ�

        switch (objCount)
        {
            // ������ : 1,2,3�� �϶� (ȸ���� ���� ����)
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;

            // ī�尡 4�� �̻��϶� ���� ȸ������ ��
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        // ���� ������
        for(int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Utils.QI;

            // ī�尡 4�� �̻��̶�� ȸ���� ��
            if(objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : - curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;

    }

    // �ڷ�ƾ�� ����Ͽ� ī�尡 ������ �������� ����
    IEnumerator AddCardSpawn()
    {
        for (int i = 0; i < 8; i++) // 8���� ī�带 ����
        {
            AddCard(true); // ī�� ���� �Լ� ȣ��
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Start()
    {
        SetupItemBuffer();
       
        StartCoroutine(AddCardSpawn());
    }


    private void Update()
    {

    }
}
