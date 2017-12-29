using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : Photon.MonoBehaviour {

    private Vector3 correctPlayerPos = Vector3.zero;
    private Quaternion correctPlayerRot = Quaternion.identity;


    void Update()
    {
        if (photonView.isMine == true) return;
        float distance = Vector3.Distance(transform.position, this.correctPlayerPos);
        if (distance < 2f)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, 10f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, 10f * Time.deltaTime);
        }
        else
        {
            transform.position = this.correctPlayerPos;
            transform.rotation = this.correctPlayerRot;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }
}