using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    [SerializeField]
    private Vector3 m_boomVector = new Vector3(0.0f, 2.5f, -5.0f), m_lookOffset = new Vector3(0.0f, 2.0f, 0.0f);

    [SerializeField]
    [Range(-180.0f, 0.0f)]
    private float m_tiltMin = -180.0f, m_panMin = -180.0f;

    [SerializeField]
    [Range(0.0f, 180.0f)]
    private float m_tiltMax = 180.0f, m_panMax = 180.0f;

    // Use this for initialization
    void Start ()
    {
        m_tiltMin = -m_tiltMin;
        m_panMin = -m_panMin;

		if (m_player == null)
        {
            Debug.Log("m_player not assigned!");
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        ChasePlayer();
	}

    private void ChasePlayer ()
    {
        Vector3 tarPos = m_player.transform.position + m_player.transform.rotation * m_boomVector;
        Vector3 lookTar = m_player.transform.position + m_lookOffset;
        Vector3 checkSightLine = tarPos - lookTar;

        RaycastHit hit;
        if (Physics.Raycast(lookTar, checkSightLine, out hit, checkSightLine.magnitude, ~LayerMask.NameToLayer("Player")))
        {
            Debug.Log("camera view obstructed by " + hit.collider.gameObject.name + "!");

            tarPos = hit.point;
        }

        transform.position = Vector3.Lerp(transform.position, tarPos, 0.1f);

        transform.rotation = Quaternion.LookRotation(lookTar - transform.position); //change to account for player rotation
    }

    public void PanTilt (Vector2 move)
    {
        float pan = (move.x >= 0.0f)? move.x * m_panMax : move.x * m_panMin, tilt = (move.y >= 0.0f) ? move.y * m_tiltMax : move.y * m_tiltMin;

        //Debug.Log("pan == " + pan.ToString() + " ; tilt == " + tilt.ToString());
    }
}
