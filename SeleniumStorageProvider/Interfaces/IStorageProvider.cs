using SeleniumStorageProvider.Enum;

namespace SeleniumStorageProvider.Interfaces
{
    public interface IStorageProvider
    {
        void Save(byte[] screenshot, string pageSource, string url, string message, string methodName, EventType eventType);
        void Save(string screenRecordingFile, string pageSource, string url, string message, string methodName, EventType eventType);
    }
}
