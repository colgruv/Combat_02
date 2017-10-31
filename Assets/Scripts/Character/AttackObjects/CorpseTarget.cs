using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseTarget : AttackObject
{
    private AttackTarget m_AttackTarget;

    /// <summary>
    /// Check for any sources that would revive the caracter.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (m_AttackTarget == null)
            m_AttackTarget = transform.parent.GetComponent<AttackTarget>();
        //m_AttackTarget.Revive();
    }
}
