using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using PoolPhysics;
using UnityEngine.SceneManagement;

// Controls cue movement position etc..
// Needs fixing, only for demo purposes.
public class CueController : MonoBehaviour
{
    public PhysicalBody2D mainBall;
    public float turnSpeed = 20;
    bool shooting;
    float maxdst =2;
    public float shootSpeed = 10;
    float radius;
    SpriteRenderer renderer;
    Vector3 lastPos;
    public float clampDst = 20;
    public float maxBallVelocity = 10;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        radius = mainBall.GetComponent<PoolPhysics.CircleCollider2D>().radius;

#if (!UNITY_EDITOR)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }
    bool mouseDown;
    void Update()
    {
        // Not efficent
        // Only for demo purposes
        var objects = GameObject.FindGameObjectsWithTag("NumBall");
        if (objects.Length == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        mainBall.velocity = Vector3.ClampMagnitude(mainBall.velocity, maxBallVelocity);

        if (shooting)
        {
            if (mainBall.velocity.magnitude < 0.05F)
            {
                mainBall.velocity = Vector3.zero;
                renderer.enabled = true;
                shooting = false;
            }
        }

        if (!shooting && !mouseDown)
        {
            //transform.position = mainBall.transform.position;
            var pos = mainBall.transform.position;
            pos.z = -0.3F;
            transform.position = pos;
            //transform.position = mainBall.transform.position + mainBall.transform.right * radius;
            transform.RotateAround(mainBall.transform.position, Vector3.forward, -Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButton(0) && !shooting)
        {
            mouseDown = true;

            var bp = mainBall.transform.position;
            bp.z = -0.3F;

            var pos = transform.position;
            pos += transform.up * Input.GetAxis("Mouse Y") * shootSpeed * Time.deltaTime;
            var ofs = pos - bp;
            transform.position = bp + Vector3.ClampMagnitude(ofs, clampDst);
        }

        if (Input.GetMouseButtonUp(0) && !shooting)
        {
            mouseDown = false;
            var d = Vector3.Distance(mainBall.transform.position, transform.position);

            mainBall.ApplyImpulse(transform.up * d);
            shooting = true;
            renderer.enabled = false;
        }
    }
}
