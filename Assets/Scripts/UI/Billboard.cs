using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Camera BillboardCamera;

    void Update()
    {
        if (BillboardCamera)
            transform.forward = BillboardCamera.transform.forward;
        else
            transform.forward = Camera.main.transform.forward;
    }

}
