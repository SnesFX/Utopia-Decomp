using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sonic
{
	[Serializable]
	public class ItemDisplayPanel : MonoBehaviour
	{
		[SerializeField]
		private User _user;

		[SerializeField]
		private Item _item;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private Text _text;

		[SerializeField]
		private bool _displayExactDigits = true;

		[SerializeField]
		private int _digitCount = 3;

		public User user
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
			}
		}

		public Item item
		{
			get
			{
				return _item;
			}
			set
			{
				_item = value;
			}
		}

		public Image image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		public Text text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public bool displayExactDigits
		{
			get
			{
				return _displayExactDigits;
			}
			set
			{
				_displayExactDigits = value;
			}
		}

		public int digitCount
		{
			get
			{
				return Mathf.Clamp(_digitCount, 0, int.MaxValue);
			}
			set
			{
				_digitCount = Mathf.Clamp(value, 0, int.MaxValue);
			}
		}

		public void Update()
		{
			Sprite sprite = null;
			int num = 0;
			if ((bool)item)
			{
				sprite = item.icon;
				if ((bool)user && (bool)user.possessedPawn && (bool)user.possessedPawn.inventory)
				{
					num = user.possessedPawn.inventory.GetItemTotalQuantity(item);
				}
			}
			if ((bool)image)
			{
				image.sprite = sprite;
			}
			if ((bool)text)
			{
				text.text = num.ToString();
			}
		}
	}
}
