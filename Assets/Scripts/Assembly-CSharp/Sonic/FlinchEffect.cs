using System;
using UnityEngine;

namespace Sonic
{
	[Serializable]
	public class FlinchEffect : Invulnerable
	{
		[SerializeField]
		private float _flashDelay = 0.2f;

		public float flashDelay
		{
			get
			{
				return Mathf.Clamp(_flashDelay, 0f, float.MaxValue);
			}
		}

		public override void OnAdd(ImpactEffect oEffect)
		{
			oEffect.variables.Add("timer", flashDelay);
			oEffect.actor.ImpactIncoming += oEffect.HandleTrigger;
		}

		public override void OnRemove(ImpactEffect oEffect)
		{
			oEffect.variables.Remove("timer");
			oEffect.actor.ImpactIncoming -= oEffect.HandleTrigger;
			CharacterAvatar characterAvatar = ((!oEffect.character) ? null : oEffect.character.avatar);
			if (!characterAvatar)
			{
				return;
			}
			for (int i = 0; i < characterAvatar.meshRenderers.Count; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = characterAvatar.meshRenderers[i];
				if ((bool)skinnedMeshRenderer)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
		}

		public override void OnUpdate(ImpactEffect oEffect)
		{
			if (!oEffect.character || !oEffect.character.avatar)
			{
				return;
			}
			float num = (float)oEffect.variables["timer"];
			num -= Time.deltaTime;
			if (num <= 0f)
			{
				num = flashDelay;
				CharacterAvatar characterAvatar = ((!oEffect.character) ? null : oEffect.character.avatar);
				if ((bool)characterAvatar)
				{
					for (int i = 0; i < characterAvatar.meshRenderers.Count; i++)
					{
						SkinnedMeshRenderer skinnedMeshRenderer = characterAvatar.meshRenderers[i];
						if ((bool)skinnedMeshRenderer)
						{
							skinnedMeshRenderer.enabled = !skinnedMeshRenderer.enabled;
						}
					}
				}
			}
			oEffect.variables["timer"] = num;
		}
	}
}
