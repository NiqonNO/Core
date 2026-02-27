namespace NiqonNO.Core.Audio
{
	public interface INOSoundEmitterPoolService : INOService
	{
		public NOSoundEmitter Acquire();
		public void Return(NOSoundEmitter emitter);
	}
}