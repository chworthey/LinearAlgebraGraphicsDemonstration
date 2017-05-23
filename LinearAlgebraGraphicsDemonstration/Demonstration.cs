using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// This is the main class responsible for the Demonstration
    /// </summary>
    public class Demonstration : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<StackableMatrix> matrixStack = new List<StackableMatrix>();
        List<StackableMatrix> hiddenStack = new List<StackableMatrix>();
        Camera camera;
        Monkey monkey;
        Model axis;

        RasterizerState solidState;
        RasterizerState wireframeState;

        /// <summary>
        /// True if initial content is loaded
        /// </summary>
        public bool Loaded { get; private set; }

        public Demonstration()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
        }

        /// <summary>
        /// LoadContent will load the content on start up.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ViewMatrix view = new ViewMatrix(Matrix.Identity, Content, GraphicsDevice, 1);
            ProjectionMatrix proj = new ProjectionMatrix(Matrix.Identity, Content, GraphicsDevice, 2);

            // Add our initial matrices to the stack
            matrixStack.Add(new DataMatrix(Content, GraphicsDevice));
            matrixStack.Add(view);
            matrixStack.Add(proj);

            // Set up the rest of our assets
            camera = new Camera(new Vector3(3, 1, 3), MathHelper.Pi + MathHelper.PiOver4, MathHelper.Pi / 25.0f,GraphicsDevice, Content, proj, view);

            monkey = new Monkey(Content);
            solidState = new RasterizerState();
            solidState.FillMode = FillMode.Solid;

            wireframeState = new RasterizerState();
            wireframeState.FillMode = FillMode.WireFrame;

            axis = Content.Load<Model>("Axis");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Loaded = true;

            GamePadState inputState = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (inputState.Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Update each matrix
            for (int n = 0; n < matrixStack.Count; n++)
            {
                matrixStack[n].Update(gameTime);

                // Tell them to recalculate their target positions
                matrixStack[n].MoveToSlot(matrixStack[n].MatrixSlot);
            }

            // Update the soon-to-be-hidden stack, and delete ones that are completely invisible
            for (int n = 0; n < hiddenStack.Count; n++)
            {
                hiddenStack[n].Update(gameTime);
                if (hiddenStack[n].IsHidden)
                {
                    hiddenStack.Clear();
                    break;
                }
            }

            // Update everything else
            camera.Update(gameTime);

            Matrix transform = getTransform();

            monkey.Update(transform);

            base.Update(gameTime);
        }

        // Multiply all of the model transforms together and return them
        Matrix getTransform()
        {
            Matrix transform = Matrix.Identity;
            for (int n = 1; n < matrixStack.Count - 2; n++)
            {
                transform *= matrixStack[n].Value;
            }
            return transform;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // Solid Opaque pass for axis
            GraphicsDevice.RasterizerState = solidState;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh mesh in axis.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                mesh.Draw();
            }

            // Solid Alpha Blended pass for monkey/ninja
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = solidState;
            monkey.Draw(camera);
            
            // Wire frame Additive pass for monkey/ninja
            GraphicsDevice.BlendState = BlendState.Additive;
            GraphicsDevice.RasterizerState = wireframeState;

            monkey.Draw(camera, true); // Wire frame draw needs to be biased to avoid Z-Fighting, hence true

            // Draw 2D overlay elements
            spriteBatch.Begin();
            camera.DrawHUDElements(spriteBatch);

            for (int n = 0; n < matrixStack.Count; n++)
            {
                matrixStack[n].Draw(spriteBatch);
            }

            for (int n = 0; n < hiddenStack.Count; n++)
            {
                hiddenStack[n].Draw(spriteBatch);
            }

            spriteBatch.End();


            base.Draw(gameTime);
        }

        /// <summary>
        /// Adds a rotation matrix to the top of the model stack (before view and proj)
        /// </summary>
        /// <param name="axis">The axis to rotate on</param>
        /// <param name="radians">Radians to rotate about the axis</param>
        public void AddRotationMatrix(RotationMatrixMajorAxis axis, float radians)
        {
            insertMat(new RotationMatrix(axis, radians, Content, GraphicsDevice, matrixStack.Count - 2));
        }

        /// <summary>
        /// Adds a translation matrix to the top of the model stack (before view and proj)
        /// </summary>
        /// <param name="translation">The translation vector</param>
        public void AddTranslationMatrix(Vector3 translation)
        {
            insertMat(new TranslationMatrix(translation, Content, GraphicsDevice, matrixStack.Count - 2));
        }

        /// <summary>
        /// Flattens the model stack into one transform (before view and proj, after data)
        /// </summary>
        public void FlattenTransform()
        {
            Matrix transform = getTransform();
            ClearTransform();

            insertMat(new FlattenedMatrix(transform, Content, GraphicsDevice, 1));
        }

        /// <summary>
        /// Deletes all model stack transforms (before view and proj, after data)
        /// </summary>
        public void ClearTransform()
        {
            int count = matrixStack.Count - 3;
            for (int n = 0; n < count; n++)
            {
                hiddenStack.Add(matrixStack[1]);
                matrixStack[1].Hide();
                matrixStack.RemoveAt(1);
            }

            StackableMatrix.SetSpacing(StackableMatrix.DefaultSpacing);
            matrixStack[matrixStack.Count - 1].MoveToSlot(matrixStack.Count - 1);
            matrixStack[matrixStack.Count - 2].MoveToSlot(matrixStack.Count - 2);
        }

        /// <summary>
        /// Adds a scale matrix to the top of the model stack (before view and proj)
        /// </summary>
        /// <param name="scale">The X, Y, and Z scale</param>
        public void AddScaleMatrix(Vector3 scale)
        {
            insertMat(new ScaleMatrix(scale, Content, GraphicsDevice, matrixStack.Count - 2));
        }

        /// <summary>
        /// Adds a user-defined matrix to the top of the model stack (before view and proj)
        /// </summary>
        /// <param name="values">The values of each element of the matrix</param>
        public void AddInputMatrix(params float[] values)
        {
            insertMat(new InputMatrix(Content, GraphicsDevice, matrixStack.Count - 2, values));
        }

        // Inserts a stackable matrix at the end of the model stack (before view and proj)
        void insertMat(StackableMatrix mat)
        {
            bool isOutOfScreen = matrixStack[matrixStack.Count - 1].MoveToSlot(matrixStack.Count);
            matrixStack[matrixStack.Count - 2].MoveToSlot(matrixStack.Count - 1);
            matrixStack.Insert(matrixStack.Count - 2, mat);

            if (isOutOfScreen)
                FlattenTransform();
        }
    }
}
