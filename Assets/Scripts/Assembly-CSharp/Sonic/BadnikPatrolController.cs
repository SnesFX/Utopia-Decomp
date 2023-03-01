using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class BadnikPatrolController : AIController
	{
		[SerializeField]
		private LayerMask _physicalLayers = default(LayerMask);

		private Badnik _badnik;

		public LayerMask physicalLayers
		{
			get
			{
				return _physicalLayers;
			}
		}

		public Badnik badnik
		{
			get
			{
				return (!_badnik) ? (_badnik = ((!base.possessedPawn || !(base.possessedPawn is Badnik)) ? null : (base.possessedPawn as Badnik))) : _badnik;
			}
		}
	}
}
