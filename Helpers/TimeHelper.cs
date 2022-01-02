namespace ChatZone.Helpers
{
    public static class TimeHelper
    {
        private const int Second = 1;
        private const int Minute = 60 * Second;
        private const int Hour = 60 * Minute;
        private const int Day = 24 * Hour;
        private const int Month = 30 * Day;

        public static string RelativeTime(this DateTime currentDate)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - currentDate.Ticks);
            var delta = Math.Abs(ts.TotalSeconds);

            switch (delta)
            {
                case < 1 * Minute:
                    return ts.Seconds == 1
                        ? "1 giây trước"
                        : (ts.Seconds + " giây trước");
                case < 2 * Minute:
                    return "1 phút trước";
                case < 45 * Minute:
                    return ts.Minutes + " phút trước";
                case < 90 * Minute:
                    return "1 giờ trước";
                case < 24 * Hour:
                    return ts.Hours + " giờ trước";
                case < 48 * Hour:
                    return "Hôm qua";
                case < 30 * Day:
                    return ts.Days + " ngày trước";
                case < 12 * Month:
                {
                    var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                    return months <= 1
                        ? "1 tháng trước"
                        : months + " tháng trước";
                }
                default:
                {
                    var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                    return years <= 1
                        ? "1 năm trước"
                        : years + " năm trước";
                }
            }
        }
    }
}