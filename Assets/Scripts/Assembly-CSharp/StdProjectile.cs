using System;
using UnityEngine;

[Serializable]
public class StdProjectile : Projectile
{
	[SerializeField]
	private ImpactSetupData _impact;

	public ImpactSetupData impact
	{
		get
		{
			return _impact;
		}
	}
}
