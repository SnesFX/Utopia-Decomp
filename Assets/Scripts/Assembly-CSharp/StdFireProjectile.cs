using System;
using UnityEngine;

[Serializable]
public class StdFireProjectile : AbilityEffect
{
	[SerializeField]
	protected Projectile _projectilePrefab;

	[SerializeField]
	protected float _projectileLifetime = 5f;

	[SerializeField]
	protected Vector3 _velocity = Vector3.forward * 5f;

	[SerializeField]
	protected bool _inheritVelocity = true;

	public Projectile projectilePrefab
	{
		get
		{
			return _projectilePrefab;
		}
	}

	public float projectileLifetime
	{
		get
		{
			return Mathf.Clamp(_projectileLifetime, 0f, float.MaxValue);
		}
	}

	public Vector3 velocity
	{
		get
		{
			return _velocity;
		}
	}

	public bool inheritVelocity
	{
		get
		{
			return _inheritVelocity;
		}
	}

	public void TestFunction()
	{
	}

	private void OnEnable()
	{
		if (!projectilePrefab)
		{
			return;
		}
		Projectile projectile = UnityEngine.Object.Instantiate(projectilePrefab);
		projectile.transform.position = base.transform.position;
		projectile.transform.rotation = base.transform.rotation;
		if ((bool)projectile.physics)
		{
			Vector3 vector = projectile.transform.rotation * velocity;
			if (inheritVelocity && (bool)base.abilityBehaviour.actorPhysics)
			{
				vector += base.abilityBehaviour.actor.physics.velocity;
			}
			projectile.physics.velocity = vector;
		}
		UnityEngine.Object.Destroy(projectile.gameObject, projectileLifetime);
	}
}
