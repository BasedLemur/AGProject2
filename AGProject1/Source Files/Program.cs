/*
 * Program.cs is the starting point for AGMGSK applications.
 *
 * For all projects submission you should:
 * 1.  Delete the file game1.cs  from your project
 * 2.  Edit the last lines in this comment appropriately
 *			so that each group members name and email are listed alphabetically
 *				for example:
 *					Group member:  Mike Barnes  renzo@csun.edu
 *					Group member:  Iam Smart	IamSmart@mycsun.edu 
 *			the project is labeled
 *		 the class and semester are specified
 *
 * 
 * Group member:  Mike Barnes
 * Project 1  Terrain Map
 * Comp 565 Spring 2017
 */

/**
*	DOCUMENTATION OF CODE CHANGES:
*	
*	CustomItems.cs:
*		This class was added to hold variables used throughout the rest of the program.
*		This is to minimize the areas of the program that are changed, and to make
*		documentation easier.
*		
*		Variables:
*			- Lerping: Variable used to see if lerping should be used when walking.
*					   This is set and unset when reading the keyboard state, when
*					   "L" is pressed in Program.cs
*			- DSFALLOFF: Variable used to alter how fast the random range falls when
*						 performing the diamond-square algorithm in TerrrainMap.cs
*			- DSINITIALRANGE: Variable used to alter the starting random range when
*							  performing the diamond-square algorithm in TerrainMap.cs
*			- TreasureMode: Variable used to tell the NPAgent when it should start
*							searching for the next treasure. Set when the N key is
*							pressed, in Program.cs. Read and unset in NPAgent.cs
*			- NPSeekingTreasure: Variable used to tell if the NPAgent is currently
*								 seeking out a treasure. Set and unset in NPAgent.cs
*			- TREASURE_X_LOCATION: Variables to quickly access the locations of each
*								   treasure. Used in NPAgent.cs
*								   
*	NPAgent.cs:
*		New variables:
*			- private NavNode backupNextGoal: Variable used to store a copy of the next
*											  normal goal for the NPAgent. Used when the
*											  NPAgent finishes searching for a treasure,
*											  to resume its normal pathing.
*			- private AGProject1.Treasure treasureGoal: Treasure the NPAgent is currently
*														seeking.
*		
*		Functions changed:
*			- Update: Added a check to see if the NPAgent should seek out the closest
*					  available treasure. Finds the treasure based on distance.
*					  Also checking if we're close to a treasure whenever we're searching
*					  for a treasure. When we are, activate the treasure.
*	
*	Stage.cs:
*		New variables:
*			- public AGProject1.Treasure treasureX: Variables to load treasures into the
*													world.
*		
*		Functions changed:
*			- setSurfaceHeight: Checks if Lerping is active. If it is, then it gets the 4
*								points surrounding the player, and Lerps the two heights
*								on corresponding X lines. Then it lerps between those two
*								points to get the final height.
*			- LoadContent: Loads in 4 treasures at specified positions.
*			- Update: Added keybindings. Press "L" to activate/deactivate lerping, and press
*										 "N" to enable treasure-seeking mode on the NPAgent.
*										 
*	TerrainMap.cs:
*		New functions:
*			- diamondSquareHeightMap: Create a height map using the Diamond-Square algorithm.
*									  This function initializes the four corners to random
*									  values, then begins the algorithm proper.
*			- diamondSquarize: The actual Diamond-Square algorithm. This runs Diamond-Square on
*							   the map parameter, then recurrsively calls itself until the
*							   algorithm is complete.
*		
*		Functions changed:
*			- createHeightTexture: Calls the DiamondSquareHeightMap to generate the height
*								   texture, instead of the pyramid world generation.
*			- Update: Added keybindings. "R" to reset Diamond-Square to its initial values, then
*					  regenerate the height and colors. "E" to regenerate the height and colors,
*					  but not reset Diamond-Square to its initial values. "Left" to lower the rate
*					  at which the random range falls off. "Right" to raise the rate at which the
*					  random range falls off. "Up" to increase the initial random range. "Down" to
*					  decrease the initial random range. "S" to save the height map and color map
*					  as PNGs.
*					  
*	Treasure.cs:
*		Class to make creation of treasures easier. Extends MovableModel3D.
*		
*		Variables:
*			- m_activated: Track if the treasure is active
*			- Object: Keep a reference to its own object when addObject is called
*		
*		Functions:
*			- Constructor: Calls parent constructor, with "jiggy" as the mesh file name.
*			- Update: Calls parent update. If the treasure is active, then moves the model up or down
*					  by the sine of the total game time.
*			- addObject: Calls parent addObject function, with the scale of the object set to 100x in
*						 all directions.
*			- Activate: Activates the treasure.
*			- Activated: Check if the treasure has been activated.
*			
*	TreasureTupleComparer.cs
*		Comparer functor used when finding the closest treasure to the NPAgent. This just compares the
*		float part of the tuple that contains the distance (float) and the corresponding treasure.
* 
*/

using System;

// Uncomment the following main function to use TerrainMap.cs
/** /
namespace TerrainMap {
/// <summary>
/// The main class.
/// </summary>
	public static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			using (var game = new TerrainMap())
					game.Run();
			}
	}
}

// Uncomment the following to use the AGMGSK
/**/
namespace AGMGSKv9 {

	 static class Program {

		  static void Main(string[] args) {
				using (Stage stage = new Stage()) {
					 stage.Run();
				}
		  }

	 }

}

/**/