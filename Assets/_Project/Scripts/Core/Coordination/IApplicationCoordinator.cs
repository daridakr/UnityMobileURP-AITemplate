using System.Threading.Tasks;

namespace Core.Coordination
{
    public interface IApplicationCoordinator
    {
        public Task InitializationTask  { get; }
    }
}