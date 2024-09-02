namespace IQGROUP_test_task.Models
{
    public interface IUserService : IDatabaseService<UserModel>
    {
        public Task<bool> CheckEmailAvailabilityAsync(string email);

        public Task<bool> CheckLoginAvailabilityAsync(string login);

        public Task<UserModel> FindAsync(AuthUserModel user);
    }
}
