namespace Dal;

internal static class Config
{
    internal const int startCourseId = 1000;
    private static int nextCourseId = startCourseId;
    internal static int NextCourseId { get => nextCourseId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;
}
