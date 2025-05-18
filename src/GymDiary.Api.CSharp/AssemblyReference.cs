using System.Reflection;

namespace GymDiary.Api.CSharp;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
