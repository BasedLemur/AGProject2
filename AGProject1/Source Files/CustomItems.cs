using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGProject1 {

	/// <summary>
	///  Class to hold variables used in the rest of the program.
	///  This is used to minimize the areas that were changed, to make documentation
	///  easier.
	/// </summary>
	class CustomItems {

		private static Random rand = new Random();

		/// <summary>
		/// Varaible used to see if lerping should be used when walking.
		/// This is set and unset when reading the keyboard state, when "L"
		/// is pressed in Program.cs
		/// </summary>
		public static bool Lerping {
			get; set;
		}

		/// <summary>
		/// Variable used to alter how fast the random range falls when performing the diamond-square algorithm in TerrrainMap.cs
		/// </summary>
		public static double DSFALLOFF = 0;

		/// <summary>
		/// Variable used to alter the starting random range when performing the diamond-square algorithm in TerrainMap.cs
		/// </summary>
		public static int DSINITIALRANGE = 128;

		/// <summary>
		/// Variable used to tell the NPAgent when it should start searching for the next treasure.
		/// Set when the N key is pressed, in Program.cs. Read and unset in NPAgent.cs
		/// </summary>
		private static bool treasureMode = false;
		public static bool TreasureMode {
			get{
				return treasureMode;
			}

			set {
				treasureMode = value;
			}
		}

		/// <summary>
		/// Variable used to tell if the NPAgent is currently seeking out a treasure.
		/// Set and unset in NPAgent.cs
		/// </summary>
		private static bool npSeekingTreasure = false;
		public static bool NPSeekingTreasure {
			get {
				return npSeekingTreasure;
			}
			set {
				npSeekingTreasure = value;
			}
		}

        
        /// <summary>
		/// Number of treasures taken by NPC
		/// Set and unset in NPAgent.cs
		/// </summary>
		private static int npcNumTreasuresLeft = 0;
		public static int NPCNumTreasuresLeft {
			get {
				return npcNumTreasuresLeft;
			}
			set {
				npcNumTreasuresLeft = value;
			}
		}

        /// <summary>
		/// Number of treasures taken by player
		/// Set and unset in NPAgent.cs
		/// </summary>
		private static int playerNumTreasuresLeft = 0;
		public static int PlayerNumTreasuresLeft {
			get {
				return playerNumTreasuresLeft;
			}
			set {
				playerNumTreasuresLeft = value;
			}
		}

        /// <summary>
		/// Number of treasures left
		/// Set and unset in NPAgent.cs
		/// </summary>
		private static int numTreasuresLeft = 4;
		public static int NumTreasuresLeft {
			get {
				return numTreasuresLeft;
			}
			set {
				numTreasuresLeft = value;
			}
		}
        
		/// <summary>
		/// Variables to quickly access the locations of each treasure. Used in
		/// NPAgent.cs
		/// </summary>
		public static readonly int[] TREASURE_ONE_LOCATION = { 273, 411 };
		public static readonly int[] TREASURE_TWO_LOCATION = { 447, 453 };
		public static readonly int[] TREASURE_THREE_LOCATION = { 200, 200 };
		public static readonly int[] TREASURE_FOUR_LOCATION = { 340, 75 };

		public static readonly bool FORWARD = rand.Next(100) < 50;

        public static readonly int numTreasures = 4; 

		public const int SEARCH_DISTANCE = 4000;
	}
}
