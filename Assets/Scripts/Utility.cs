namespace GridGameDevelopment
{
    public static class Utility
    {
        public static string TimeFormat(float time)
        {
            return $"{(int)time / 60:00}:{time % 60:00.00}";
        } 
    }
}