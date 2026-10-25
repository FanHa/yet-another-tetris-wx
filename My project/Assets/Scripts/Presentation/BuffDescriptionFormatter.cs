namespace Presentation
{
    public static class BuffDescriptionFormatter
    {
        public static string Format(params string[] lines) => string.Join("\n", lines);
    }
}