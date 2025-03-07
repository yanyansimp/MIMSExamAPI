using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IItemRepository
    {
        List<Item> GetAllItems();
        Item GetItemById(int id);
        void AddItem(Item item);
        void DeleteItem(int id);
    }
}
