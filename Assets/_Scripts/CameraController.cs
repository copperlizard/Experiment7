using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    private Rigidbody m_playerRB;

    private MotionBlur m_motionBlur;

    [SerializeField]
    private Vector3 m_boomVector = new Vector3(0.0f, 2.5f, -5.0f), m_lookOffset = new Vector3(0.0f, 2.0f, 0.0f);

    [SerializeField]
    [Range(-180.0f, 0.0f)]
    private float m_tiltMin = -180.0f, m_panMin = -180.0f;

    [SerializeField]
    [Range(0.0f, 180.0f)]
    private float m_tiltMax = 180.0f, m_panMax = 180.0f;

    [SerializeField]
    private float m_cameraClearanceRadius = 0.2f, m_cameraMinFollowDist = 1.0f, m_cameraMaxFollowDist = 10.0f;
    //private float m_intersectionSepDist = 0.1f;

    private float m_pan = 0.0f, m_tilt = 0.0f;

    // Use this for initialization
    void Start ()
    {
        m_tiltMin = -m_tiltMin;
        m_panMin = -m_panMin;

		if (m_player == null)
        {
            Debug.Log("m_player not assigned!");
        }
        else
        {
            m_playerRB = m_player.GetComponent<Rigidbody>();
        }

        m_motionBlur = GetComponent<MotionBlur>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        ChasePlayer();
	}

    private void ChasePlayer ()
    {
        Vector3 tarPos = m_player.transform.position + m_player.transform.rotation * (Quaternion.Euler(m_tilt, m_pan, 0.0f) * m_boomVector);
        Vector3 lookTar = m_player.transform.position + m_player.transform.rotation * m_lookOffset;

        RaycastHit hit;

        // Check if move obstructed...
        Vector3 move = tarPos - transform.position;
        if (Physics.Raycast(transform.position, move.normalized, out hit, move.magnitude, ~LayerMask.GetMask("Player", "Projectile", "PlayerBody"), QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("(Raycast) camera move obstructed by " + hit.collider.gameObject.name + "!");
            //Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow);
                        
            tarPos += hit.normal * m_cameraClearanceRadius;            
        }

        Vector3 checkSightLine = tarPos - lookTar;

        // Too close
        if (Vector3.Distance(tarPos, lookTar) < m_cameraMinFollowDist)
        {
            tarPos = lookTar + checkSightLine.normalized * m_cameraMinFollowDist;
        }
        
        // Check for sightline obstruction        
        if (Physics.SphereCast(lookTar, m_cameraClearanceRadius, checkSightLine, out hit, checkSightLine.magnitude, ~LayerMask.GetMask("Player", "Projectile", "PlayerBody"), QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("(SphereCast) camera view obstructed by " + hit.collider.gameObject.name + "!");
            
            tarPos = hit.point + hit.normal * m_cameraClearanceRadius;            
        }
        
        // Needed to account for objects less than m_cameraClearanceRadius from player
        if (Physics.Raycast(lookTar, checkSightLine, out hit, checkSightLine.magnitude, ~LayerMask.GetMask("Player", "Projectile", "PlayerBody"), QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("(Raycast) camera view obstructed by " + hit.collider.gameObject.name + "!");

            tarPos = hit.point + hit.normal * m_cameraClearanceRadius;
        }

        //Debug.DrawLine(tarPos, tarPos - checkSightLine, Color.red);
        //Debug.DrawLine(tarPos, transform.position, Color.blue);

        float lerpModA = new Vector2(m_pan / m_panMax, m_tilt / m_tiltMax).magnitude;
        float lerpModB = Mathf.SmoothStep(0.0f, 1.0f, (tarPos - transform.position).magnitude / m_cameraMaxFollowDist);
        float lerpMod = Mathf.Max(lerpModA, lerpModB);
        transform.position = Vector3.Lerp(transform.position, tarPos, (5.0f + 5.0f * lerpMod) * Time.deltaTime); 
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTar - transform.position, m_player.transform.up), (20.0f + 20.0f * lerpMod) * Time.deltaTime);

        m_motionBlur.blurAmount = m_playerRB.velocity.magnitude / 90.0f;
    }

    public void PanTilt (Vector2 move)
    {
        float pan = (move.x >= 0.0f)? move.x * m_panMax : move.x * m_panMin, tilt = (move.y >= 0.0f) ? move.y * m_tiltMax : move.y * m_tiltMin;

        m_pan = Mathf.Lerp(m_pan, pan, 3.0f * Time.deltaTime);
        m_tilt = Mathf.Lerp(m_tilt, tilt, 3.0f * Time.deltaTime);
    }    
}
