namespace DAL.Interfaces;

public interface ICategoryRepository
{
    Task<(IEnumerable<Category> Categories, int TotalCount)> GetAllAsync(int pageIndex, int pageSize);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
}
