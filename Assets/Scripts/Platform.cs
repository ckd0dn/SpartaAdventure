using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public Transform startPosition; // ���� ����
    public Transform endPosition; // �� ����
    public float speed = 2.0f; // ���� �ӵ�

    private Vector3 target; // ���� ��ǥ ����
    public LayerMask playerLayer; // Player ���̾ ���� ���̾� ����ũ

    void Start()
    {
        // �ʱ� ��ǥ ���� ����
        target = endPosition.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // ������ ���� ��ǥ �������� �̵���ŵ�ϴ�.
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // ������ ��ǥ ������ �����ߴ��� Ȯ���մϴ�.
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            // ��ǥ ������ �ݴ�� ��ȯ�մϴ�.
            target = (target == startPosition.position) ? endPosition.position : startPosition.position;
        }
    }

    // �÷��̾ ���ǿ� ����� ��, ���ǰ� �Բ� �����̵��� ��
    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ü�� ���̾ ���̾� ����ũ�� ���Ͽ� Ȯ��
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            collision.transform.SetParent(transform);
        }
    }

    // �÷��̾ ���ǿ��� �������� ��, ������ �ڽ� ���踦 ����
    void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            collision.transform.SetParent(null);
        }
    }

}
