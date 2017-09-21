using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackShellHandler : ShellHandlerAbstractClass
{
    [HideInInspector]
    public GameObject Target; //usually its a tank.

    public float LerpSpeed;

    // TODO: NNNNEEEEDDD CHECK!

    //TODO: set speed in start.

    override public void Start()
    {
        base.Start();
        StartCoroutine(ExplodeAfter());
    }

    override public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name!= "ChildCollider")
        {
            Explode(other);
        }
       
    }

    private void FixedUpdate()
    {
        if (!Target)
        {
            return;
        }
        else
        {
            transform.forward = Vector3.Lerp(transform.forward, Target.transform.position - transform.position,LerpSpeed);
        }
    }
    
    private IEnumerator ExplodeAfter()
    {
        yield return new WaitForSeconds(MaxLifeTime);
        Explode(null);
        yield return null;
    }
}
