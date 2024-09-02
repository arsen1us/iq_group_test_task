namespace IQGROUP_test_task.Models
{
    public interface IDatabaseService<T> where T : class
    {
        public Task<T> FindByIdAsync(string id);

        public Task<List<T>> FindAllAsync();

        public Task InsertOneAsync(T obj);

        public Task DeleteAsync(string id);

        public Task UpdateAsync(T obj);

        public Task<bool> IsExistAsync(string id);
    }
}
