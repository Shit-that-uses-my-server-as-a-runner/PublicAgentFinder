namespace PublicityApp.Abstractions;

public interface IInMemoryDb
{
    Task<IEnumerable<string>>? GetByKeyAsync(string key);
    Task LoadFromStreamAsync(Stream stream);
}
