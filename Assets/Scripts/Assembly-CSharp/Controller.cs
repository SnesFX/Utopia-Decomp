using System;
using UnityEngine;

[Serializable]
[DisallowMultipleComponent]
public class Controller : MonoBehaviour
{
	[SerializeField]
	private Pawn _possessedPawn;

	public Pawn possessedPawn
	{
		get
		{
			return _possessedPawn;
		}
		protected set
		{
			_possessedPawn = value;
		}
	}

	public virtual void Possess(Pawn oPawn)
	{
		if (!(oPawn == null))
		{
			possessedPawn = oPawn;
			possessedPawn.Possess(this);
		}
	}

	public virtual void Unpossess(Pawn oPawn)
	{
		if (oPawn == possessedPawn)
		{
			oPawn.Unpossess();
			possessedPawn = null;
		}
	}

	private void Awake()
	{
		if ((bool)possessedPawn)
		{
			Possess(possessedPawn);
		}
	}
}
