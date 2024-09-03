
using MongoDB.Bson;
using MongoDB.Driver;

namespace IQGROUP_test_task.Models
{
    public class UserService : IUserService
    {
        IMongoClient _client;
        IMongoDatabase _database;
        IMongoCollection<UserModel> _usersCollection;
        ILogger<UserService> _logger;

        public UserService(IMongoClient client, IConfiguration config, ILogger<UserService> logger)
        {
            _client = client;
            _database = _client.GetDatabase(config["MongoDbSettings:DbName"]);
            _usersCollection = _database.GetCollection<UserModel>("users");
            _logger = logger;
        }
        // Аутентификация пользователя

        public async Task<UserModel> FindAsync(AuthUserModel authUser)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u.Email, authUser.Email)
                    & Builders<UserModel>.Filter.Eq(u => u.Password, authUser.Password);
                var cursor = await _usersCollection.FindAsync(filter);
                UserModel user = await cursor.FirstOrDefaultAsync();
                if (user != null)
                {
                    // log
                    return user;
                }
                else
                {
                    // log
                    return null;
                }
            }
            catch (MongoCursorNotFoundException ex)
            {
                // log
                // Обработка ошибок, связанных с курсором
                throw new Exception();
            }
            catch (MongoQueryException ex)
            {
                // Обработка ошибок запроса
                throw new Exception();
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
                throw new Exception();
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
                throw new Exception();
            }

        }
        // Получить всех пользователей

        public async Task<List<UserModel>> FindAllAsync()
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Empty;
                var cursor = await _usersCollection.FindAsync(filter);
                return await cursor.ToListAsync();
            }
            catch (MongoCursorNotFoundException ex)
            {
                // Обработка ошибок, связанных с курсором
                throw new Exception();
            }
            catch (MongoQueryException ex)
            {
                // Обработка ошибок запроса
                throw new Exception();
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
                throw new Exception();
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
                throw new Exception();
            }

        }
        // Получить пользователя по id

        public async Task<UserModel> FindByIdAsync(string id)
        {      
            try
            {
                if (await IsExistAsync(id))
                {
                    throw new Exception();
                }
                else
                {
                    throw new Exception();
                }
                // Логика получения документа
            }
            catch (MongoCursorNotFoundException ex)
            {
                // Обработка ошибок, связанных с курсором
                throw new Exception();
            }
            catch (MongoQueryException ex)
            {
                // Обработка ошибок запроса
                throw new Exception();
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
                throw new Exception();
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
                throw new Exception();
            }
        }
        // Добавить пользователя

        public async Task InsertOneAsync(UserModel user)
        {
            try
            {
                await _usersCollection.InsertOneAsync(user);
                _logger.LogInformation("Get method is success!");
                //return Ok();
            }
            catch (MongoDuplicateKeyException ex)
            {
                // Обработка ошибки уникального ключа (например, при дублировании _id)
            }
            catch (MongoWriteException ex)
            {
                // Обработка общих ошибок записи
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
            }
        }

        public async Task DeleteAsync(string _id)
        {
            try
            {
                // log
                var id = new ObjectId(_id);
                var filter = Builders<UserModel>.Filter.Eq(u => u._id, id.ToString());
                var deletedResult = await _usersCollection.DeleteOneAsync(filter);
                
                if(deletedResult.DeletedCount > 0)
                {

                }
                else
                {

                }
            }
            catch (MongoWriteException ex)
            {
                // Обработка общих ошибок записи, которые могут возникнуть при удалении
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
            }
            catch(ArgumentNullException ex)
            {

            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
            }
        }

        public Task UpdateAsync(UserModel obj)
        {
            throw new NotImplementedException();
            try
            {
                // Логика обновления документа
            }
            catch (MongoWriteException ex)
            {
                // Обработка общих ошибок записи, включая нарушение ограничений
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
            }
        }
        // Проверка, существует ли пользователь с данным id

        public async Task<bool> IsExistAsync(string id)
        {
            try
            {
                var filter = Builders<UserModel>.Filter.Eq(u => u._id, "id");
                var cursor = await _usersCollection.FindAsync(filter);
                if (cursor.FirstOrDefault() == null)
                    return false;
                return true;
            }

            catch (MongoCursorNotFoundException ex)
            {
                // Обработка ошибок, связанных с курсором
                return true;
            }
            catch (MongoQueryException ex)
            {
                // Обработка ошибок запроса
                return true;
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
                return true;
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
                return true;
            }
        }
        // Проверка на уникальность почты

        public async Task<bool> CheckEmailAvailabilityAsync(string email)
        {
            try
            {
                var emailFilter = Builders<UserModel>.Filter.Eq(u => u.Email, email);
                var cursor = await _usersCollection.FindAsync(emailFilter);
                if (cursor.FirstOrDefault() == null)
                    return true;
                return false;
            }
            catch (MongoCursorNotFoundException ex)
            {
                // Обработка ошибок, связанных с курсором
                return false;
            }
            catch (MongoQueryException ex)
            {
                // Обработка ошибок запроса
                return false;
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
                return false;
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
                return false;
            }

        }
        // Проверка на уникальность логина

        public async Task<bool> CheckLoginAvailabilityAsync(string login)
        {
            try
            {
                var loginFilter = Builders<UserModel>.Filter.Eq(u => u.Login, login);
                var cursor = await _usersCollection.FindAsync(loginFilter);
                if (cursor.FirstOrDefault() == null)
                    return true;
                return false;
            }
            catch (MongoCursorNotFoundException ex)
            {
                // Обработка ошибок, связанных с курсором
                return false;
            }
            catch (MongoQueryException ex)
            {
                // Обработка ошибок запроса
                return false;
            }
            catch (MongoConnectionException ex)
            {
                // Обработка ошибок соединения с базой данных
                return false;
            }
            catch (Exception ex)
            {
                // Обработка всех остальных исключений
                return false;
            }
        }
    }
}
