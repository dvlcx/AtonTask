namespace AtonTask.DAL.Repositories
{
    public class AbstractRepository
    {
        protected readonly TaskDbContext _dbContext;

        public AbstractRepository(TaskDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}