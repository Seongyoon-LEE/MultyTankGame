using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
//using UnityEditor.EditorTools;

public class TankDamage : MonoBehaviourPun
{
    [SerializeField] int maxHp = 100;
    [SerializeField] int curHp = 0;
    [SerializeField] Image hpBar; // 체력바 UI
    [SerializeField] MeshRenderer[] tankMeshRenderer;
    GameObject expEffect;

    WaitForSeconds ws;

    readonly string tankTag = "TANK";
    readonly string apacheTag = "APACHE";
    void Start()
    {
        tankMeshRenderer = GetComponentsInChildren<MeshRenderer>(); // 탱크의 모든 MeshRenderer를 가져옴
        curHp = maxHp; // 현재 체력을 최대 체력으로 초기화
        expEffect = Resources.Load<GameObject>("Effects/BigExplosionEffect");
        hpBar.color = Color.green; // 체력바의 초기 색상 설정
        ws = new WaitForSeconds(5f); // 폭발 효과 재생 대기 시간 설정
    }
    public void OnDamage(string Tag)
    {

        // 로컬 플레이어가 데미지를 받았을 때
        photonView.RPC("OnDamageRPC", RpcTarget.All, Tag); // 네트워크 상의 모든 플레이어에게 데미지 정보를 전달
        print(Tag + "가 데미지를 받았습니다. 현재 체력: " + curHp);

        // 리모트 플레이어가 데미지를 받았을 때
        photonView.RPC("OnDamageRPC", RpcTarget.Others, Tag); // 다른 플레이어에게 데미지 정보를 전달
    }
        [PunRPC] // Photon RPC를 사용하여 네트워크 상에서 호출 가능
        void OnDamageRPC(string Tag)
        {
            if (curHp > 0 && Tag == tankTag)
            {
                // 데미지 전달
                HpBarInit(Tag);
                if (curHp < 0)
                    StartCoroutine(ExplosionTank());
            }
        }
        IEnumerator ExplosionTank()
        {
            GameObject eff = Instantiate(expEffect, transform.position, Quaternion.identity);
            Destroy(eff, 2.0f); // 폭발 효과를 2초 후에 제거
            GetComponent<BoxCollider>().enabled = false; // 탱크의 충돌체를 비활성화
            SetTankVisible(false); // 탱크를 보이지 않게 설정
            yield return ws;
            SetTankVisible(true); // 5초 후에 탱크를 다시 보이게 설정
            GetComponent<BoxCollider>().enabled = true; // 탱크의 충돌체를 비활성화
            curHp = maxHp; // 탱크의 체력을 최대 체력으로 초기화
        }
        void SetTankVisible(bool isVisible)
        {
            // 탱크의 MeshRenderer를 활성화 또는 비활성화
            foreach (MeshRenderer mesh in tankMeshRenderer)
            {
                mesh.enabled = isVisible;
            }
        }
        void HpBarInit(string Tag)
        {
            if (Tag == tankTag)
                curHp -= 30; // 탱크가 받는 데미지
            else
                curHp -= 1;
            hpBar.fillAmount = (float)curHp / (float)maxHp; // 체력바 업데이트
            if (hpBar.fillAmount <= 0.3f)
            {
                hpBar.color = Color.red; // 체력바 색상을 빨간색으로 변경
            }
            else if (hpBar.fillAmount <= 0.5f)
            {
                hpBar.color = Color.yellow; // 체력바 색상을 노란색으로 변경
            }
        }
    }

