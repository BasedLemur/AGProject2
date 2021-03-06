﻿/*  
	 Copyright (C) 2017 G. Michael Barnes
 
	 The file Pack.cs is part of AGMGSKv9 a port and update of AGXNASKv8 from
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
    /// Pack represents a "flock" of MovableObject3D's Object3Ds.
    /// Usually the "player" is the leader and is set in the Stage's LoadContent().
    /// With no leader, determine a "virtual leader" from the flock's members.
    /// Model3D's inherited List<Object3D> instance holds all members of the pack.
    /// 
    /// 2/1/2016 last changed
    /// </summary>
    public class Pack : MovableModel3D {	
	    Object3D leader;
        public int flockLevel = 0; 
        float seperationEnd = 1000;
	    float seperationDecline = 400;
	    float alignmentStart = 400;
	    float alignmentPeak = 1000;
	    float alignmentDecline = 2000;
	    float alignmentEnd = 3000;
	    float cohesionStart = 2000;
	    float cohesionPeak = 3000;
	    float SAhalf = 600;             //half way weight distance between seperation and alignment
	    float AChalf = 1000;            //half way weight distance between alignment and cohesion
        float distance = 0; 
	    float weight = 1;           
        double[] flockValues = new double[] {0.0, 0.33, 0.66, 0.99};
        bool packing = false;
    
    

        /// <summary>
        /// Construct a pack with an Object3D leader
        /// </summary>
        /// <param name="theStage"> the scene </param>
        /// <param name="label"> name of pack</param>
        /// <param name="meshFile"> model of a pack instance</param>
        /// <param name="xPos, zPos">  approximate position of the pack </param>
        /// <param name="aLeader"> alpha dog can be used for flock center and alignment </param>
	    public Pack(Stage theStage, string label, string meshFile, int nDogs, int xPos, int zPos, Object3D theLeader)
		    : base(theStage, label, meshFile) {
		    isCollidable = true;
		    random = new Random();
		    leader = theLeader;
		    int spacing = stage.Spacing;
		    // initial vertex offset of dogs around (xPos, zPos)
		    int [,] position = { {0, 0}, {7, -4}, {-5, -2}, {-7, 4}, {5, 2} };
		    for( int i = 0; i < position.GetLength(0); i++) {
			    int x = xPos + position[i, 0];
			    int z = zPos + position[i, 1];
			    float scale = (float)(0.25 + random.NextDouble());
			    addObject( new Vector3(x * spacing, stage.surfaceHeight(x, z), z * spacing),
						      new Vector3(0, 1, 0), 0.0f,
						      new Vector3(scale, scale, scale));
		    }
        }
		

	    /// <summary>
	    /// Each pack member's orientation matrix will be updated.
	    /// Distribution has pack of dogs moving randomly.  
	    /// Supports leaderless and leader based "flocking" 
	    /// </summary>		
	    public override void Update(GameTime gameTime) {
		    // if (leader == null) need to determine "virtual leader from members"
		    float angle = 0.3f;
        
            if (random.NextDouble() < flockValues[flockLevel])
		    {
			    packing = true;
		    }

            if (!packing)
		    {
			    foreach (Object3D obj in instance)
			    {
				    obj.Yaw = 0.0f;
				    if (random.NextDouble() < 0.08)
				    {
					    if (random.NextDouble() < 0.5) obj.Yaw -= angle;
					    else obj.Yaw += angle;
				    }
				    obj.updateMovableObject();
				    stage.setSurfaceHeight(obj);
			    }
			    base.Update(gameTime); 
		    }
		    else if (packing)
		    {
			    foreach (Object3D obj in instance)
			    {
				    flock(obj);
				    packing = false;
			    }
		    }
        }

        public float weightCalc(float distance, float x, float y)
	    {
		    float calculatedWeight = 0;
		    calculatedWeight = (y - (distance - x)) / y;
		    return calculatedWeight;
	    }

	    public float InverseweightCalc(float inverseWeight)
	    {
		    inverseWeight = 1 - inverseWeight;
		    return inverseWeight;
	    }

        public Object3D Leader {
	    get { return leader; }
	    set { leader = value; }
        }

        public void flock(Object3D dog)
	    {
		    Vector3 flockingVector = Vector3.Zero;
		    Vector3 forwardVector;
		    Vector3 alignmentVector;
		    Vector3 cohesionVector;
		    Vector3 seperationVector;
		    Vector3 rotationAxis;
		    float turningAngle = 0.1f;

            forwardVector = dog.Forward;              
            alignmentVector = leader.Forward;
		    cohesionVector = Vector3.Zero;
		    seperationVector = Vector3.Zero;

		    distance = Vector3.Distance(dog.Translation, leader.Translation);     

		    if (distance < seperationEnd)
		    {
			    foreach (Object3D model in instance)
			    {
				    if (model == leader)
				    {
					    seperationVector = seperationVector - model.Translation - dog.Translation;
				    }
			    }
			    seperationVector = seperationVector - leader.Translation - dog.Translation;

			    if (distance > seperationDecline && distance < seperationEnd)
			    {
				    weight = weightCalc(distance, seperationDecline, SAhalf);
			    }
			    seperationVector = seperationVector * weight;
			    Vector3.Normalize(seperationVector);
		    }
		    if ((distance > alignmentStart) && (distance < alignmentEnd))
		    {
			    if (distance > alignmentStart && distance < alignmentPeak)
			    {
				    weight = weightCalc(distance, seperationDecline, SAhalf);
				    weight = InverseweightCalc(weight);
			    }
			    else if (distance > alignmentDecline && distance < alignmentEnd)
			    {
				    weight = weightCalc(distance, alignmentDecline, AChalf);
			    }
			    alignmentVector = alignmentVector * weight;
			    Vector3.Normalize(alignmentVector);
		    }
        
		    if (distance > cohesionStart)
		    {
			    cohesionVector = leader.Translation - dog.Translation;

			    if (distance > cohesionPeak)
			    {
				    weight = 1;
			    }
			    else if (distance > cohesionStart && distance < cohesionPeak)
			    {
				    weight = weightCalc(distance, alignmentDecline, AChalf);
				    weight = InverseweightCalc(weight);
			    }

			    cohesionVector = cohesionVector * weight;
			    Vector3.Normalize(cohesionVector);
		    }

		    flockingVector = alignmentVector + cohesionVector + seperationVector;
		    Vector3.Normalize(flockingVector);

		    rotationAxis = Vector3.Cross(forwardVector, flockingVector);
		    Vector3.Normalize(flockingVector);

		    if (rotationAxis.X + rotationAxis.Y + rotationAxis.Z < 0)
		    {
			    turningAngle = -turningAngle;
		    }

		    dog.Yaw += turningAngle;
		    dog.updateMovableObject();
		    stage.setSurfaceHeight(dog);
	    }
    }
}