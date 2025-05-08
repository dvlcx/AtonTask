using AtonTask.Exceptions;
using AtonTask.Models.InputModels;
using AtonTask.Models.OutputModels;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace AtonTask.DAL.Repositories
{
    public class UserRepository : AbstractRepository, IUserRepository
    {
        public UserRepository(TaskDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User> CreateUserAsync(UserCreateDto dto, string createdBy)
        {
            User user = new User 
                { 
                    Guid = Guid.NewGuid(),            
                    Login = dto.Login,
                    Name = dto.Name,
                    Password = dto.Password,
                    Gender = dto.Gender,
                    Birthday = dto.Birthday,
                    Admin = dto.IsAdmin,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByCredentialsAsync(string login, string password)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
        }

        public async Task<UserResponseDto?> GetUserDtoByLoginAsync(string login)
        {
            var user = await this.GetUserByLoginAsync(login);
            var userDto = new UserResponseDto{
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday,
                RevokedOn = user.RevokedOn,
            };
            return userDto;
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbContext.Users
                .Where(x => x.RevokedOn == null)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersOlderThanAsync(int age)
        {
            return await _dbContext.Users
                .Where(x => x.Birthday < DateTime.Now.AddYears(-age))
                .ToListAsync();
        }

        public async Task UpdateUserAsync(string login, string modifiedBy, UserUpdateDto dto)
        {
            var user = await GetUserByLoginAsync(login);
            
            this.ValidateRevocation(user.Login, user.RevokedOn);

            if (dto.Name is not null) user.Name = dto.Name;
            if (dto.Birthday.HasValue) user.Birthday = dto.Birthday;
            if (dto.Gender.HasValue) user.Gender = dto.Gender.Value;
            user.ModifiedBy = modifiedBy;
            user.ModifiedOn = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePasswordAsync(string login, string modifiedBy, string newPassword)
        {
            var user = await GetUserByLoginAsync(login);

            this.ValidateRevocation(user.Login, user.RevokedOn);

            user.Password = newPassword;
            user.ModifiedBy = modifiedBy;
            user.ModifiedOn = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateLoginAsync(string login, string modifiedBy, string newLogin)
        {
            var user = await GetUserByLoginAsync(login);

            this.ValidateRevocation(user.Login, user.RevokedOn);

            user.Login = newLogin;
            user.ModifiedBy = modifiedBy;
            user.ModifiedOn = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        public async Task SoftDeleteUserAsync(string login, string revokedBy)
        {
            var user = await GetUserByLoginAsync(login);

            user.RevokedBy = revokedBy;
            user.RevokedOn = DateTime.Now;

            await _dbContext.SaveChangesAsync();        
        }

        public async Task RestoreUserAsync(string login)
        {
            var user = await GetUserByLoginAsync(login);

            user.RevokedBy = null;
            user.RevokedOn = null;

            await _dbContext.SaveChangesAsync();        
        }

        public async Task HardDeleteUserAsync(string login)
        {
            var user = await GetUserByLoginAsync(login);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Login == login)
                ?? throw new KeyNotFoundException("No user with such login!");
        }

        private void ValidateRevocation(string login, object? revocationData)
        {
            if (revocationData is not null)
                throw new UserRevokedException(login);
        }
    }
}