using System.Collections.Generic;

namespace InfluxDB.Net.Models
{
    public class Shard
    {
        public int Id { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public bool LongTerm { get; set; }
        public List<Member> Shards { get; set; }

        public class Member
        {
            public List<int> ServerIds { get; set; }
        }
    }
}