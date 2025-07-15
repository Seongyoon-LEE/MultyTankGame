using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 플레이어 탱크가 여러개일때
// 아파치는 플레이어 탱크중 가장 가까운 거리를 탐색해서 공격하는 로직 
public class ApacheAI : MonoBehaviourPun,IPunObservable
{
    private readonly string tankTag = "TANK";
    public enum AppacheState { PATROL, ATTACK, DESTROY }
    public AppacheState state = AppacheState.PATROL;

    public List<Transform> patrolList;
    float rotSpeed = 15f, moveSpeed = 10f;
    Transform myTr;

    int currentPatrolIdx = 0;
    float wayCheck = 7f;
    public bool isSearch = true;
    public float attackTime = 1f;
    public float attackRemiming = 1f;

    private ApacheAI_Attack attak;
    // 플레이어 탱크를 담을 게임 오브젝트 배열 
    [SerializeField] private GameObject[] playerTanks = null; // 플레이어 탱크들
    Transform closetTank;
    Vector3 newWorkPosition = Vector3.zero;
    Quaternion newWorkRotation = Quaternion.identity;
    void Start()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable; // 통신 유형은 UDP 방식
        photonView.ObservedComponents[0] = this;

        var pObj = GameObject.Find("Points");
        if (pObj != null)
            pObj.GetComponentsInChildren<Transform>(patrolList);

        patrolList.RemoveAt(0);

        myTr = transform;

        attak = GetComponent<ApacheAI_Attack>();

        newWorkPosition = myTr.position;
        newWorkRotation = myTr.rotation;

        if(PhotonNetwork.IsMasterClient)
        InvokeRepeating("UpdateTankList", 0f, 0.5f); // 0.5초마다 탱크 리스트를 업데이트
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) // 마스터 클라이언트 방장 호스트만 AI로직을 실행
        {
           // isSearch 상태에 따라 WayPatrol 또는 Attack 을 직접 호출하는 대신,
           // 상태 머신 패턴을 사용하여 더 명확하게 관리합니다.
           switch(state)
            {
                case AppacheState.PATROL:
                    WayPatrol();
                    break;
                case AppacheState.ATTACK:
                    Attack();
                    break;
            }
        }
        else // 다른 클라이언트는 네트워크로 받은 위치로 부드럽게 이동
        {
            myTr.position = Vector3.Lerp(myTr.position, newWorkPosition, Time.fixedDeltaTime * 10f);
            myTr.rotation = Quaternion.Slerp(myTr.rotation, newWorkRotation, Time.fixedDeltaTime * 10f);
            Attack();
        }

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 로컬인 자신의 이동과 회전을 송신 
        {
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
            stream.SendNext((int)this.state); // 상태 정보도 동기화 하면 더 좋음 
        }
        else // 리모트의 이동과 회전을 수신 받음
        {
            newWorkPosition = (Vector3)stream.ReceiveNext();
            newWorkRotation = (Quaternion)stream.ReceiveNext();
            this.state = (AppacheState)stream.ReceiveNext(); // 상태 정보도 동기화 받음
        }
    }
    void Update()
    {
        CheckP();
    }

    void WayPatrol()
    {
        state = AppacheState.PATROL;
        Vector3 movePos = patrolList[currentPatrolIdx].position - myTr.position;

        myTr.rotation = Quaternion.Slerp(myTr.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * rotSpeed);
        myTr.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);

        // Search() 메서드 대신 Way Patrol 상태에서 직접 공격 전환 조건을 확인합니다.
        // closetTank가 null이 아니고, 거리가 80f 이내일때 공격 상태로 전환합니다.
        if(closetTank != null && Vector3.Distance(closetTank.position, myTr.position) < 80f)
        {
            state = AppacheState.ATTACK; // 공격 상태로 전환
        }

    }

    void UpdateTankList()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            closetTank = FindClossetTank();
        }
    }
    private Transform FindClossetTank()
    {
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        // 탱크가 한대도 없으면 null을 반환하여 에러 방지
        if(playerTanks == null || playerTanks.Length == 0)
        {
            return null;
        }
        Transform target = null;
        // 비교를 위해 초기 거리를 매우 큰 값으로 설정
        float closestDistSqr = Mathf.Infinity;

        foreach(GameObject _tank in playerTanks)
        {
            if(!_tank.activeInHierarchy) // 비활성화된 탱크는 제외
                continue;
            // 나와 탱크 사이의 제곱 거리를 계산
            float distSqr = (_tank.transform.position - myTr.position).sqrMagnitude;
            // 현재까지 가장 가까웠던 거리보다 더 가까우면 타겟으로 설정
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                target = _tank.transform;
            }
        }
        return target;
    }

    void CheckP()
    {
        if (Vector3.Distance(transform.position, patrolList[currentPatrolIdx].position) <= 5f)
        {
            if (currentPatrolIdx == patrolList.Count - 1)
                currentPatrolIdx = 0;
            else
                currentPatrolIdx++;
        }
    }

    void Attack()
    {
        state = AppacheState.ATTACK;
        //Vector3 targetDist = (GameObject.FindWithTag(tankTag).transform.position - myTr.transform.position);
        closetTank = FindClossetTank();
        if(closetTank == null)
        {
            print("탱크 비활성화, 다시 탐색");
            state = AppacheState.PATROL; // 타겟이 없으면 다시 페트롤 모드로 변경
            isSearch = true; // 타겟이 없으면 다시 탐색 모드로 변경
            return;
        }
        // 공격 방향 맞추기 
        Vector3 _normal = (closetTank.position - myTr.position).normalized;
        myTr.rotation = Quaternion.Slerp(myTr.rotation, Quaternion.LookRotation(_normal), Time.fixedDeltaTime * rotSpeed);

        // 공격 주기 
        if (Time.time - attackTime >= attackRemiming)
        {
            attak.Fire(attak.firePosL, attak.leaserBeamL);
            attak.Fire(attak.firePosR, attak.leaserBeamR);
            attackTime = Time.time;
        }

        // 거리가 멀어지면 다시 페트롤 
        if (Vector3.Distance(closetTank.position, myTr.position) > 80f)
        {
            state = AppacheState.PATROL; // 타겟이 멀어지면 다시 페트롤 모드로 변경
        }
    }

}
