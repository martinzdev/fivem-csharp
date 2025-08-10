using System.Threading.Tasks;

namespace core.Client.Modules.Vehicle
{
    public interface IVehicleService
    {
        Task SpawnVehicle(string modelName);
        Task DeleteVehicle();
        bool IsVehicleValid(string modelName);
    }
}
