
public class DestroyAnimation : IAnimation
{
	private readonly UnitAvatar _target;
	
	public DestroyAnimation(UnitAvatar target)
	{
		_target = target;
	}
	
	public bool ApplyAnimation(float time)
	{
		_target.Destroy();
		return true;
	}
}
