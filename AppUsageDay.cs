using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document_Parser {
    class AppUsageDay {
        public int titleIndex = 0;
        public int dateIndex = 1;
        public int timeIndex = 2;
        public int durationIndex = 3;

        public DateTime Date_Time;
        public TimeSpan totalDayDuration;

        public List<string[]> AllItems = new List<string[]>();

        public string UncategorizedAppInfo = "";

        //index corresponds to appList array
        public List<TimeSpan> AppDurations = new List<TimeSpan>();

        public TimeSpan SocialDuration = new TimeSpan(00, 00, 0);
        public TimeSpan MediaDuration = new TimeSpan(00, 00, 0);
        public TimeSpan BrowsingDuration = new TimeSpan(00, 00, 0);
        public TimeSpan GamesDuration = new TimeSpan(00, 00, 0);
        public TimeSpan ProductivityDuration = new TimeSpan(00, 00, 0);
        public TimeSpan UncategorizedDuration = new TimeSpan(00, 00, 0);

        public AppUsageDay(DateTime _dateTime, int appTitlesLength) {
            Date_Time = _dateTime;
            //populate with blank timespans same length as standard apps list array
            for (int i = 0; i < appTitlesLength; i++) {
                AppDurations.Add(new TimeSpan());
            }
        }

        public void AddCommon(string[] item) {
            TimeSpan itemDuration = TimeSpan.Parse(item[durationIndex]);
            string itemTitle = item[titleIndex];
            AllItems.Add(new string[] { item[titleIndex], item[dateIndex], item[timeIndex], item[durationIndex] });
            totalDayDuration += itemDuration;

            //HANDLE CATEGORIES
            //Social
            if (itemTitle.Contains("Mimi") ||
                itemTitle.Contains("Messages") ||
                itemTitle.Contains("Messenger") ||
                itemTitle.Contains("Hangouts") ||
                itemTitle.Contains("Wire") ||
                itemTitle.Contains("Voice") ||
                itemTitle.Contains("Phone") ||
                itemTitle.Contains("Gmail") ||
                itemTitle.Contains("InboxIt") ||
                itemTitle.Contains("Phone")){
                SocialDuration += itemDuration;
            }
            //Media
            if (itemTitle.Contains("Spotify") ||
                itemTitle.Contains("Youtube") ||
                itemTitle.Contains("Plex") ||
                itemTitle.Contains("Kindle") ||
                itemTitle.Contains("ReadEra") ||
                itemTitle.Contains("Video Player")) {
                MediaDuration += itemDuration;
            }
            //Browsing
            if (itemTitle.Contains("Brave") ||
                itemTitle.Contains("Sync Pro") ||
                itemTitle.Contains("Inoreader") ||
                itemTitle.Contains("Firefox") ||
                itemTitle.Contains("Chrome")) {
                BrowsingDuration += itemDuration;
            }
            //Games
            if (itemTitle.Contains("Among Us") ||
                itemTitle.Contains("lichess") ||
                itemTitle.Contains("Genshin") ||
                itemTitle.Contains("Clash")) {
                GamesDuration += itemDuration;
            }
            //Productivity
            if (itemTitle.Contains("Daylio") ||
                itemTitle.Contains("InboxIt") ||
                itemTitle.Contains("Docs") ||
                itemTitle.Contains("Drive") ||
                itemTitle.Contains("AnkiDroid") ||
                itemTitle.Contains("Sheets") ||
                itemTitle.Contains("Todoist") ||
                itemTitle.Contains("Timely") ||
                itemTitle.Contains("Thing Counter") ||
                itemTitle.Contains("Sleep") ||
                itemTitle.Contains("Chess Tempo") ||
                itemTitle.Contains("KeepTrack")) {
                ProductivityDuration += itemDuration;
            }
        }

        public void AddStandardItem(string[] item, int AppListIndex) {
            AddCommon(item);
            AppDurations[AppListIndex] += TimeSpan.Parse(item[durationIndex]);
        }

        public void AddNonStandardItem(string[] item) {
            AddCommon(item);
            UncategorizedAppInfo += item[titleIndex] + "- " + item[durationIndex] + ',';
            UncategorizedDuration += TimeSpan.Parse(item[durationIndex]);
        }

        public void SortItemsByTime() {
            AllItems = AllItems
                .OrderBy(arr => arr[dateIndex] + arr[timeIndex])
                .ToList();
        }
    }
}
