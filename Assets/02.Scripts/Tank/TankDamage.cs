using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.VisualScripting;

public class TankDamage : MonoBehaviourPun
{
    [SerializeField]
    private MeshRenderer[] renderers;
    private readonly string tankTag = "TANK";
    private readonly string apacheTag = "APACHE";
    private GameObject expEffect = null;
    private readonly int initHp = 100;
    private int curHp = 0;
    public Image hpBar;
    public Canvas tankCanvas;
    
    private WaitForSeconds ws;
    void Start()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        expEffect = Resources.Load<GameObject>("Effects/BigExplosionEffect");
        curHp = initHp;
        hpBar.color = Color.green;
        ws = new WaitForSeconds(5f);
    }

    public void OnDamage(string tag)
    {
        photonView.RPC("OnDamageRPC", RpcTarget.All, tag);
        Debug.Log("데미지 전달");
    }
    [PunRPC]
    void OnDamageRPC(string tag)
    {
        if (curHp > 0 && tag == tankTag)
        {
            // 데미지 전달
            HpBarInit(tag);
            if (curHp <= 0)
            {
                StartCoroutine(ExplosionTank());
            }
        }
    }
    [PunRPC]
    void OnApachDamageRPC(string Tag)
    {
        if(curHp > 0 && Tag == tankTag)
        {
            curHp -= 5;

            HpBarInit(Tag);
            if(curHp <= 0)
            {
                    StartCoroutine(ExplosionTank());
            }
        }
    }
    IEnumerator ExplosionTank()
    {
        var eff = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(eff, 2.0f);
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        SetTankVisible(false);
        tankCanvas.enabled = false;
        this.gameObject.tag = "Untagged";
        TankScriptsOnOff(false);

        yield return ws;
        TankScriptsOnOff(true);
        this.gameObject.tag = "TANK";
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        tankCanvas.enabled = true;
        SetTankVisible(true);
        curHp = initHp;
        hpBar.fillAmount = 1.0f;
        hpBar.color = Color.green;


    }

    private void TankScriptsOnOff(bool isEnable)
    {
        var scripts = GetComponentsInChildren<MonoBehaviourPun>();
        foreach(var script in scripts)
        {
            if (script != null)
            {
                script.enabled = isEnable;
            }
        }
    }

    void SetTankVisible(bool isVisible)
    {
        foreach (var meshR in renderers)
        {
            meshR.enabled = isVisible;
        }
    }
    void HpBarInit(string tag)
    {
        if (tag == tankTag)
            curHp -= 30;
        else
            curHp -= 1;
            hpBar.fillAmount = (float)curHp / (float)initHp;

        if (hpBar.fillAmount <= 0.3f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount <= 0.5f)
            hpBar.color = Color.yellow;
    }

    
}
