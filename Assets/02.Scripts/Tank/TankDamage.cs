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
    [SerializeField] Image hpBar; // ü�¹� UI
    [SerializeField] MeshRenderer[] tankMeshRenderer;
    GameObject expEffect;

    WaitForSeconds ws;

    readonly string tankTag = "TANK";
    readonly string apacheTag = "APACHE";
    void Start()
    {
        tankMeshRenderer = GetComponentsInChildren<MeshRenderer>(); // ��ũ�� ��� MeshRenderer�� ������
        curHp = maxHp; // ���� ü���� �ִ� ü������ �ʱ�ȭ
        expEffect = Resources.Load<GameObject>("Effects/BigExplosionEffect");
        hpBar.color = Color.green; // ü�¹��� �ʱ� ���� ����
        ws = new WaitForSeconds(5f); // ���� ȿ�� ��� ��� �ð� ����
    }
    public void OnDamage(string Tag)
    {

        // ���� �÷��̾ �������� �޾��� ��
        photonView.RPC("OnDamageRPC", RpcTarget.All, Tag); // ��Ʈ��ũ ���� ��� �÷��̾�� ������ ������ ����
        print(Tag + "�� �������� �޾ҽ��ϴ�. ���� ü��: " + curHp);

        // ����Ʈ �÷��̾ �������� �޾��� ��
        photonView.RPC("OnDamageRPC", RpcTarget.Others, Tag); // �ٸ� �÷��̾�� ������ ������ ����
    }
        [PunRPC] // Photon RPC�� ����Ͽ� ��Ʈ��ũ �󿡼� ȣ�� ����
        void OnDamageRPC(string Tag)
        {
            if (curHp > 0 && Tag == tankTag)
            {
                // ������ ����
                HpBarInit(Tag);
                if (curHp < 0)
                    StartCoroutine(ExplosionTank());
            }
        }
        IEnumerator ExplosionTank()
        {
            GameObject eff = Instantiate(expEffect, transform.position, Quaternion.identity);
            Destroy(eff, 2.0f); // ���� ȿ���� 2�� �Ŀ� ����
            GetComponent<BoxCollider>().enabled = false; // ��ũ�� �浹ü�� ��Ȱ��ȭ
            SetTankVisible(false); // ��ũ�� ������ �ʰ� ����
            yield return ws;
            SetTankVisible(true); // 5�� �Ŀ� ��ũ�� �ٽ� ���̰� ����
            GetComponent<BoxCollider>().enabled = true; // ��ũ�� �浹ü�� ��Ȱ��ȭ
            curHp = maxHp; // ��ũ�� ü���� �ִ� ü������ �ʱ�ȭ
        }
        void SetTankVisible(bool isVisible)
        {
            // ��ũ�� MeshRenderer�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
            foreach (MeshRenderer mesh in tankMeshRenderer)
            {
                mesh.enabled = isVisible;
            }
        }
        void HpBarInit(string Tag)
        {
            if (Tag == tankTag)
                curHp -= 30; // ��ũ�� �޴� ������
            else
                curHp -= 1;
            hpBar.fillAmount = (float)curHp / (float)maxHp; // ü�¹� ������Ʈ
            if (hpBar.fillAmount <= 0.3f)
            {
                hpBar.color = Color.red; // ü�¹� ������ ���������� ����
            }
            else if (hpBar.fillAmount <= 0.5f)
            {
                hpBar.color = Color.yellow; // ü�¹� ������ ��������� ����
            }
        }
    }

