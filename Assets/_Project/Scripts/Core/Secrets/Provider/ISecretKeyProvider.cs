using System.Threading.Tasks;

namespace Core.Secrets
{
    public interface ISecretKeyProvider
    {
        public Task<(bool success, string secretValue)> TryGetSecretAsync(string keyName);
    }
}