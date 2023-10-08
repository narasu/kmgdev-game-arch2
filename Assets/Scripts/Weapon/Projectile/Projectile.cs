﻿using UnityEngine;
public class Projectile : BasePhysicsActor, IPoolable
{
    private float damage;
    private float radius;
    private Transform mainCamera;
    public Projectile()
    {
        SceneObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        PhysicsBody = SceneObject.AddComponent<Rigidbody>();
        base.InitializeActor();
        mainCamera = Camera.main.transform;
    }
    
    public void Initialize(ProjectileData _projectileData)
    {
        SceneObject.transform.position = mainCamera.position + (mainCamera.forward * 5.0f);
        SceneObject.transform.rotation = Quaternion.Euler(mainCamera.forward);
        PhysicsBody.useGravity = _projectileData.HasGravity;
        PhysicsBody.velocity = Vector3.zero;
        PhysicsBody.AddForce(SceneObject.transform.forward * _projectileData.Speed);
        damage = _projectileData.Damage;
        radius = _projectileData.Radius;
        SceneObject.transform.localScale = Vector3.one * (radius * 0.5f);
    }

    public override void FixedUpdate()
    {
        Collider[] hitColliders = new Collider[8];
        int numColliders = Physics.OverlapSphereNonAlloc(SceneObject.transform.position, radius, hitColliders);
        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                if (ActorDirectory.TryLocate(hitColliders[i].gameObject, out IActor actor))
                {
                    actor.GetComponent<IDamageable>()?.TakeDamage(damage);
                }
            }
        }
    }

    public void OnHit()
    {
    }

    public void OnEnableObject()
    {
        IsActive = true;
    }

    public void OnDisableObject()
    {
        IsActive = false;
    }
}
