namespace GymDiary.DbMigrations;

public interface IDbMigrator
{
    Task Upgrade();
}
