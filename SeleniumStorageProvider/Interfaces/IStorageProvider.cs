namespace SeleniumStorageProvider.Interfaces
{
    public interface IStorageProvider
    {
        void Save(byte[] file, string fileName);
    }
}
