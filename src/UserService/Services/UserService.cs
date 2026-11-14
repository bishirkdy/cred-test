using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services
{
    public class UserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public List<User> GetAll() => _repository.GetAll();

        public User? GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID");

            return _repository.GetById(id);
        }

        public void Add(User? user)
        {
            if (user is null || string.IsNullOrWhiteSpace(user.Name) ||
                string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Invalid user");

            if (_repository.GetByEmail(user.Email) is not null)
                throw new InvalidOperationException("User already exists");

            _repository.Add(user);
        }

        public void Update(User? user)
        {
            if (user is null || user.Id <= 0)
                throw new ArgumentException("Invalid user");

            if (_repository.GetById(user.Id) is null)
                throw new KeyNotFoundException("User not found");

            _repository.Update(user);
        }

        public void Remove(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID");

            if (_repository.GetById(id) is null)
                throw new KeyNotFoundException("User not found");

            _repository.Delete(id);
        }
    }
}
