using UnityEngine;
using System.Collections;

public class HomingObject : MonoBehaviour
{
    private float dis;
    private float speed = 0.1f;
    private float waitTime;
    public GameObject Target = null;
    public GameObject From = null;
    public Transform Fire = null;

    // Use this for initialization
    void Start()
    {
        dis = Vector3.Distance(transform.position, Target.transform.position);

        //포탄생성후 초반에 포탄이 벌어지듯이 연출하기위해
        //포탄의 회전을 캐릭터위치에서 포탄의 위치의 방향으로 놓습니다
        transform.rotation = Quaternion.LookRotation(transform.position - From.transform.position);
        Fire = gameObject.transform.Find("fire");
        //Fire.gameObject.SetActive(false);
    }

    void Update()
    {
        DiffusionMissile_Move_Operation();
    }

    void DiffusionMissile_Move_Operation()
    {
        if (Target == null) return;


        waitTime += Time.deltaTime;
        //1.5초 동안 천천히 forward 방향으로 전진합니다
        if (waitTime < 2.0f)
        {
            //speed += Time.deltaTime;
            transform.Translate(transform.forward * speed, Space.World);
        }
        else
        {
            // 1.5초 이후 타겟방향으로 lerp위치이동 합니다
            //Fire.gameObject.SetActive(true);
            speed += Time.deltaTime;
            float t = speed / dis;

            transform.position = Vector3.LerpUnclamped(transform.position, Target.transform.position, t);
            if(Vector3.Distance(transform.position, Target.transform.position) < 0.1f)
            {
                Destroy(gameObject);
                return;
            }
        }

        // 매프레임마다 타겟방향으로 포탄이 방향을바꿉니다
        //타겟위치 - 포탄위치 = 포탄이 타겟한테서의 방향
        Vector3 directionVec = Target.transform.position - transform.position;
        Quaternion qua = Quaternion.LookRotation(directionVec);
        transform.rotation = Quaternion.Slerp(transform.rotation, qua, Time.deltaTime * 2f);
    }
}
