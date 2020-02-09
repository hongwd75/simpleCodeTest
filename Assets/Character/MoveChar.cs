using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveChar : MonoBehaviour
{
    public float speed = 2.0f;
    private Rigidbody rigid;
 
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
 
    public bool Run(Vector3 targetPos)
    {
        // 이동하고자하는 좌표 값과 현재 내 위치의 차이를 구한다.
        float dis = Vector3.Distance(transform.position, targetPos);
        if (dis >= 0.01f) // 차이가 아직 있다면
        {
            // 캐릭터를 이동시킨다.
            transform.localPosition = Vector3.MoveTowards(transform.position, targetPos, speed * (Time.deltaTime+0.3f));
            //SetAnim(PlayerAnim.ANIM_WALK); // 걷기 애니메이션 처리
            Turn(targetPos);
            return true;
        }
        return false;
        
    }
 
    public void Turn(Vector3 targetPos)
    {
        // 캐릭터를 이동하고자 하는 좌표값 방향으로 회전시킨다
        Vector3 dir = targetPos - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        rigid.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 550.0f * (Time.deltaTime + 0.3f));
    }
}
