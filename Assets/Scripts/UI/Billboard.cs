using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Camera BillboardCamera;

    void Update()
    {
        transform.forward = BillboardCamera.transform.forward;    
    }

}
