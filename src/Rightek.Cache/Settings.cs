using Rightek.Cache.Enums;

namespace Rightek.Cache
{
    public class Settings
    {
        public string Key { get; set; }
        public long TimeToLive { get; set; }
        public ExpirationType ExpirationType { get; set; } = ExpirationType.ABSOLUTE;
        public TimeUnit TimeUnit { get; set; } = TimeUnit.MILLISECOND;
    }
}