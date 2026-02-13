using UnityEngine;

namespace NiqonNO.Core
{
    public class NODebug : NOScriptableObject
    {
        public void Log(string message) => Debug.Log(message);
        public void Log(float message) => Log(message.ToString("F"));
        public void Log(int message) => Log(message.ToString());
        public void Log(bool message) => Log(message.ToString());
        public void Log(Vector3 message) => Log(message.ToString());
        public void Log(Vector2 message) => Log(message.ToString());
        public void Log(Quaternion message) => Log(message.ToString());
        public void Log(Object message) => Log(message.ToString());
    }
}
