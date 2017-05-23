using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LinearAlgebraGraphicsDemonstration
{
    /// <summary>
    /// A class responsible for the camera and its controls
    /// </summary>
    class Camera
    {
        /// <summary>
        /// The view matrix
        /// </summary>
        public Matrix View { get; set; }

        /// <summary>
        /// The projection matrix
        /// </summary>
        public Matrix Projection { get; set; }

        /// <summary>
        /// The eye's position
        /// </summary>
        public Vector3 Position { get; private set; }

        const float thumbstickSensitivity = 0.04f;

        float yaw = 0.0f;
        float pitch = 0.0f;
        float targetFOV = MathHelper.PiOver4;
        float currentFOV = MathHelper.PiOver4;
        float currentAlpha = 0.0f;
        float targetAlpha = 0.0f;
        bool zoomedIn = false;
        bool pressedZoomInLast = false;
        GraphicsDevice graphicsDevice;
        Texture2D zoomIndicator;
        ProjectionMatrix projectionRef;
        ViewMatrix viewRef;

        /// <summary>
        /// Constructs a new camera
        /// </summary>
        /// <param name="initialPosition">The initial position for the eye</param>
        /// <param name="initialYaw">The initial yaw</param>
        /// <param name="initialPitch">The initial pitch</param>
        /// <param name="graphicsDevice">The graphics device</param>
        /// <param name="content">The content manager</param>
        /// <param name="projectionRef">A reference to the projection stackable matrix</param>
        /// <param name="viewRef">A referemce to the view stackable matrix</param>
        public Camera(Vector3 initialPosition, float initialYaw, float initialPitch, GraphicsDevice graphicsDevice, ContentManager content,
            ProjectionMatrix projectionRef, ViewMatrix viewRef)
        {
            this.graphicsDevice = graphicsDevice;
            Position = initialPosition;
            yaw = initialYaw;
            pitch = initialPitch;

            this.projectionRef = projectionRef;
            this.viewRef = viewRef;

            zoomIndicator = content.Load<Texture2D>("ZoomIndicator");
        }

        /// <summary>
        /// Updates animations, and gathers inputs
        /// </summary>
        /// <param name="time">The time</param>
        public void Update(GameTime time)
        {
            GamePadState inputState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboard = Keyboard.GetState();
            bool isConnected = inputState.IsConnected;

            Matrix orientation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0.0f);

            // Do slick zoom animations
            currentFOV = MathHelper.Lerp(currentFOV, targetFOV, (float)time.ElapsedGameTime.TotalSeconds * 15.0f);
            currentAlpha = MathHelper.Lerp(currentAlpha, targetAlpha, (float)time.ElapsedGameTime.TotalSeconds * 15.0f);

            // Set projection and views to what they should be
            Projection = Matrix.CreatePerspectiveFieldOfView(currentFOV, graphicsDevice.Viewport.AspectRatio, 0.1f, 100.0f);
            View = Matrix.CreateLookAt(Position, -Vector3.Transform(Vector3.Forward, orientation) + Position, Vector3.Up);

            // Do input-related tasks
            if (isConnected)
            {
                yaw += -inputState.ThumbSticks.Right.X * thumbstickSensitivity;
                pitch += -inputState.ThumbSticks.Right.Y * thumbstickSensitivity;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.A))
                    yaw += thumbstickSensitivity * 0.5f;
                if (keyboard.IsKeyDown(Keys.D))
                    yaw -= thumbstickSensitivity * 0.5f;
                if (keyboard.IsKeyDown(Keys.W))
                    pitch -= thumbstickSensitivity * 0.5f;
                if (keyboard.IsKeyDown(Keys.S))
                    pitch += thumbstickSensitivity * 0.5f;
            }

            float bumperInfluence = 0.0f;

            if (isConnected)
            {
                if (inputState.Buttons.LeftShoulder == ButtonState.Pressed)
                    bumperInfluence = -thumbstickSensitivity;
                if (inputState.Buttons.RightShoulder == ButtonState.Pressed)
                    bumperInfluence = thumbstickSensitivity;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Q))
                    bumperInfluence = -thumbstickSensitivity;
                if (keyboard.IsKeyDown(Keys.E))
                    bumperInfluence = thumbstickSensitivity;
            }

            bool rightStickPressed = isConnected ? inputState.Buttons.RightStick == ButtonState.Pressed : keyboard.IsKeyDown(Keys.Space);
            if (rightStickPressed && !pressedZoomInLast)
            {
                zoomedIn = !zoomedIn;
                if (zoomedIn)
                {
                    targetFOV = MathHelper.PiOver4 / 2.0f;
                    targetAlpha = 0.2f;
                }
                else
                {
                    targetFOV = MathHelper.PiOver4;
                    targetAlpha = 0.0f;
                }
                pressedZoomInLast = true;
            }
            if (!rightStickPressed)
                pressedZoomInLast = false;

            float speedMult = 1.0f;
            if (isConnected)
            {
                float triggerAmount = inputState.Triggers.Left;
                if (triggerAmount > 0.0f)
                    speedMult = triggerAmount * 3.0f + 1.0f;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.LeftShift))
                    speedMult = 4.0f;
            }

            float transX = 0.0f;
            float transY = 0.0f;

            if (isConnected)
            {
                transX = -inputState.ThumbSticks.Left.X;
                transY = inputState.ThumbSticks.Left.Y;
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.Left))
                    transX += 1;
                if (keyboard.IsKeyDown(Keys.Right))
                    transX -= 1;
                if (keyboard.IsKeyDown(Keys.Up))
                    transY += 1;
                if (keyboard.IsKeyDown(Keys.Down))
                    transY -= 1;
            }

            Position += Vector3.Transform(new Vector3(transX * thumbstickSensitivity * speedMult, bumperInfluence,
                    transY * thumbstickSensitivity * speedMult),
                    orientation);

            projectionRef.UpdateProj(Projection);
            viewRef.UpdateView(View);
        }

        /// <summary>
        /// Draws the zoom indicator over the screen
        /// </summary>
        /// <param name="spriteBatch">The sprite batch</param>
        public void DrawHUDElements(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(zoomIndicator, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White * currentAlpha);
        }
    }
}
