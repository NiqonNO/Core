namespace NiqonNO.Core.Audio.World
{
	public interface INOSoundEmitterPoolService : INOService
	{
		NOSoundEmitter Acquire();
		void Return(NOSoundEmitter emitter);

	}
}