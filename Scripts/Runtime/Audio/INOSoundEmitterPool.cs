namespace NiqonNO.Core.Audio
{
	public interface INOSoundEmitterPool
	{
		public NOSoundEmitter Acquire();
		public void Return(NOSoundEmitter emitter);
	}
}