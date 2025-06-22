namespace BLL.Implements;

public class GenericService<T> : IGenericService<T> where T : class
{
    protected readonly IGenericRepository<T> _repository;
    protected readonly ElectronicStoreDbContext _context;

    public GenericService(IGenericRepository<T> repository, ElectronicStoreDbContext context)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found.");
        }
        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        _repository.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found.");
        }
        _repository.Delete(entity);
        await _context.SaveChangesAsync();
    }
}