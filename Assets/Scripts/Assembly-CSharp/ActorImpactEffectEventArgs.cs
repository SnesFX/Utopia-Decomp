public class ActorImpactEffectEventArgs : ActorEventArgs
{
	private ImpactEffectApplicationData _impactEffectData;

	public ImpactEffectApplicationData impactEffectData
	{
		get
		{
			return _impactEffectData;
		}
	}

	public ActorImpactEffectEventArgs(ContextImpactEffect c, ImpactEffectApplicationData oData)
	{
		_context = c;
		_impactEffectData = oData;
	}
}
