using System;

public class PawnPossessedEventArgs : EventArgs
{
	private Controller _controller;

	public Controller controller
	{
		get
		{
			return _controller;
		}
	}

	public PawnPossessedEventArgs(Controller oController)
	{
		_controller = oController;
	}
}
