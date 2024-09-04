using IQGROUP_test_task.Models;

namespace IQGROUP_test_task.Services
{
    public interface IUserService : IDatabaseService<UserModel>
    {
        public Task<bool> CheckEmailAvailabilityAsync(string email);

        public Task<bool> CheckLoginAvailabilityAsync(string login);

        public Task<UserModel> FindAsync(AuthUserModel user);

        public Task UpdateAsync(string id, UserModel user);

        public Task<List<UserModel>> FindByLoginAsync(string login);
    }
}
