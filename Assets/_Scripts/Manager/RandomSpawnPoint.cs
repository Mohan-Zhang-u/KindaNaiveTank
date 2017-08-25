//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

// NEED TESTING!!!
[RequireComponent(typeof(BoxCollider))]
public class RandomSpawnPoint : MonoBehaviour {
    private Transform Ground;
    private bool IsEmpty = true;
    private BoxCollider _MyCollider;

    private void OnEnable()
    {
        // this can fail.
        Ground = GameObject.Find("Ground").transform;
        IsEmpty = true;
        _MyCollider = this.GetComponent<BoxCollider>();
    }

    public void SetGround(Transform g)
    {
        Ground = g;
    }

    public Transform spawnPointTransform
    {
        get
        {
            return transform;
        }
    }

    public bool IsEmptyZone
    {
        get { return IsEmpty; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            return;
        }
        else
        {
            MoveToNewPosition();
        }
        
    }

    public void MoveToNewPosition()
    {
        IsEmpty = false;
        float x = Ground.lossyScale.x;
        float z = Ground.lossyScale.z;
        Vector3 randomvector;
        Collider[] checker;
        for (int i = 0; i < 10; i++)
        {
            // 2 in order to avoid collide with ground
            randomvector = new Vector3(Random.Range(-x, x), 2 * _MyCollider.size.y, Random.Range(-z, z));
            checker = Physics.OverlapSphere(randomvector, _MyCollider.size.x);
            if (checker.Length == 0 || (checker.Length == 1 && checker[0].gameObject.layer == LayerMask.NameToLayer("Ground")))
            {
                transform.SetPositionAndRotation(randomvector, transform.rotation);
                IsEmpty = true;
                break;
            }
        }
        IsEmpty = false;
    }
}
