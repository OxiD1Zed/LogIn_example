using LogIn_example.Entities;
using Npgsql;
using System;
using System.Collections.Generic;

namespace LogIn_example.Providers
{
    public interface IUserProvider
    {
        User GetUser(long id);

        List<User> GetUsers();

        bool SaveUser(User user);
    }

    public class UserProvider : IUserProvider
    {
        private static UserProvider _instance;
        private NpgsqlConnection _connection;

        private UserProvider() { }

        public static UserProvider Create()
        {
            if(_instance == null)
            {
                _instance = new UserProvider();
            }
            
            return _instance;
        }

        public void InitConnection()
        {
            if(_connection == null)
            {
                _connection = new NpgsqlConnection("Server=127.0.0.1;User Id=postgres;Password=12345678;Port=5432;Database=login_example");
            }
        }

        public void OpenConnection()
        {
            if(_connection != null && _connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void CloseConnection()
        {
            if(_connection != null && _connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public User GetUser(long id)
        {
            OpenConnection();
            using (NpgsqlCommand command = _connection.CreateCommand())
            {
                command.CommandText = $"select * from \"user\" where id = {id}";
                using(NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            long userId = reader.GetInt64(reader.GetOrdinal("id"));
                            string login = reader.GetString(reader.GetOrdinal("login"));
                            string password = reader.GetString(reader.GetOrdinal("password"));

                            return new User(id: userId, password: password, login: login);
                        }

                        throw new NpgsqlException("Не удалось прочитать пользователя");
                    }

                    throw new Exception("Пользователь не найден");
                }
            }
        }

        public List<User> GetUsers()
        {
            OpenConnection();
            using(NpgsqlCommand command = _connection.CreateCommand())
            {
                command.CommandText = $"select * from \"user\"";
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        List<User> users = new List<User>();
                        while (reader.Read())
                        {
                            long userId = reader.GetInt64(reader.GetOrdinal("id"));
                            string login = reader.GetString(reader.GetOrdinal("login"));
                            string password = reader.GetString(reader.GetOrdinal("password"));
                            users.Add(new User(id: userId, password: password, login: login));
                        }

                        return users;
                    }

                    throw new Exception("Пользователи не найдены");
                }
            }
        }

        private bool IsRegistrationUser(string login)
        {
            OpenConnection();
            using (NpgsqlCommand command = _connection.CreateCommand())
            {
                command.CommandText = $"select * from \"user\" where login = \'{login}\'";
                using(NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }

                    return false;
                }
            }         
        }

        public bool SaveUser(User user)
        {
            OpenConnection();
            using (NpgsqlCommand command = _connection.CreateCommand())
            {
                if (!IsRegistrationUser(user.login))
                {
                    command.CommandText = $"insert into \"user\" (login, password) values (\'{user.login}\', \'{user.password}\')";
                    int result = command.ExecuteNonQuery();

                    return result == 1;
                }
            }
                
            throw new Exception("Пользователь уже зарегистрирован");
        }
    }
}
