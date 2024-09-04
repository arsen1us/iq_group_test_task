using DnsClient;
using IQGROUP_test_task.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IQGROUP_test_task.Services
{
    public class UserService : IUserService
    {
        IMongoClient _client;
        IMongoDatabase _database;
        IMongoCollection<UserModel> _usersCollection;
        ILogger<UserService> _logger;
        IDateTimeService _dateTimeService;

        public UserService(IMongoClient client, IConfiguration config, ILogger<UserService> logger, IDateTimeService dateTimeService)
        {
            _client = client;
            _database = _client.GetDatabase(config["MongoDbSettings:DbName"]);
            _usersCollection = _database.GetCollection<UserModel>("users");
            _logger = logger;
            _dateTimeService = dateTimeService;
        }
        // Аутентификация пользователя

        public async Task<UserModel> FindAsync(AuthUserModel authUser)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.Email, authUser.Email)
                    & Builders<UserModel>.Filter.Eq(u => u.Password, authUser.Password);
                var cursor = await _usersCollection.FindAsync(filter);

                _logger.LogInformation($"INFO: [{timestamp}]  Successfully request to find user by email - [{authUser.Email}] and password - [{authUser.Password}]");
                return await cursor.FirstOrDefaultAsync();

            }
            catch (MongoCursorNotFoundException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] MongoDb cursor error. Details - {ex.Message}");
                throw new Exception();
            }
            catch (MongoQueryException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error processing request to MondoDb server. Details - {ex.Message}");
                throw new Exception();
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception();
            }

        }
        // Получить всех пользователей

        public async Task<List<UserModel>> FindAllAsync()
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var filter = Builders<UserModel>.Filter.Empty;
                var cursor = await _usersCollection.FindAsync(filter);

                _logger.LogInformation($"INFO: [{timestamp}] Successfully request to find all users");
                return await cursor.ToListAsync();
            }
            catch (MongoCursorNotFoundException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] MongoDb cursor error. Details - {ex.Message}");
                throw new Exception();
            }
            catch (MongoQueryException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error processing request to MondoDb server. Details - {ex.Message}");
                throw new Exception();
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception();
            }

        }
        // Получить пользователя по id

        public async Task<UserModel> FindByIdAsync(string id)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u._id, id);
                var cursor = await _usersCollection.FindAsync(filter);

                _logger.LogInformation($"INFO: [{timestamp}] Successfully request to find user by id - [{id}]");
                return await cursor.FirstOrDefaultAsync();
            }
            catch (MongoCursorNotFoundException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] MongoDb cursor error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoQueryException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error processing request to MondoDb server. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        // Поиск пользователя по части логину
        public async Task<List<UserModel>> FindByLoginAsync(string login)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var filter = Builders<UserModel>.Filter.Regex(u => u.Login, new BsonRegularExpression(login, "i"));
                var cursor = await _usersCollection.FindAsync(filter);

                _logger.LogInformation($"INFO: [{timestamp}] Successfully request to find users by login");
                return await cursor.ToListAsync();
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoCommandException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Request to MongoDb Server Error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoQueryException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error processing request to MondoDb server. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }

            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }

        }

        // Добавить пользователя

        public async Task InsertOneAsync(UserModel user)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();
            try
            {
                await _usersCollection.InsertOneAsync(user);
                _logger.LogInformation($"INFO: [{timestamp}] User with id - [{user._id}] successfully added to database");
            }
            catch (MongoDuplicateKeyException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Duplicate key in MongoDb server. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoWriteException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error writing to MongoDb. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAsync(string _id)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u._id, _id);
                var deletedResult = await _usersCollection.DeleteOneAsync(filter);

                _logger.LogInformation($"INFO: [{timestamp}] Successfully request to delete user from MondoDb, id - [{_id}]");
            }
            catch (MongoWriteException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error writing to MongoDb. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        // Изменение данных пользователя
        // Надо убрать

        public Task UpdateAsync(UserModel updUser)
        {
            throw new Exception();
        }

        // Изменение данных пользователя

        public async Task UpdateAsync(string id, UserModel updUser)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var user = await FindByIdAsync(updUser._id);
                if (user.Login != updUser.Login || user.Password != updUser.Password || user.Password != updUser.Password)
                {
                    var filter = Builders<UserModel>.Filter.Eq(u => u._id, updUser._id);

                    var update = Builders<UserModel>.Update
                        .Set(u => u.Login, updUser.Login)
                        .Set(u => u.Email, updUser.Email)
                        .Set(u => u.Email, updUser.Email);

                    await _usersCollection.UpdateOneAsync(filter, update);

                    _logger.LogInformation($"INFO: [{timestamp}] Successfully request to update user information, id - [{id}]");
                }
            }
            catch (MongoWriteException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error writing to MongoDb. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        // Проверка, существует ли пользователь с данным id

        public async Task<bool> IsExistAsync(string id)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u._id, "id");
                var cursor = await _usersCollection.FindAsync(filter);

                _logger.LogInformation($"INFO: [{timestamp}] Successfully request to check user existing, id - [{id}]");
                return await cursor.FirstOrDefaultAsync() != null;
            }

            catch (MongoCursorNotFoundException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] MongoDb cursor error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoQueryException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error processing request to MondoDb server. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        // Проверка на уникальность почты

        public async Task<bool> CheckEmailAvailabilityAsync(string email)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var emailFilter = Builders<UserModel>.Filter.Eq(u => u.Email, email);
                var cursor = await _usersCollection.FindAsync(emailFilter);

                _logger.LogInformation($"INFO: [{timestamp}] Successfully request to check login - [{email}] availability");
                return await cursor.FirstOrDefaultAsync() == null;
            }
            catch (MongoCursorNotFoundException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] MongoDb cursor error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoQueryException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error processing request to MondoDb server. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }

        }
        // Проверка на уникальность логина

        public async Task<bool> CheckLoginAvailabilityAsync(string login)
        {
            string timestamp = _dateTimeService.GetDateTimeNow();

            try
            {
                var loginFilter = Builders<UserModel>.Filter.Eq(u => u.Login, login);
                var cursor = await _usersCollection.FindAsync(loginFilter);

                _logger.LogInformation($"INFO: [{timestamp}] Successfully request to check login - [{login}] availability");
                return await cursor.FirstOrDefaultAsync() == null;


            }
            catch (MongoCursorNotFoundException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] MongoDb cursor error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoQueryException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Error processing request to MondoDb server. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (MongoConnectionException ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] Connecting to MondoDb server error. Details - {ex.Message}");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR: [{timestamp}] An internal server error occurred. Details: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
