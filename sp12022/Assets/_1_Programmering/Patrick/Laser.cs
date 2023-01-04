using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera cam;
    public LineRenderer lineRenderer;
    public Transform startPoint, endPoint;
    public GameObject startVFX, endVFX;
    private bool firing, startFire;
    public float Speed;
    private Vector3 origin, targetPos, currentTarget;
    [SerializeField] public bool AlwaysOn, Moveable;

    private List<ParticleSystem> particles = new List<ParticleSystem>();
    void Start(){
        origin = transform.position;
        targetPos = new Vector3(transform.Find("TargetPosition").position.x, transform.Find("TargetPosition").position.y);
        FillLists();
        DisableLaser();
        if (AlwaysOn)
            Fire();
    }

    // Update is called once per frame
    void Update()
    {
        if (startFire)
        {
            EnableLaser();
            startFire = false;
        }

        if (firing)
        {
            UpdateLaser();
            Move();
        }

        if (!firing)
        {
            DisableLaser();
        }
    }

    public void Fire(){
        startFire = true;
    }

    public void StopFiring(){
        firing = false;
    }

    public bool CurrentlyFiring(){
        return startFire || firing;
    }

    private void DisableLaser(){
        lineRenderer.enabled = false;
        
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop();
        }
    }

    private void UpdateLaser(){
        var targetPos = (Vector2)endPoint.position;
        
        lineRenderer.SetPosition(0,startPoint.position);
        startVFX.transform.position = (Vector2)startPoint.position;
        
        lineRenderer.SetPosition(1,targetPos);

        Vector2 direction = targetPos - (Vector2)transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll((Vector2)transform.position, direction.normalized, direction.magnitude);
        if (hits.Length>0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.gameObject.name == "Player" || hit.transform.gameObject.name == "Level")
                {
                    lineRenderer.SetPosition(1, hit.point);
                }
            }
        }
        
        endVFX.transform.position = lineRenderer.GetPosition(1);
    }

    private void Move(){
        if (!Moveable) return;
        
        if (transform.position==targetPos)
            currentTarget = origin;
        if (transform.position==origin)
            currentTarget = targetPos;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, Speed * Time.deltaTime);
    }

    private void EnableLaser(){
        lineRenderer.enabled = true;
        
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }

        firing = true;
    }

    void FillLists(){
        for (int i = 0; i < startVFX.transform.childCount; i++)
        {
            var ps = startVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                particles.Add(ps);
            }
        }
        
        for (int i = 0; i < endVFX.transform.childCount; i++)
        {
            var ps = endVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
            {
                particles.Add(ps);
            }
        }
    }
}
