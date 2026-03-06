namespace NiqonNO.Core.Audio.World
{
	public interface INOSoundEmitterPoolService : INOService
	{
		NOSoundEmitter AcquireAvailable();
		void Return(NOSoundEmitter emitter);


	}
}