using AGMGSKv9;
using Microsoft.Xna.Framework;
using System;

namespace AGProject1 {
	/// <summary>
	/// Class to make creation of treasures easier. Extends MovableModel3D
	/// </summary>
	public class Treasure : MovableModel3D {

		// Track if the treasure is active
		private bool m_activated = false;
		// Keep a reference to its own object when addObject is called
		private Object3D m_object3D;
		public Object3D Object {
			get {
				return m_object3D;
			}
		}

		public Treasure(Stage theStage, string label) : base(theStage, label, "jiggy") {
			base.IsCollidable = true;
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			// if activated, levetate
			if(m_activated) {
				m_object3D.Translation = new Vector3(m_object3D.Translation.X, m_object3D.Translation.Y + (float)(Math.Sin(5 * gameTime.TotalGameTime.TotalSeconds) * 2), m_object3D.Translation.Z);
			}
		}

		public Object3D addObject(Vector3 position, float radians = 0) {
			m_object3D = base.addObject(position, new Vector3(0, 1, 0), radians, new Vector3(100, 100, 100));
			return m_object3D;
		}

		// Set the treasure ot active.
		public void Activate() {
			m_activated = true;
			m_object3D.Translation = new Vector3(m_object3D.Translation.X, m_object3D.Translation.Y + 100, m_object3D.Translation.Z);
		}

		// Check if the treasure has been activated.
		public bool Activated() {
			return m_activated;
		}
	}
}
