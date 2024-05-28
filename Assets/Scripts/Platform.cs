using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Transform startPosition; // 시작 지점
    public Transform endPosition; // 끝 지점
    public float speed = 2.0f; // 발판 속도

    private Vector3 target; // 현재 목표 지점
    public LayerMask playerLayer; // Player 레이어를 위한 레이어 마스크

    void Start()
    {
        // 초기 목표 지점 설정
        target = endPosition.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // 발판을 현재 목표 지점으로 이동시킵니다.
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 발판이 목표 지점에 도달했는지 확인합니다.
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // 목표 지점을 반대로 전환합니다.
            target = (target == startPosition.position) ? endPosition.position : startPosition.position;
        }
    }

    // 플레이어가 발판에 닿았을 때, 발판과 함께 움직이도록 함
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체의 레이어를 레이어 마스크와 비교하여 확인
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            collision.transform.SetParent(transform);
        }
    }

    // 플레이어가 발판에서 떨어졌을 때, 발판의 자식 관계를 해제
    void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            collision.transform.SetParent(null);
        }
    }

}
