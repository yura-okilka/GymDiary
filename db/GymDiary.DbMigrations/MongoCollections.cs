namespace GymDiary.DbMigrations;

public static class MongoCollections
{
    public const string Users = "users";
    public const string ExerciseCategories = "exerciseCategories";
    public const string ExerciseDefinitions = "exerciseDefinitions";
    public const string Routines = "routines";
    public const string Workouts = "workouts";

    public static IEnumerable<string> All => [Users, ExerciseCategories, ExerciseDefinitions, Routines, Workouts];
}
