using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPackagingRepository
    {
        List<Packaging> GetAllPackaging();
        Packaging GetPackagingById(int id);
        void AddPackaging(Packaging packaging);
        void DeletePackaging(int id);
    }
}
