using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; //opening files for reading/writing
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Document_Parser {
    public partial class Form1 : Form {
        string[] filePathTitles = { "sleep", "daylio","weight","KeepTrack","AUM","buckets"};

        public Form1() {
            InitializeComponent();
            
            //drag and drop - associate two event handlers with control events
            this.listBox1.DragDrop += new
                System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
            this.listBox1.DragEnter += new
                 System.Windows.Forms.DragEventHandler(this.listBox1_DragEnter);
        }

        //handle dragEnter Event
        private void listBox1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }
        //handle DragDrop event
        private void listBox1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
            listBox1.Items.Clear();
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int i;
            for (i = 0; i < s.Length; i++)
                listBox1.Items.Add(s[i]);
        }

        private void button1_Click(object sender, EventArgs e) {
            Task.Run(new Action(identifyFileToParse));
        }

        private void button2_Click(object sender, EventArgs e) {
            //bool boxChecked = false;
            //if (checkBox1.Checked) {
            //    boxChecked = true;
            //}
            
            var newline = System.Environment.NewLine;
            var tab = "\t";
            var clipboard_string = new StringBuilder();

            //code to append column headers to top of output
            //var arrayNames = (from DataColumn x in dataGridView1.Columns
            //                  select x.ColumnName).ToArray();
            //for (int i = 0; i < arrayNames.Length; i++) {
            //    clipboard_string.Append(row.);
            //}

            foreach (DataGridViewRow row in dataGridView1.Rows) {
                for (int i = 0; i < row.Cells.Count; i++) {
                    if (i == (row.Cells.Count - 1))
                        clipboard_string.Append(row.Cells[i].Value + newline);
                    else
                        clipboard_string.Append(row.Cells[i].Value + tab);
                }
            }

            Clipboard.SetText(clipboard_string.ToString());

        }

        public void identifyFileToParse() {
            string filePath = listBox1.Items[0].ToString();
            if (filePath.Contains(filePathTitles[0])) {
                parseSleepCSV(parseCSV_ToLines(filePath));
            }
            else if (filePath.Contains(filePathTitles[1])) {
                parseDaylioCSV(parseCSV_ToLines(filePath));
            }
            else if (filePath.Contains(filePathTitles[2])) {
                parseWeightCSV(parseCSV_ToLines(filePath));
            }
            else if (filePath.Contains(filePathTitles[3])) {
                parseKeeptrackCSV(parseCSV_ToLines(filePath));
            }
            else if (filePath.Contains(filePathTitles[4])) {
                parseAppUsageCSV(parseCSV_ToLines(filePath));
            }
            else if (filePath.Contains(filePathTitles[5])) {
                parseActivityWatchJSON(filePath);
            }
            else Console.WriteLine("Error: File name does not match accept file title parameters");
        }

        public string[] parseCSV_ToLines(string filePath) {
            var csv = File.ReadAllLines(filePath);

            if (filePath.Contains(filePathTitles[0])) {
                csv = csv
                    .Where(x => !x.StartsWith(",,") && !x.StartsWith("Id"))
                    .ToArray();
            }
            else if (filePath.Contains(filePathTitles[1])) {
                csv = csv
                    .Where(x => !x.StartsWith("full"))
                    .ToArray();
            }
            else if (filePath.Contains(filePathTitles[2])) {
                csv = csv
                    .Where(x => !x.StartsWith("Weight"))
                    .ToArray();
            }
            else if (filePath.Contains(filePathTitles[3])) {
                csv = csv
                    .Where(x => !x.StartsWith("Trackers"))
                    .ToArray();
            }
            else if (filePath.Contains(filePathTitles[4])) {
                csv = csv
                    .Where(x => !x.StartsWith("\"App name") && !x.StartsWith("\"Activity history") && !x.StartsWith("\"Created by") && !x.StartsWith("\"Screen on") && !x.StartsWith("\"Screen off") && !x.StartsWith("\"Device shutdown") && !x.StartsWith("\"Device boot") && !x.StartsWith("\"System updates") && !x.StartsWith("\"\"") && !x.StartsWith("\n"))
                    .ToArray();
            }
            else if (filePath.Contains(filePathTitles[5])) {
                csv = csv
                    .Where(x => !x.StartsWith("id,time"))
                    .ToArray();
            }

            return csv;
        }

        public void removeStringArrayQuotes(string[] csv) {
            for (int i = 0; i < csv.Length; i++) {
                if (csv[i].Contains("\"")) {
                    csv[i] = csv[i].Replace("\"", "");
                }
            }
        }

        public List<string[]> splitStringArrayByDelimiter(string[] csv, char delimiter) {
            List<string[]> strings = new List<string[]>();
            for (int i = 0; i < csv.Length; i++) {
                strings.Add(csv[i].Split(delimiter));
            }
            return strings;
        }

        public void populateDataTableHeaders(string[] headers, DataTable table) {
            foreach (string headerString in headers) {
                table.Columns.Add(headerString);
            }
        }

        public void parseActivityWatchJSON(string filePath) {
            string[] activityWatchColumnHeaders = { "Date", "Total Time", "Programming", "Media","Social", "Productivity", "Gaming", "OS", "ChessEngine", "Work", "Uncategorized"};
            string[] productivityEventTitles = { "Google Docs", "Sublime", "Google Sheets", "Todoist", "Wikipedia", "Inoreader", "Anki", "Khan", "Biology", "Chemistry" };
            string[] mediaEventTitles = { "YouTube", "Plex", "Pornhub", "Xhamster", "Xvideo", "bing", "VLC", "Spotify" };
            string[] socialEventTitles = { "reddit", "Facebook", "Twitter", "Instagram", "Discord", "4chan", "Messenger", "Telegram", "Signal", "WhatsApp", "Rambox", "Gmail" };
            string[] programmingEventTitles = { "GitHub", "Stack Overflow", "stackoverflow", "repl", "repl.it", "devenv.exe", "commandLine - Microsoft Visual Studio", "VsDebugConsole.exe", "Oceans", "learncpp", "Learn C", "csharp", "csharp.net-tutorials.com", "C#", "Document_Parser", "Requester", "API" };
            string[] gamingEventTitles = { "Minecraft", "RimWorld", "SC2", "Battle", "Witcher", "Distance Incremental", "jacorb90", "DistInc", "factorio", "FallGuys", "UDK", "Rocket", "Raft", "starcraft", "star craft", "broodwar", "brood war", "Warcraft", "World of Warcraft", "Overwatch", "Age of Empires", "Empire Earth", "Skynet" };
            string[] OS_EventTitles = { "explorer", "New Tab", "Photos", "Sound" };
            string[] chessEngineEventTitles = { "Arena", "Gameknot", "(1500) —", "— (1500)", "Arena.exe", "Chess Game" };
            string[] workEventTitles = { "eBay", "Google Ads", "USPS", "Mailchimp" };

            List<string[]> allEventTitles = new List<string[]>(){ programmingEventTitles, mediaEventTitles, socialEventTitles, productivityEventTitles,  gamingEventTitles, OS_EventTitles, chessEngineEventTitles, workEventTitles };

            int columnsTotalLength = workEventTitles.Length + chessEngineEventTitles.Length + OS_EventTitles.Length + gamingEventTitles.Length + programmingEventTitles.Length + socialEventTitles.Length + mediaEventTitles.Length + productivityEventTitles.Length + activityWatchColumnHeaders.Length;

            //string filePath = "C:\\Users\\Heisenberg\\Desktop\\aw-buckets-export 12-15-2020.json";

            List<Event> nonAfkEvents = new List<Event>();

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(filePath)) {
                JsonSerializer serializer = new JsonSerializer();
                Root JSON_Data = (Root)serializer.Deserialize(file, typeof(Root));

                List<Range> afkTimes = JSON_Data.buckets.AwWatcherAfkDESKTOPHM37FBL.events
                    .Where(x => x.data.status == "afk")
                    .Select(y => new Range(y.timestamp, y.duration)).ToList();

                List<Range> activeTimes = JSON_Data.buckets.AwWatcherAfkDESKTOPHM37FBL.events
                    .Where(x => x.data.status == "not-afk")
                    .Select(y => new Range(y.timestamp, y.duration)).ToList();

                List<Event> allEvents = JSON_Data.buckets.AwWatcherWindowDESKTOPHM37FBL.events
                    .Where(x => !x.data.app.Contains("chrome"))
                    .Concat(JSON_Data.buckets.AwWatcherWebChrome.events)
                    .OrderBy(y => y.timestamp)
                    .ToList();

                activeTimes.Reverse();
                afkTimes.Reverse();

                //foreach(Event asdf in allEvents) {
                //    Console.WriteLine(asdf.timestamp);
                //}

                //dedupe allEvents
                //for (int i = 0; i < allEvents.Count; i++) {
                //    if ()
                //}

                //populate nonAfkEvents events that dont overlap with afk times
                int i = 0;
                foreach (Event e in allEvents) {
                    while (e.timestamp > activeTimes[i].endTime) {
                        i++;
                    }
                    if (e.timestamp < activeTimes[i].startTime) {
                        continue;
                    }
                    if (e.timestamp >= activeTimes[i].startTime && e.timestamp <= activeTimes[i].endTime) {
                        nonAfkEvents.Add(e);
                        continue;
                    }
                }
            }

            //Create and populate days list
            List<ActivityWatchDay> days = new List<ActivityWatchDay>();
            TimeSpan dayHourTermination = new TimeSpan(07, 00, 0);
            DateTime startDate = nonAfkEvents[0].timestamp.AddDays(-1).Date + dayHourTermination;
            DateTime endDate = nonAfkEvents[nonAfkEvents.Count - 1].timestamp.AddDays(1);
            DateTime currDate = startDate;
            while (currDate <= endDate) {
                days.Add(new ActivityWatchDay(currDate));
                currDate = currDate.Date.AddDays(1) + dayHourTermination;
            }

            foreach(Event ev in nonAfkEvents) {
                int currDayIndex = 0;
                DateTime tempDateTime = ev.timestamp;
                //Console.WriteLine(ev.timestamp);

                bool infoAdded = false;
                while (infoAdded == false) {
                    DateTime currDay = days[currDayIndex].Date;
                    //if timestamp is within current day
                    if (tempDateTime >= currDay && tempDateTime < currDay.AddDays(1)) {
                        
                        bool contains = false;
                        for (int i = 0; i < columnsTotalLength; i++) {
                            int categoryIndex = 0;
                            foreach (string[] categoryTitle in allEventTitles) {
                                for (int j = 0; j < categoryTitle.Length; j++) {
                                    if (ev.data.title.Contains(categoryTitle[j])) {
                                        days[currDayIndex].AddData(ev.duration, categoryIndex);
                                        Console.WriteLine(days[currDayIndex].Date + " - " + days[currDayIndex].TotalDuration + " Added: " + ev.data.title + " - " + ev.duration);
                                        contains = true;
                                        infoAdded = true;
                                    }
                                    else if (ev.data.app != null && contains != true && infoAdded != true) {  
                                        if (ev.data.app.Contains(categoryTitle[j])) {
                                            days[currDayIndex].AddData(ev.duration, categoryIndex);
                                            Console.WriteLine(days[currDayIndex].Date + " - " + days[currDayIndex].TotalDuration + " Added: " + ev.data.app + " - " + ev.duration);

                                            contains = true;
                                            infoAdded = true;
                                        }
                                    }
                                    
                                    if (contains == true && infoAdded == true) {
                                        break;
                                    }
                                }
                                if (contains == true && infoAdded == true) {
                                    break;
                                }
                                categoryIndex++;
                            }
                            if (contains == true && infoAdded == true) {
                                break;
                            }
                        }
                        if (contains == false) {
                            days[currDayIndex].AddData(ev.duration, -1);
                            infoAdded = true;
                        }
                    }
                    else {
                        currDayIndex++;
                    }
                }
            }

            Console.WriteLine("fin");

            //DataTable table = new DataTable();

            //foreach (string headerString in activityWatchColumnHeaders) {
            //    table.Columns.Add(headerString);
            //}

            //foreach (ActivityWatchDay day in days) {
            //    table.Rows.Add(day.Date, day.TotalDuration, day.productivityDuration, day.mediaDuration, day.socialDuration, day.programmingDuration, day.gamingDuration, day.OS_Duration, day.chessEngineDuration, day.workDuration, day.uncategorizedDuration);
            //}

            //dataGridView1.Invoke(new MethodInvoker(() =>
            //    dataGridView1.DataSource = table)
            //            );
        }

        public void parseAppUsageCSV(string[] csv) {
            string[] appTitlesList = { "Daylio", "Hangouts", "Gmail", "Sync Pro", "Plex",  "Sleep", "Thing Counter", "Chrome", "InboxIt", "Brave", "YouTube", "Timely", "Wire", "Call", "Todoist", "lichess", "AnkiDroid", "Messages", "KeepTrack", "Mimi", "Spotify", "Calendar", "Chess Tempo", "Messenger", "Sheets", "Drive", "Phone", "Voice", "Inoreader", "Among Us", "Kindle", "ReadEra", "Genshin Impact", "Docs", "Firefox", "Video Player"};

            //populate column headers
            List<string> appUsageColumnHeaders = new List<string>();
            appUsageColumnHeaders.Add("Date");
            appUsageColumnHeaders.Add("Total Time");
            appUsageColumnHeaders.Add("Total Social");
            appUsageColumnHeaders.Add("Total Media");
            appUsageColumnHeaders.Add("Total Browsing");
            appUsageColumnHeaders.Add("Total Games");
            appUsageColumnHeaders.Add("Total Productivity");
            appUsageColumnHeaders.Add("Total Uncategorized");
            appUsageColumnHeaders.Add("Uncategorized Info");
            foreach (string str in appTitlesList) {
                appUsageColumnHeaders.Add(str);
            }

            removeStringArrayQuotes(csv);
            List<string[]> csv_Rows = splitStringArrayByDelimiter(csv, ',');
            csv_Rows.Reverse();

            //Create and populate Days List
            List<AppUsageDay> days = new List<AppUsageDay>();
            DateTime startDate = DateTime.Parse(csv_Rows[0][1]) + new TimeSpan(7,00,0);
            DateTime endDate = DateTime.Parse(csv_Rows[csv_Rows.Count - 1][1]);
            DateTime currDate = startDate;
            while (currDate <= endDate) {
                days.Add(new AppUsageDay(currDate, appTitlesList.Length));
                currDate = currDate.AddDays(1);
            }

            foreach (string[] row in csv_Rows) {
                int currDayIndex = 0;
                DateTime tempDateTime = DateTime.Parse(row[1] + " " + row[2]);

                bool infoAdded = false;
                while (infoAdded == false && currDayIndex < days.Count) {
                    DateTime currDay = days[currDayIndex].Date_Time;
                    if (tempDateTime > currDay && tempDateTime < currDay.AddDays(1)) {
                        bool contains = false;
                        for (int i = 0; i < appTitlesList.Length; i++) {
                            if (row[0].Contains(appTitlesList[i])) {
                                contains = true;
                                days[currDayIndex].AddStandardItem(row, i);
                                infoAdded = true;
                                break;
                            }
                        }
                        if (contains == false) {
                            days[currDayIndex].AddNonStandardItem(row);
                            infoAdded = true;
                        }
                    }
                    else {
                        currDayIndex++;
                    }
                }
            }

            DataTable table = new DataTable();

            foreach (string headerString in appUsageColumnHeaders) {
                table.Columns.Add(headerString);
            }

            foreach (AppUsageDay day in days) {
                DataRow tempRow;
                tempRow = table.NewRow();
                tempRow.SetField(0, day.Date_Time.ToString("MM/dd/yyyy"));
                tempRow.SetField(1, day.totalDayDuration);
                tempRow.SetField(2, day.SocialDuration);
                tempRow.SetField(3, day.MediaDuration);
                tempRow.SetField(4, day.BrowsingDuration);
                tempRow.SetField(5, day.GamesDuration);
                tempRow.SetField(6, day.ProductivityDuration);
                tempRow.SetField(7, day.UncategorizedDuration);
                tempRow.SetField(8, day.UncategorizedAppInfo);
                for (int i = 0; i < day.AppDurations.Count; i++) {
                    tempRow.SetField(i + 9, day.AppDurations[i]);
                }
                table.Rows.Add(tempRow);
            }

            dataGridView1.Invoke(new MethodInvoker(() =>
                dataGridView1.DataSource = table)
                        );
        }

        public int getNthIndex(string str, char charToFind, int nthInstance) {
            int count = 0;
            for (int i = 0; i < str.Length; i++) {
                if (str[i] == charToFind) {
                    count++;
                    if (count == nthInstance) {
                        return i;
                    }
                }
            }
            return -1;
        }

        public string[] _formatDreams(string[] lines) {
            //dependant on hard-coding the endRowText = the category that comes after dream log
            string startRowText = "Dream Log";
            string endRowText = "Dual N-Back";
            int i = 0;
            int targetLine = 0;
            while (i < lines.Length) {
                //find dream log start
                if (lines[i].Contains(startRowText)) {
                    i++;
                    //run until next section starts
                    while (!lines[i].StartsWith(endRowText)) {
                        //remove all quotes in all lines
                        if (lines[i].Contains("\"")) {
                            lines[i] = lines[i].Replace("\"", "");
                        }

                        //if comma count is >2, replace all extra instances
                        int count = lines[i].Count(f => f == ',');
                        if (count > 2) {
                            int thirdIndex = getNthIndex(lines[i], ',', 3);
                            string tempString = lines[i].Substring(thirdIndex);
                            lines[i] = lines[i].Remove(thirdIndex);
                            tempString = tempString.Replace(",", ";");
                            lines[i] = lines[i] + tempString;
                        }

                        //when find blank line
                        if (lines[i] == "") {
                            i++;
                            continue;
                        }
                        
                        //when find next date item
                        if (Char.IsNumber(lines[i][0]) && lines[i].Contains("/")) { 
                            targetLine = i;
                            i++;
                            continue;
                        }

                        else {
                            //replace all commas in lines that dont contain a date
                            lines[i] = lines[i].Replace(",", " | ");

                            //add lines with content to target line and clear them
                            lines[targetLine] = lines[targetLine] + " | " + lines[i];
                            lines[i] = "";
                            
                            i++;
                        }
                    }
                    break;
                }
                i++;
            }
            return lines;
        }

        public void parseKeeptrackCSV(string[] csv) {
            string[] keepTrackColumnHeaders = { "Date/Time", "Breakfast", "Brush Left Hand", "Chess Tactics", "Cold Shower", "Dual N-Back", "Fish Oil", "Floss", "FlossBlood", "SleptBedLastNight", "Spaced Repetition", "Meditate", "Meditation Time", "Meditation Notes", "Read Book", "Read Time", "ExerciseCount", "ExerciseTags", "ExerciseNotes", "SmokeCount", "SmokeTimes", "SmokeNotes", "HeadacheCount", "HeadacheSeverities", "HeadacheTimes", "HeadacheNotes", "HandStretchCount", "HandStretchTags", "HandStretchTimes", "HandStretchNotes", "Dream", "Dream Log"};

            List<string[]> strings = new List<string[]>();

            List<KeepTrackDay> days = new List<KeepTrackDay>();

            csv = _formatDreams(csv);


            for (int i = 0; i < csv.Length; i++) {
                string tempSubString = "";
                if (csv[i].Contains(",\"")) {
                    int quoteIndex = csv[i].IndexOf("\"");
                    tempSubString = csv[i].Substring(quoteIndex, csv[i].Length - quoteIndex);
                    csv[i] = csv[i].Remove(quoteIndex, csv[i].Length - quoteIndex);
                    //weird variation handler
                    if (tempSubString == "\"") {
                        tempSubString += " | ";
                    }
                    //handle entries that contain commas (so csv put them in quotes)
                    bool isItOver = false;
                    int j = 1;
                    while (isItOver == false) {
                        if (!tempSubString.EndsWith("\"")) {
                            tempSubString += " " + csv[i + j];
                            j++;
                        }
                        else isItOver = true;
                    }
                }

                tempSubString = tempSubString.Replace(",", " | ");
                csv[i] += tempSubString;
                strings.Add(csv[i].Split(','));
                }

                //foreach (string[] s in strings) {
                //    Console.WriteLine(s[0]);
                //}


            //keep alphebatized, if frequently adding, should write an alphabetization function
            //IF EDIT THESE - EDIT KEEPTRACKDAY HEADER INDEX IF/ELSE ORDERS
            string[] categoryStrings = { "Breakfast", "Brush Left Hand", "Chess Tactics", "Cold Shower", "Dream Log", "Dual N-Back", "Exercise", "Fish Oil", "Floss", "HandStretch", "Headache", "Meditate", "Read Book", "SleptBedLastNight", "Smoke", "Spaced Repetition" };

            //Create Default Date Objects
            TimeSpan ts = new TimeSpan(07, 00, 0);
            DateTime startDate = DateTime.Parse("08/30/2019") + ts;
            DateTime endDate = DateTime.Now + ts;
            DateTime currDate = startDate;
            while (endDate > currDate) {
                days.Add(new KeepTrackDay(currDate));
                currDate = currDate.AddDays(1);
            }

            int headerIndex = 0;
            string currCategory = categoryStrings[headerIndex];
            for (int i = 0; i < strings.Count; i++) {
                if (strings[i][0].Contains(@"/")) { 
                    foreach (KeepTrackDay day in days) {
                        DateTime tempDate = day.Date.AddDays(1);
                        DateTime tempDate2 = DateTime.Parse(strings[i][0] + " " + strings[i][1]);
                        if (tempDate2 >= day.Date && tempDate2 < tempDate) {
                            day.AddData(headerIndex, strings[i]);
                            break;
                        }
                    }
                }

                //handle category changing on category title found
                if (headerIndex < categoryStrings.Length - 1) {
                    if (strings[i][0] == categoryStrings[headerIndex + 1]) {
                        headerIndex++;
                        currCategory = categoryStrings[headerIndex];
                    }
                }
            }

            DataTable table = new DataTable();

            foreach (string headerString in keepTrackColumnHeaders) {
                table.Columns.Add(headerString);
            }

            foreach (KeepTrackDay day in days) {
                table.Rows.Add(day.Date.ToString("MM/dd/yyyy"), day.Breakfast, day.BrushLeftHand, day.ChessTactics, day.ColdShower, day.DualNBack, day.FishOil, day.Floss, day.FlossTag, day.SleptBedLastNight, day.SpacedRepetition, day.Meditate, day.MeditationTime, day.MeditationNotes, day.ReadBook, day.ReadTime, day.ExerciseCount, day.ExerciseTags, day.ExerciseNotes, day.SmokeCount, day.SmokeTimes, day.SmokeNotes, day.HeadacheCount, day.HeadacheSeverity, day.HeadacheTimes, day.HeadacheNotes, day.HandStretchCount, day.HandStretchTags, day.HandStretchTimes, day.HandStretchNotes, day.DreamLog, day.DreamLogContent);
            }

            dataGridView1.Invoke(new MethodInvoker(() =>
                dataGridView1.DataSource = table)
                        );
        }

        public void parseWeightCSV(string[] csv) {
            string[] weightColumnHeaders = { "Date/Time", "Weight(lb)", "Body Fat", "Muscle Mass", "Water", "BMI" };
            List<string[]> strings = splitStringArrayByDelimiter(csv, ',');
            List<Weight_Day> days = new List<Weight_Day>();
            List<Weight_Entry> weightData = new List<Weight_Entry>();

            //populate Weight Entries
            for (int i = 0; i < strings.Count; i++) {
                weightData.Add(new Weight_Entry(strings[i]));
            }

            //populate day range
            TimeSpan dayHourTermination = new TimeSpan(07, 00, 0);
            DateTime startDate = weightData[0].Date.Date + dayHourTermination;
            DateTime endDate = weightData[weightData.Count - 1].Date;
            DateTime currDate = startDate;
            while (endDate >= currDate) {
                days.Add(new Weight_Day(currDate));
                currDate = currDate.AddDays(1);
            }

            //populate day data
            int currDayIndex = 0;
            foreach (Weight_Entry item in weightData) {
                if (currDayIndex == days.Count - 1) {
                    break;
                }
                bool added = false;
                while (added == false) {
                    if (currDayIndex == days.Count - 1) {
                        break;
                    }
                    if (item.Date.Date > days[currDayIndex].Date) {
                        currDayIndex++;
                    }
                    else {
                        days[currDayIndex].AddData(item);
                        added = true;
                    }
                }
            }

            DataTable table = new DataTable();

            foreach (string headerString in weightColumnHeaders) {
                table.Columns.Add(headerString);
            }

            foreach (Weight_Day day in days) {
                table.Rows.Add(day.Date.ToString("MM/dd/yyyy"), day.Weight, day.Bodyfat,  day.Musclemass, day.Water, day.BMI);
            }

            dataGridView1.Invoke(new MethodInvoker(() =>
                dataGridView1.DataSource = table)
                        );
        }
    
        public void parseDaylioCSV(string[] csv) {
            string[] daylioColumnHeaders = {  "Date","Entries","MoodAvg","StressAvg","EnergyAvg","PainAvg","Tags","Time1","Mood1","Stress1","Energy1","Pain1","Tags1","Notes1","Time2","Mood2","Stress2","Energy2","Pain2","Tags2","Notes2","Time3","Mood3","Stress3","Energy3","Pain3","Tags3","Notes3","Time4","Mood4","Stress4","Energy4","Pain4","Tags4","Notes4","Time5","Mood5","Stress5","Energy5","Pain5","Tags5","Notes5"
            };
            removeStringArrayQuotes(csv);
            List<string[]> strings = splitStringArrayByDelimiter(csv, ',');
            List<Daylio_Day> days = new List<Daylio_Day>();

            for (int i = 0; i < strings.Count; i++) {
                //if days list has 0 entries
                if (days.Count == 0) {
                    days.Add(new Daylio_Day(strings[i][0]));
                    days[i].Items.Add(new Daylio_Item(strings[i][0], strings[i][1], strings[i][2], strings[i][3], strings[i][4], strings[i][5], strings[i][6]));
                    days[i].recalculate();
                }
                //if data matches date of last days entry
                else if (strings[i][0] == days[days.Count-1].FullDate) {
                    days[days.Count - 1].Items.Add(new Daylio_Item(strings[i][0], strings[i][1], strings[i][2], strings[i][3], strings[i][4], strings[i][5], strings[i][6]));
                    days[days.Count - 1].recalculate();
                }
                else {
                    days.Add(new Daylio_Day(strings[i][0]));
                    days[days.Count - 1].Items.Add(new Daylio_Item(strings[i][0], strings[i][1], strings[i][2], strings[i][3], strings[i][4], strings[i][5], strings[i][6]));
                    days[days.Count - 1].recalculate();
                }
            }

            DataTable table = new DataTable();
            populateDataTableHeaders(daylioColumnHeaders, table);

            //append day row data to Data Table
            foreach (Daylio_Day day in days) {
                string[] tempRow = new string[day.RowData.Count];
                for (int i = 0; i < day.RowData.Count; i++) {
                    tempRow[i] = day.RowData[i];
                }
                table.Rows.Add(tempRow);
            }

            DataTable reversedTable = table.AsEnumerable().Reverse().CopyToDataTable();

            //populate data grid view
            dataGridView1.Invoke(new MethodInvoker(() => 
                dataGridView1.DataSource = reversedTable)
                        );
        }

        public void parseSleepCSV(string[] csv) {
            string[] sleepColumnHeaders = { "Date", "Sleep", "Wake", "Total Sleep" };
            removeStringArrayQuotes(csv);
            List<string[]> strings = splitStringArrayByDelimiter(csv,',');
            List<Sleep_Item> sleepData = new List<Sleep_Item>();

            List<Sleep_Day> days = new List<Sleep_Day>();

            //populate sleep data
            for (int i = 0; i < strings.Count; i++) {
                sleepData.Add(new Sleep_Item(strings[i][0], strings[i][2], strings[i][3], strings[i][5]));
            }
            sleepData = sleepData.OrderBy(y => y.Date).ToList();

            //populate day range
            TimeSpan ts = new TimeSpan(07, 00, 0);
            DateTime startDate = sleepData[0].Date + ts;
            DateTime endDate = sleepData[sleepData.Count - 1].Date + ts;
            DateTime currDate = startDate;
            while (endDate >= currDate) {
                days.Add(new Sleep_Day(currDate));
                currDate = currDate.AddDays(1);
            }

            //populate day data
            int currDayIndex = 0;
            foreach (Sleep_Item item in sleepData) {
                if (currDayIndex == days.Count - 1) {
                    break;
                }
                bool added = false;
                while (added == false) {
                    if (item.Date > days[currDayIndex].Date) {
                        currDayIndex++;
                    }
                    else {
                        days[currDayIndex].AddItem(item);
                        added = true;
                    }
                }
            }

            DataTable table = new DataTable();

            foreach (string headerString in sleepColumnHeaders) {
                table.Columns.Add(headerString);
            }

            foreach(Sleep_Day day in days) {
                table.Rows.Add(day.Date.ToString("MM/dd/yyyy"), day.Sleep?.ToString("hh:mm tt"), day.Wake?.ToString("hh:mm tt"), day.TotalSleep?.ToString("hh\\:mm\\:ss"));
            }

            dataGridView1.Invoke(new MethodInvoker(() =>
                dataGridView1.DataSource = table)
                        );
        }
    }

    public class Weight_Entry {
        public DateTime Date { get; set; }
        public double Weight { get; set; }
        public string Bodyfat { get; set; }
        public string Musclemass { get; set; }
        public string Water { get; set; }
        public string BMI { get; set; }

        public Weight_Entry(string[] entry) {
            Date = DateTime.Parse(entry[5]);
            Weight = Double.Parse(entry[0]);
            Bodyfat = entry[1];
            Musclemass = entry[2];
            Water = entry[3];
            BMI = entry[4];
        }
    }

    public class Weight_Day {
        public List<Weight_Entry> Items = new List<Weight_Entry>();
        public DateTime Date { get; set; }
        public double? Weight = null;
        public string Bodyfat { get; set; }
        public string Musclemass { get; set; }
        public string Water { get; set; }
        public string BMI { get; set; }

        public void AddData(Weight_Entry item) {
            Items.Add(item);
            if (Items.Count > 1) {
                double tempWeight = 0;
                foreach (Weight_Entry entry in Items) {
                    tempWeight += entry.Weight;
                }
                Weight = Math.Round(tempWeight / Items.Count, 1);
            }
            else {
                Weight = item.Weight;
            }
            
            Bodyfat = item.Bodyfat;
            Musclemass = item.Musclemass;
            Water = item.Water;
            BMI = item.BMI;
        }

        public Weight_Day(DateTime _date) {
            Date = _date;
        }

    }

    public class Sleep_Day {
        public List<Sleep_Item> Items = new List<Sleep_Item>();
        public DateTime Date { get; set; }
        public DateTime? Sleep = null;
        public DateTime? Wake = null;
        public TimeSpan? TotalSleep = null;

        public void AddItem(Sleep_Item item) {
            //items are parsed in reverse date/time order
            Items.Add(item);
            if (TotalSleep == null) {
                TotalSleep = item.SleepDuration;
            }
            else {
                TotalSleep += item.SleepDuration;
            }

            if (Items.Count == 1) {
                Sleep = item.SleepTime;
                Wake = item.WakeTime;
            }
            Sleep = item.SleepTime;
        }

        public Sleep_Day(DateTime _date) {
            Date = _date;
        }
    }

    public class Sleep_Item {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime SleepTime { get; set; }
        public DateTime WakeTime { get; set; }
        public TimeSpan SleepDuration { get; set; }

        public Sleep_Item(string _id, string _SleepTime, string _WakeTime, string _TotalSleep) {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string[] formats = { "dd. MM. yyyy HH:mm", "dd. MM. yyyy H:mm" };
            Id = _id;
            SleepTime = DateTime.ParseExact(_SleepTime, formats, provider, DateTimeStyles.None);
            WakeTime = DateTime.ParseExact(_WakeTime, formats, provider, DateTimeStyles.None);
            SleepDuration = TimeSpan.FromHours(Convert.ToDouble(_TotalSleep));
            Date = WakeTime.Date.Add(new TimeSpan(0, 00, 0));
        }
    }
    public class Daylio_Day {
        public List<Daylio_Item> Items = new List<Daylio_Item>();
        public string FullDate { get; set; }
        public decimal MoodAvg { get; set; }
        public decimal StressAvg { get; set; }
        public decimal EnergyAvg { get; set; }
        public decimal PainAvg { get; set; }
        public int EnergyEntries = 0;
        public int StressEntries = 0;
        public int PainEntries = 0;
        public string AllExtraActivities { get; set; }

        public List<string> RowData = new List<string>();

        public Daylio_Day(string _fulldate) {
            FullDate = _fulldate;
        }

        public void recalculate() {
            //handle first item being added, (count=1 because it is added before this function call)
            if (Items.Count == 1) {
                //populating initial values
                MoodAvg = Items[0].MoodValue;
                if (Items[0].StressValue != -1) { 
                    StressAvg = Items[0].StressValue;
                    StressEntries++;
                }
                if (Items[0].PainValue != -1) { 
                    PainAvg   = Items[0].PainValue;
                    PainEntries++;
                }
                if (Items[0].EnergyValue != -1) {
                    EnergyAvg = Items[0].EnergyValue;
                    EnergyEntries++;
                }

                AllExtraActivities += Items[0].ExtraActivities;

                //DateTime tempDate = DateTime.Parse(this.FullDate);

                string[] tempRow = { DateTime.Parse(this.FullDate).ToString("MM/dd/yyyy"), this.Items.Count.ToString(), this.MoodAvg.ToString(), this.StressAvg.ToString(), this.EnergyAvg.ToString(), this.PainAvg.ToString(), this.AllExtraActivities,Items[0].Time, Items[0].MoodValue.ToString(), Items[0].StressValue.ToString(), Items[0].EnergyValue.ToString(),Items[0].PainValue.ToString(), Items[0].ExtraActivities, Items[0].Notes };

                RowData.AddRange(tempRow);
            }
            //handling all entries except initial entry
            else {
                decimal moodTotal = 0;
                decimal stressTotal = 0;
                //THIS IS DOING A WHOLE RECALC - we could prob make these totals as higher level vars and then add to them on new entries
                decimal energyTotal = 0;
                decimal painTotal = 0;
                for (int i = 0; i < Items.Count; i++) {
                    moodTotal += Items[i].MoodValue;
                    if (Items[i].StressValue != -1)
                        stressTotal += Items[i].StressValue;
                    if (Items[i].EnergyValue != -1)
                        energyTotal += Items[i].EnergyValue;
                    if (Items[i].PainValue != -1)
                        painTotal += Items[i].PainValue;
                }

                //using 3/9 energyAvg column as tester
                //FIX THIS - IT IS AVERAGING EVEN WHEN A VALUE IS NULL SO ITS LOWER THAN IT SHOULD BE
                if (Items[Items.Count - 1].EnergyValue != -1) 
                    EnergyEntries++;
                if (Items[Items.Count - 1].StressValue != -1) 
                    StressEntries++;
                if (Items[Items.Count - 1].PainValue != -1) 
                    PainEntries++;

                if (Items.Count > 0)
                    MoodAvg = Decimal.Round(moodTotal / Items.Count, 2);
                if (StressEntries > 0)
                    StressAvg = Decimal.Round(stressTotal / StressEntries, 2);
                if (PainEntries > 0)
                    PainAvg = Decimal.Round(painTotal / PainEntries, 2);
                if (EnergyEntries > 0)
                    EnergyAvg = Decimal.Round(energyTotal / EnergyEntries, 2);

                //append new item's data to row
                string[] tempRow = { Items[Items.Count - 1].Time, Items[Items.Count - 1].MoodValue.ToString(), Items[Items.Count - 1].StressValue.ToString(), Items[Items.Count - 1].EnergyValue.ToString(), Items[Items.Count -1].PainValue.ToString(), Items[Items.Count - 1].ExtraActivities, Items[Items.Count - 1].Notes };
                RowData.AddRange(tempRow);
                
                //updating entries and averages display
                RowData[1] = Items.Count.ToString();
                RowData[2] = MoodAvg.ToString();
                RowData[3] = StressAvg.ToString();
                RowData[4] = EnergyAvg.ToString();
                RowData[5] = PainAvg.ToString();

                AllExtraActivities += Items[Items.Count - 1].ExtraActivities;
                RowData[6] = AllExtraActivities;
            }
        }
    }
    public class Daylio_Item {
        public string Full_Date { get; set; }
        public string Date { get; set; }
        public string Weekday { get; set; }
        public string Time { get; set; }
        public string Mood { get; set; }
        public int MoodValue { get; set; }
        public int StressValue { get; set; }
        public int EnergyValue { get; set; }
        public int PainValue { get; set; }
        public string Activities { get; set; }
        public string ExtraActivities { get; set; }
        public string Notes { get; set; }

        public Daylio_Item(string _fullDate, string _date, string _weekday, string _time, string _mood, string _activities, string _notes) {
            Full_Date = _fullDate;
            Date = _date;
            Weekday = _weekday;
            Time = _time;
            Mood = _mood;

            //grab integer Mood value (included in the raw _mood string) and ignore the rest
            MoodValue = Int32.Parse(new String(_mood.Where(Char.IsDigit).ToArray()));
            Activities = _activities;
            for (int i = 0; i < Activities.Length; i++) {
                if (Activities.Contains(" ")) {
                    Activities = Activities.Replace(" ", "");
                }
            }
            _activities = Regex.Replace(_activities, @"\s+", "");
            Notes = _notes;
            //parse stress level
            if (_activities.Contains("NoStress"))
                StressValue = 0;
            else if (_activities.Contains("LowStress"))
                StressValue = 1;
            else if (_activities.Contains("ModerateStress"))
                StressValue = 2;
            else if (_activities.Contains("HighStress"))
                StressValue = 3;
            else if (_activities.Contains("ExtremeStress"))
                StressValue = 4;
            else 
                StressValue = -1;

            //parse energy level
            if (_activities.Contains("NoEnergy"))
                EnergyValue = 0;
            else if (_activities.Contains("LowEnergy"))
                EnergyValue = 1;
            else if (_activities.Contains("ModerateEnergy"))
                EnergyValue = 2;
            else if (_activities.Contains("GoodEnergy"))
                EnergyValue = 3;
            else if (_activities.Contains("GreatEnergy"))
                EnergyValue = 4;
            else
                EnergyValue = -1;

            //parse pain level
            if (_activities.Contains("NoPain"))
                PainValue = 0;
            else if (_activities.Contains("LowPain"))
                PainValue = 1;
            else if (_activities.Contains("MediumPain"))
                PainValue = 2;
            else if (_activities.Contains("HighPain"))
                PainValue = 3;
            else if (_activities.Contains("ExtremePain"))
                PainValue = 4;
            else
                PainValue = -1;

            //parse extra activities
            if (_activities.Contains("Neck"))
                ExtraActivities += "Neck";
            if (_activities.Contains("Scap"))
                ExtraActivities += "Scap";
            if (_activities.Contains("Headache"))
                ExtraActivities += "Headache";
            if (_activities.Contains("Fighting"))
                ExtraActivities += "Fighting";
            if (_activities.Contains("Weed"))
                ExtraActivities += "Weed";
        }
    }
}