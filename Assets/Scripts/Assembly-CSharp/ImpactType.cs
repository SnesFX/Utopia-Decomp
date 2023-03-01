using System;

[Serializable]
public enum ImpactType
{
	None = 0,
	Physics = 1,
	Projectile = 2,
	Touch = 3,
	Ability = 4,
	Periodic = 5,
	Environment = 6,
	Absolute = 7
}
