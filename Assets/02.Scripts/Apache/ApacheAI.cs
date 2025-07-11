using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ApacheAI : MonoBehaviour
{
    private readonly string tankTag = "TANK";
    public enum AppacheState { PATROL, ATTACK, DESTROY }
    public AppacheState state = AppacheState.PATROL;

    public List<Transform> patrolList;
    float rotSpeed = 15f, moveSpeed = 10f;
    Transform myTr;

    int currentPatorlIdx = 0;
    float wayCheck = 7f;
    public bool isSearch = true;
    public float attackTime = 0f;
    public float attackRemiming = 0.3f;

    private ApacheAI_Attack attak;

    void Start()
    {
        var pObj = GameObject.Find("Points");
        if (pObj != null)
            pObj.GetComponentsInChildren<Transform>(patrolList);

        patrolList.RemoveAt(0);

        myTr = transform;

        attak = GetComponent<ApacheAI_Attack>();

    }

    void FixedUpdate()
    {
        if (isSearch)
            WayPatrol();
        else
            Attack();

        
    }
    void Update()
    {
        CheckP();
    }

    void WayPatrol()
    {
        state = AppacheState.PATROL;
        Vector3 movePos = patrolList[currentPatorlIdx].position - myTr.position;

        myTr.rotation = Quaternion.Slerp(myTr.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * rotSpeed);
        myTr.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        Search();

    }
    void Search()
    {
        //float tankFindDist = (GameObject.FindWithTag(tankTag).transform.position - myTr.transform.position).sqrMagnitude;
        //// Distance로 변경하는게 좋음, 자원을 많이 먹기 때문에
        //if (tankFindDist <= 80f * 80f)
            

        if (Vector3.Distance(GameObject.FindWithTag(tankTag).transform.position, myTr.transform.position) < 80f)
            isSearch = false;
    }
    void CheckP()
    {
        if (Vector3.Distance(transform.position, patrolList[currentPatorlIdx].position) <= 5f)
        {
            if (currentPatorlIdx == patrolList.Count - 1)
                currentPatorlIdx = 0;
            else
                currentPatorlIdx++;
        }
    }

    void Attack()
    {
        state = AppacheState.ATTACK;
        Vector3 targetDist = (GameObject.FindWithTag(tankTag).transform.position - myTr.transform.position);
        myTr.rotation = Quaternion.Slerp(myTr.rotation, Quaternion.LookRotation(targetDist.normalized), Time.fixedDeltaTime * rotSpeed);
        if (Time.time - attackTime >= attackRemiming)
        {
            attak.Fire(attak.firePosL, attak.leaserBeamL);
            attak.Fire(attak.firePosR, attak.leaserBeamR);
            attackTime = Time.time;
        }
        if (targetDist.magnitude > 80f)
            isSearch = true;
    }


    
}
