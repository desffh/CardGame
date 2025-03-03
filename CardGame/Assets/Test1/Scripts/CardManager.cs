using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // 싱글톤
    public static CardManager Inst { get; private set; }

    private void Awake()
    {
        Inst = this;
    }

    // private지만 에디터에서 보이게
    [SerializeField] ItemSO ItemSO;

    // Item 타입을 담을 List 선언 (리스트는 동적배열 - 아직 크기 할당 안된 상태)
    List<Item> itemBuffer;

    [SerializeField] List<Card> myCards; // Card 타입을 담을 리스트 (내 카드)

    [SerializeField] Transform cardSpawnPoint; // 카드 생성위치 가져오기 


    // 카드 정렬 시작, 끝 위치
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;




    // 리스트에 아이템을 넣어줄 함수
    void SetupItemBuffer()
    {
        // 크기 동적할당
        itemBuffer = new List<Item>(100); // 미리 용량을 100 잡아두기

        for(int i = 0; i < ItemSO.items.Length; i++)
        {
            Item item = ItemSO.items[i];
            
            // 퍼센트 만큼 추가
            for(int j = 0; j < item.percent; j++)
            {
                itemBuffer.Add(item);
            }
        }
        // 아이템 버퍼 안의 카드 섞기
        for(int i = 0;i < itemBuffer.Count; i++)
        {
            int rand = Random.Range(i, itemBuffer.Count);
            Item temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
    }


    // 리스트에서 카드 뽑기
    public Item PopItem()
    {
        // 다 뽑았으면 다시 버퍼 채우기 
        if (itemBuffer.Count == 0)
        {
            SetupItemBuffer();
        }

        Item item = itemBuffer[0];
        itemBuffer.RemoveAt(0); // 리스트 메서드 (0번째 요소 제거)
        return item;
    }

    // 에디터에서 카드 프리팹 연결
    [SerializeField] GameObject cardPrefabs;

    // 내 카드가 맞는지 확인
    void AddCard(bool isMine)
    {
        var cardObject = Instantiate(cardPrefabs, cardSpawnPoint.position, Utils.QI); // 게임 오브젝트 타입
        var card = cardObject.GetComponent<Card>(); // Card 타입
        card.Setup(PopItem(), isMine); // 앞면인지 뒷면인지 나오게 된다

        if(isMine == true)
        {
            myCards.Add(card);
        }

        SetOriginOrder(isMine);
        CardAlignment(isMine);
    }

    // 리스트 전체 정렬하기 (먼저 추가한 카드가 제일 뒷쪽에 보임)
    void SetOriginOrder(bool isMine)
    {
        int count = isMine ? myCards.Count : 0;

        for (int i = 0; i < count; i++)
        {
            var targetCard = isMine ? myCards[i] : null;
            // ? -> targerCard가 null이 아니면 컴포넌트 가져오기
            targetCard?.GetComponent<Order>().SetOriginOrder(i);
        }
    }

    // 카드 정렬
    void CardAlignment(bool isMine)
    {
        List<PRS> originCardPRSs = new List<PRS>();

        // 내 카드라면
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
        List<PRS> results = new List<PRS>(objCount); // objCount 만큼 용량 미리 할당

        switch (objCount)
        {
            // 고정값 : 1,2,3개 일때 (회전이 없기 때문)
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;

            // 카드가 4개 이상일때 부터 회전값이 들어감
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        // 원의 방정식
        for(int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Utils.QI;

            // 카드가 4개 이상이라면 회전값 들어감
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

    // 코루틴을 사용하여 카드가 일정한 간격으로 나옴
    IEnumerator AddCardSpawn()
    {
        for (int i = 0; i < 8; i++) // 8장의 카드를 생성
        {
            AddCard(true); // 카드 생성 함수 호출
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
