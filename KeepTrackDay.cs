using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Document_Parser {
    public class KeepTrackDay {
		public DateTime Date; // AT 7AM

		//if getting too many errors, just make all of these have multiple per-day functionality
		public bool ?Breakfast = null;
		public bool ?BrushLeftHand = null;
		public bool ?ChessTactics = null;
		public bool ?ColdShower = null;
		public bool ?DualNBack = null;
		public bool ?FishOil = null;
		public bool ?Floss = null;
		public string FlossTag = "";
		public bool ?SleptBedLastNight = null;
		public bool ?SpacedRepetition = null;
		public bool ?DreamLog = null;
		public string DreamLogContent = "";

		public int HandStretchCount = 0;
		public string HandStretchTags = "";
		public string HandStretchTimes = "";
		public string HandStretchNotes = "";
		public int HeadacheCount = 0;
		public string HeadacheSeverity = "";
		public string HeadacheTimes = "";
		public string HeadacheNotes = "";
		public bool ?Meditate = null;
		public int MeditationTime = 0;
		public string MeditationNotes = "";
		public bool ?ReadBook = null;
		public int ReadTime = 0;
		public string ReadNotes = "";
		public int SmokeCount = 0;
		public string SmokeTimes = "";
		public string SmokeNotes = "";
        public int ExerciseCount = 0;
		public string ExerciseTags = "";
		public string ExerciseNotes = "";

		public KeepTrackDay(DateTime _date) {
			Date = _date;
		}

		public void AddData(int headerIndex, string[] data) {
			//Breakfast
			if (headerIndex == 0) {
				Breakfast = true;
			}
			//Brush Left Hand
			else if (headerIndex == 1) {
				BrushLeftHand = true;
			}
			//Chess Tactics
			else if (headerIndex == 2) {
				ChessTactics = true;
			}
			//Cold Shower
			else if (headerIndex == 3) {
				ColdShower = true;
			}
			//Dream Log
			else if (headerIndex == 4) {
				DreamLog = true;
				if (data.Length > 2) {
					DreamLogContent = data[2];
				}
					
			}
			//Dual N-Back
			else if (headerIndex == 5) {
				DualNBack = true;
			}
			//Exercise
			else if (headerIndex == 6) {
				ExerciseCount++;
				ExerciseTags = ExerciseTags.Insert(0,data[2] + ", ");
				if (data.Length > 3)
					ExerciseNotes = ExerciseNotes.Insert(0,data[3] + ", ");
			}
			//Fish Oil
			else if (headerIndex == 7) {
				FishOil = true;
			}
			//Floss
			else if (headerIndex == 8) {
				Floss = true;
				FlossTag = data[2];
			}
			//HandStretch
			else if (headerIndex == 9) {
				HandStretchCount++;
				HandStretchTimes = HandStretchTimes.Insert(0, data[1] + ", ");
				HandStretchTags = HandStretchTags.Insert(0, data[2] + ", ");
				if (data.Length > 2)
					HandStretchNotes = HandStretchNotes.Insert(0,data[2] + ", ");
			}
			//Headache
			else if (headerIndex == 10) {
				HeadacheCount++;
				HeadacheSeverity = HeadacheSeverity.Insert(0,data[2] + ", ");
				if (data.Length > 3)
					HeadacheNotes = HeadacheNotes.Insert(0, data[3] + ", ");
			}
			//Meditate
			else if (headerIndex == 11) {
				Meditate = true;
				MeditationTime += Convert.ToInt32(data[2]);
				if (data.Length > 3)
					MeditationNotes += data[3];
			}
			//Read Book
			else if (headerIndex == 12) {
				ReadBook = true;
				ReadTime += Convert.ToInt32(data[2]);
			}
			//SleptBedLastNight
			else if (headerIndex == 13) {
				SleptBedLastNight = true;
			}
			//Smoke
			else if (headerIndex == 14) {
				SmokeCount++;
				SmokeTimes = SmokeTimes.Insert(0, data[1] + ", ");
				if (data.Length > 2)
					SmokeNotes = SmokeNotes.Insert(0, data[2] + ", ");
			}
			//Spaced Repetition
			else if (headerIndex == 15) {
				SpacedRepetition = true;
			}
			
		}
	}
}