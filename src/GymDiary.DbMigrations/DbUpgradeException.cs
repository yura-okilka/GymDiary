namespace GymDiary.DbMigrations;

public class DbUpgradeException(string database, Exception innerException)
    : Exception($"'{database}' database upgrade error", innerException)
{
    public string Database { get; } = database;
}
