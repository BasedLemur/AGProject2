/*  
	 Copyright (C) 2017 G. Michael Barnes
 
	 The file NPAgent.cs is part of AGMGSKv9 a port and update of AGXNASKv8 from
	 MonoGames 3.5 to MonoGames 3.6  

	 AGMGSKv9 is free software: you can redistribute it and/or modify
	 it under the terms of the GNU General Public License as published by
	 the Free Software Foundation, either version 3 of the License, or
	 (at your option) any later version.

	 This program is distributed in the hope that it will be useful,
	 but WITHOUT ANY WARRANTY; without even the implied warranty of
	 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	 GNU General Public License for more details.

	 You should have received a copy of the GNU General Public License
	 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/


#region Using Statements
using System;
using System.IO;  // needed for trace()'s fout
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace AGMGSKv9 {

	/// <summary>
	/// A non-playing character that moves.  Override the inherited Update(GameTime)
	/// to implement a movement (strategy?) algorithm.
	/// Distribution NPAgent moves along an "exploration" path that is created by the
	/// from int[,] pathNode array.  The exploration path is traversed in a reverse path loop.
	/// Paths can also be specified in text files of Vector3 values, see alternate
	/// Path class constructors.
	/// 
	/// 1/20/2016 last changed
	/// </summary>
	public class NPAgent : Agent {
		private NavNode nextGoal;

		/// <summary>
		/// Variable used to store a copy of the next normal goal for the NPAgent.
		/// Used when the NPAgent finishes searching for a treasure, to resume its normal pathing.
		/// </summary>
		private NavNode backupNextGoal;

		/// <summary>
		/// Treasure the NPAgent is currently seeking.
		/// </summary>
		private AGProject1.Treasure treasureGoal;


		private Path path;
		private int snapDistance = 20;  // this should be a function of step and stepSize
		// If using makePath(int[,]) set WayPoint (x, z) vertex positions in the following array
		//private int[,] pathNode = { {505, 490}, {500, 500}, {490, 505},  // bottom, right
		//							{435, 505}, {425, 500}, {420, 490},  // bottom, middle
		//							{420, 450}, {425, 440}, {435, 435},  // middle, middle
		//							{490, 435}, {500, 430}, {505, 420},  // middle, right
		//							{505, 105}, {500,  95}, {490,  90},  // top, right
		//							{110,  90}, {100,  95}, { 95, 105},  // top, left
		//							{ 95, 480}, {100, 490}, {110, 495},  // bottom, left
		//							{495, 480} };						  // loop return

		private int[,] pathNodeForward = { { 505, 490 }, { 500, 500 }, { 490, 505 },
											{ 437, 480 }, { 430, 459 }, { 443, 420 },
											{ 339, 362 }, { 318, 366 }, { 271, 394 },
											{ 210, 208 }, { 210, 151 }, { 225, 135 },
											{ 248, 117 }, { 320,  93 }, { 384, 109 },
											{ 505, 200 } };

		private int[,] pathNodeBackward = { { 505, 200 }, { 384, 109 }, { 320,  93 },
											{ 248, 117 }, { 225, 135 }, { 210, 151 },
											{ 210, 208 }, { 271, 394 }, { 318, 366 },
											{ 339, 362 }, { 443, 420 }, { 430, 459 },
											{ 437, 480 }, { 490, 505 }, { 500, 500 },
											{ 505, 490 } };
		/// <summary>
		/// Create a NPC. 
		/// AGXNASK distribution has npAgent move following a Path.
		/// </summary>
		/// <param name="theStage"> the world</param>
		/// <param name="label"> name of </param>
		/// <param name="pos"> initial position </param>
		/// <param name="orientAxis"> initial rotation axis</param>
		/// <param name="radians"> initial rotation</param>
		/// <param name="meshFile"> Direct X *.x Model in Contents directory </param>
		public NPAgent(Stage theStage, string label, Vector3 pos, Vector3 orientAxis, 
				float radians, string meshFile)
				: base(theStage, label, pos, orientAxis, radians, meshFile) { 
			// change names for on-screen display of current camera
			first.Name =  "npFirst";
			follow.Name = "npFollow";
			above.Name =  "npAbove";
            IsCollidable = true;  // agent test collision with Collidable set.
			stage.Collidable.Add(agentObject);  // agents's agentObject can be collided with by others.
			// path is built to work on specific terrain, make from int[x,z] array pathNode
			path = new Path(stage, AGProject1.CustomItems.FORWARD ? pathNodeForward : pathNodeBackward, Path.PathType.LOOP); // continuous search path
			stage.Components.Add(path);
			nextGoal = path.NextNode;  // get first path goal
			backupNextGoal = new NavNode(nextGoal.Translation);
			agentObject.turnToFace(nextGoal.Translation);  // orient towards the first path goal
			// set snapDistance to be a little larger than step * stepSize
			snapDistance = (int) (1.5 * (agentObject.Step * agentObject.StepSize));
		}	

		/// <summary>
		/// Simple path following.  If within "snap distance" of a the nextGoal (a NavNode) 
		/// move to the NavNode, get a new nextGoal, turnToFace() that goal.  Otherwise 
		/// continue making steps towards the nextGoal.
		/// </summary>
		public override void Update(GameTime gameTime) {

			// If the NPAgent should begin searching for a treasure
			if (!AGProject1.CustomItems.NPSeekingTreasure) {
				// find closest non-activated treasure
				// get distances to each treasure
				float t1distance = Vector2.Distance(new Vector2(this.agentObject.Translation.X, this.agentObject.Translation.Z), new Vector2(AGProject1.CustomItems.TREASURE_ONE_LOCATION[0] * stage.Spacing, AGProject1.CustomItems.TREASURE_ONE_LOCATION[1] * stage.Spacing));
				float t2distance = Vector2.Distance(new Vector2(this.agentObject.Translation.X, this.agentObject.Translation.Z), new Vector2(AGProject1.CustomItems.TREASURE_TWO_LOCATION[0] * stage.Spacing, AGProject1.CustomItems.TREASURE_TWO_LOCATION[1] * stage.Spacing));
				float t3distance = Vector2.Distance(new Vector2(this.agentObject.Translation.X, this.agentObject.Translation.Z), new Vector2(AGProject1.CustomItems.TREASURE_THREE_LOCATION[0] * stage.Spacing, AGProject1.CustomItems.TREASURE_THREE_LOCATION[1] * stage.Spacing));
				float t4distance = Vector2.Distance(new Vector2(this.agentObject.Translation.X, this.agentObject.Translation.Z), new Vector2(AGProject1.CustomItems.TREASURE_FOUR_LOCATION[0] * stage.Spacing, AGProject1.CustomItems.TREASURE_FOUR_LOCATION[1] * stage.Spacing));

				// put distances into an array, then sort
				Tuple<float, AGProject1.Treasure> tup1dist = new Tuple<float, AGProject1.Treasure>(t1distance, stage.treasure1);
				Tuple<float, AGProject1.Treasure> tup2dist = new Tuple<float, AGProject1.Treasure>(t2distance, stage.treasure2);
				Tuple<float, AGProject1.Treasure> tup3dist = new Tuple<float, AGProject1.Treasure>(t3distance, stage.treasure3);
				Tuple<float, AGProject1.Treasure> tup4dist = new Tuple<float, AGProject1.Treasure>(t4distance, stage.treasure4);
				Tuple<float, AGProject1.Treasure>[] distances = { tup1dist, tup2dist, tup3dist, tup4dist };
				Array.Sort(distances, new AGProject1.TreasureTupleComparer());
				Console.WriteLine("" + distances[0].Item1 + ":" + distances[0].Item2.Name + " " + distances[1].Item1 + ":" + distances[1].Item2.Name + " " + distances[2].Item1 + ":" + distances[2].Item2.Name + " " + distances[3].Item1 + ":" + distances[3].Item2.Name);

				// set next target to treasure
				if(!distances[0].Item2.Activated() && (AGProject1.CustomItems.TreasureMode || distances[0].Item1 <= AGProject1.CustomItems.SEARCH_DISTANCE)) {
					
					nextGoal = new NavNode(new Vector3(distances[0].Item2.Object.Translation.X, 0, distances[0].Item2.Object.Translation.Z));
					treasureGoal = distances[0].Item2;
					AGProject1.CustomItems.NPSeekingTreasure = true;
				}
				else if(!distances[1].Item2.Activated() && (AGProject1.CustomItems.TreasureMode || distances[1].Item1 <= AGProject1.CustomItems.SEARCH_DISTANCE)) {
					nextGoal = new NavNode(new Vector3(distances[1].Item2.Object.Translation.X, 0, distances[1].Item2.Object.Translation.Z));
					treasureGoal = distances[1].Item2;
					AGProject1.CustomItems.NPSeekingTreasure = true;
				}
				else if(!distances[2].Item2.Activated() && (AGProject1.CustomItems.TreasureMode || distances[2].Item1 <= AGProject1.CustomItems.SEARCH_DISTANCE)) {
					nextGoal = new NavNode(new Vector3(distances[2].Item2.Object.Translation.X, 0, distances[2].Item2.Object.Translation.Z));
					treasureGoal = distances[2].Item2;
					AGProject1.CustomItems.NPSeekingTreasure = true;
				}
				else if(!distances[3].Item2.Activated() && (AGProject1.CustomItems.TreasureMode || distances[3].Item1 <= AGProject1.CustomItems.SEARCH_DISTANCE)) {
					nextGoal = new NavNode(new Vector3(distances[3].Item2.Object.Translation.X, 0, distances[3].Item2.Object.Translation.Z));
					treasureGoal = distances[3].Item2;
					AGProject1.CustomItems.NPSeekingTreasure = true;
				}
			}
			agentObject.turnToFace(nextGoal.Translation);  // adjust to face nextGoal every move
			// agentObject.turnTowards(nextGoal.Translation);
			// See if at or close to nextGoal, distance measured in 2D xz plane
			float distance = Vector3.Distance(
				new Vector3(nextGoal.Translation.X, 0, nextGoal.Translation.Z),
				new Vector3(agentObject.Translation.X, 0, agentObject.Translation.Z));
			stage.setInfo(15, stage.agentLocation(this));
			stage.setInfo(16,
				string.Format("        nextGoal ({0:f0}, {1:f0}, {2:f0})  distance to next goal = {3,5:f2})", 
				nextGoal.Translation.X/stage.Spacing, nextGoal.Translation.Y, nextGoal.Translation.Z/stage.Spacing, distance) );
			if (distance <= snapDistance)  {
				// If the NPAgent finished searching for a treasure
				if (AGProject1.CustomItems.NPSeekingTreasure) {
					// Set the treasure to active.
					treasureGoal.Activate();
                    AGProject1.CustomItems.NPCNumTreasuresLeft = AGProject1.CustomItems.NPCNumTreasuresLeft + 1; 
                    AGProject1.CustomItems.NumTreasuresLeft = AGProject1.CustomItems.NumTreasuresLeft - 1; 
					AGProject1.CustomItems.NPSeekingTreasure = false;
					AGProject1.CustomItems.TreasureMode = false;
					// Resume moving to the last active waypoint.
					nextGoal = new NavNode(backupNextGoal.Translation);
				}
				// else, the agent finished moving to the next node in its path.
				else {
					// snap to nextGoal and orient toward the new nextGoal 
					nextGoal = path.NextNode;
					backupNextGoal = new NavNode(nextGoal.Translation);
					// agentObject.turnToFace(nextGoal.Translation);
				}
			}
			base.Update(gameTime);  // Agent's Update();
		}
	} 
}
