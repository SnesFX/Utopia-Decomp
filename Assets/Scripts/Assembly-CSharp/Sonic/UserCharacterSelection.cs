using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	[RequireComponent(typeof(User))]
	public class UserCharacterSelection : MonoBehaviour
	{
		private User _user;

		[SerializeField]
		private Character _characterPrefab;

		public User user
		{
			get
			{
				return (!_user) ? (_user = GetComponent<User>()) : _user;
			}
		}

		public Character characterPrefab
		{
			get
			{
				return _characterPrefab;
			}
		}
	}
}
