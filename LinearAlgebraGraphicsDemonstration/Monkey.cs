using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// Represents the monkey model drawer
    /// </summary>
    class Monkey
    {
        Model monkeyModel;
        Matrix currentTransform;

        public Monkey(ContentManager content)
        {
            monkeyModel = content.Load<Model>("Monkey");
        }

        /// <summary>
        /// Updates the monkey with its current transform
        /// </summary>
        /// <param name="currentTransformMatrix">The model/world matrix</param>
        public void Update(Matrix currentTransformMatrix)
        {
            currentTransform = currentTransformMatrix;
        }

        /// <summary>
        /// Draws the monkey
        /// </summary>
        /// <param name="camera">The camera</param>
        /// <param name="bias">If true, nudges monkey towards camera a small amount to avoid Z-fighting artifacts</param>
        public void Draw(Camera camera, bool bias=false)
        {
            foreach (ModelMesh mesh in monkeyModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Alpha = 0.85f;
                    if (bias)
                    {
                        Vector3 direction = currentTransform.Translation - camera.Position;
                        direction.Normalize();
                        Matrix biasedTransform = currentTransform;
                        biasedTransform.Translation -= direction * 0.005f;
                        effect.World = biasedTransform;
                    }
                    else
                        effect.World = currentTransform;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                mesh.Draw();
            }
        }
    }
}
