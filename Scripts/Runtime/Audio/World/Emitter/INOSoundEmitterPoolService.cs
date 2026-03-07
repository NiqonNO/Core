using NiqonNO.Core.Audio.Logic;

namespace NiqonNO.Core.Audio.World
{
	public interface INOSoundEmitterPoolService : INOService
	{
		INOSoundInstance AssignToEmitter(NOSoundClip clip);
		void Return(INOSoundInstance instance);
	}
}