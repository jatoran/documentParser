using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Document_Parser {
    public class ActivityWatchDay {
        public DateTime Date;
        public TimeSpan TotalDuration;

        public TimeSpan productivityDuration = new TimeSpan(00, 00, 0);
        public TimeSpan mediaDuration = new TimeSpan(00, 00, 0);
        public TimeSpan socialDuration = new TimeSpan(00, 00, 0);
        public TimeSpan programmingDuration = new TimeSpan(00, 00, 0);
        public TimeSpan gamingDuration = new TimeSpan(00, 00, 0);
        public TimeSpan OS_Duration = new TimeSpan(00, 00, 0);
        public TimeSpan chessEngineDuration = new TimeSpan(00, 00, 0);
        public TimeSpan workDuration = new TimeSpan(00, 00, 0);
        public TimeSpan uncategorizedDuration = new TimeSpan(00, 00, 0);

        public void AddData(double duration, int categoryIndex) {
            TimeSpan itemDuration = TimeSpan.FromSeconds(Convert.ToInt32((Math.Round(duration))));
            TotalDuration += itemDuration;

            //uncategorized
            if (categoryIndex == -1) {
                uncategorizedDuration += itemDuration;
            }
            //productivity
            if (categoryIndex == 0) {
                productivityDuration += itemDuration;
            }
            //media
            else if (categoryIndex == 1) {
                mediaDuration += itemDuration;
            }
            //social
            else if (categoryIndex == 2) {
                socialDuration += itemDuration;
            }
            //programming
            else if (categoryIndex == 3) {
                programmingDuration += itemDuration;
            }
            //gaming
            else if (categoryIndex == 4) {
                gamingDuration += itemDuration;
            }
            //OS
            else if (categoryIndex == 5) {
                OS_Duration += itemDuration;
            }
            //chessengine
            else if (categoryIndex == 6) {
                chessEngineDuration += itemDuration;
            }
            //work
            else if (categoryIndex == 7) {
                workDuration += itemDuration;
            }
        }

        public ActivityWatchDay(DateTime _date) {
			Date = _date;
		}
}
    public class Range {
        public DateTime startTime;
        public DateTime endTime;
        public TimeSpan duration;
        public Range(DateTime _startTime, double _duration) {
            duration = TimeSpan.FromSeconds(Convert.ToInt32((Math.Round(Convert.ToDouble(_duration)))));
            startTime = _startTime;
            endTime = _startTime + duration;
        }
    }

    public class Data {
        public string status { get; set; }
        public string app { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public bool audible { get; set; }
        public bool incognito { get; set; }
        public int tabCount { get; set; }
    }

    public class Event {
        public DateTime timestamp { get; set; }
        public double duration { get; set; }
        public Data data { get; set; }
    }

    public class AwWatcherAfkDESKTOPHM37FBL {
        public string id { get; set; }
        public DateTime created { get; set; }
        public object name { get; set; }
        public string type { get; set; }
        public string client { get; set; }
        public string hostname { get; set; }
        public List<Event> events { get; set; }
    }

    public class AwWatcherWindowDESKTOPHM37FBL {
        public string id { get; set; }
        public DateTime created { get; set; }
        public object name { get; set; }
        public string type { get; set; }
        public string client { get; set; }
        public string hostname { get; set; }
        public List<Event> events { get; set; }
    }

    public class AwWatcherWebChrome {
        public string id { get; set; }
        public DateTime created { get; set; }
        public object name { get; set; }
        public string type { get; set; }
        public string client { get; set; }
        public string hostname { get; set; }
        public List<Event> events { get; set; }
    }

    public class AwStopwatch {
        public string id { get; set; }
        public DateTime created { get; set; }
        public object name { get; set; }
        public string type { get; set; }
        public string client { get; set; }
        public string hostname { get; set; }
        public List<object> events { get; set; }
    }

    public class Buckets {
        [JsonProperty("aw-watcher-afk_DESKTOP-HM37FBL")]
        public AwWatcherAfkDESKTOPHM37FBL AwWatcherAfkDESKTOPHM37FBL { get; set; }
        [JsonProperty("aw-watcher-window_DESKTOP-HM37FBL")]
        public AwWatcherWindowDESKTOPHM37FBL AwWatcherWindowDESKTOPHM37FBL { get; set; }
        [JsonProperty("aw-watcher-web-chrome")]
        public AwWatcherWebChrome AwWatcherWebChrome { get; set; }
        [JsonProperty("aw-stopwatch")]
        public AwStopwatch AwStopwatch { get; set; }
    }

    public class Root {
        public Buckets buckets { get; set; }
    }
}
