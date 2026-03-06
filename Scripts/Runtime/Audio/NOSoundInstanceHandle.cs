namespace NiqonNO.Core.Audio
{
	public readonly struct NOSoundInstanceHandle
	{
		public static NOSoundInstanceHandle Invalid => new(-1);
		
		public int Id { get; }
		public bool IsValid => Id >= 0;

		public NOSoundInstanceHandle(int id)
		{
			Id = id;
		}
	}
}