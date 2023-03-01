public class ActorImpactEventArgs : ActorEventArgs
{
	private ImpactApplicationData _impactData;

	public ImpactApplicationData impactData
	{
		get
		{
			return _impactData;
		}
	}

	public ActorImpactEventArgs(ContextImpact c, ImpactApplicationData oData)
	{
		_context = c;
		_impactData = oData;
	}
}
