namespace NiqonNO.Core.Audio
{
	public interface INOSoundEmitterPool : INOService
	{
		public NOSoundEmitter Acquire();
		public void Return(NOSoundEmitter emitter);
	}
}