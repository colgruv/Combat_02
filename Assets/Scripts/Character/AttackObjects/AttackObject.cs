using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    protected NetworkCharacterData m_Owner;
    public NetworkCharacterData Owner { get { return m_Owner; } }
	
    // Use this for initialization
	void Start ()
    {
        findOwner(transform);
	}

    /// <summary>
    /// Recursively step up through hierarchy until a CharacterInfo component is found.
    /// </summary>
    /// <param name="_transform">Current transform component</param>
    protected void findOwner(Transform _transform)
    {
        m_Owner = _transform.GetComponent<NetworkCharacterData>();

        if (m_Owner == null)
        {
            if (_transform.parent == null)
            {
                Debug.LogWarning("Could not find CharacterInfo m_Owner in AttackObject parentage");
                return;
            }
            else
            {
                findOwner(_transform.parent);
            }
        }
    }
}
