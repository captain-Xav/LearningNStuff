using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPhysics : MonoBehaviour
{
    CharacterController CharacterController { get; set; }


    [SerializeField] private float _collisionBoxLenght = 0.2f;
    [SerializeField] private float _collisionBoxHeight = 0.05f;

    public bool IsGrounded { get; private set; }
    public RaycastHit GroundHitInfo { get; private set; }

    void Awake()
    {
        this.CharacterController = this.GetComponent<CharacterController>();
    }

    public void UpdatePhysics()
    {
        this.IsGrounded = false;
        ////float halfExtent = this.CharacterController.radius * 2;

        Vector3 center = this.CharacterController.transform.position;
        center.y += this.CharacterController.radius;
        // center.z += _collisionBoxLenght * 0.25f;
        //Vector3 size = new Vector3(_collisionBoxLenght / 2f, _collisionBoxHeight / 2f, _collisionBoxLenght / 2f);
        //Physics.BoxCast(center, size, Vector3.down, out RaycastHit hitInfo, this.CharacterController.transform.rotation, 1.2f);

        Physics.SphereCast(center, this.CharacterController.radius, Vector3.down, out RaycastHit hitInfo, this.CharacterController.radius);

        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.name);
            this.IsGrounded = true;
            this.GroundHitInfo = hitInfo;
        }
    }
    void OnDrawGizmosSelected()
    {
        CharacterController cc = this.GetComponent<CharacterController>();
        ////Vector3 center = Vector3.zero;
        ////center.y += _collisionBoxHeight / 2f;
        ////center.z += _collisionBoxLenght  * 0.25f;
        ////Vector3 size = new Vector3(_collisionBoxLenght, _collisionBoxHeight, _collisionBoxLenght);

        ////// Display the explosion radius when selected
        Gizmos.matrix = cc.transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        ////Gizmos.DrawWireCube(center, size);
        Gizmos.DrawWireSphere(Vector3.zero, cc.radius);

        Gizmos.color = Color.blue;
        ////Gizmos.DrawWireCube(center, size);
        Gizmos.DrawWireSphere(Vector3.up * cc.radius, cc.radius);
    }
}
