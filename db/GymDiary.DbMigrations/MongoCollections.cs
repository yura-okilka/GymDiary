namespace GymDiary.DbMigrations;

public static class MongoCollections
{
    public const string ExerciseCategories = "exerciseCategories";
    public const string Exercises = "exercises";
    public const string Routines = "routines";
    public const string WorkoutSessions = "workoutSessions";
    public const string Users = "users";

    public static IEnumerable<string> All => [ExerciseCategories, Exercises, Routines, WorkoutSessions, Users];
}
