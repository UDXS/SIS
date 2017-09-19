using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft;
namespace SIS
{
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D pausebutton;
        Texture2D background;
        SpriteFont debug;
        SoundEffect music;
        SoundEffect explosion;
        Texture2D hero;
        Texture2D herom;
        Texture2D enemy;
        Texture2D enemym;      
        SoundEffect heroshoot;
        SoundEffect enemyshoot;
        List<Point> Missiles;
        List<Point> Emissiles;
        List<Vector2> Enemies;
        Vector2 loc;
        int lost;
        int wave;
        int health;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            InactiveSleepTime = TimeSpan.FromSeconds(1);
            Content.RootDirectory = "Content";             
        }
       
        protected override void Initialize()
        {
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            loc = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 4 * 3);
            Missiles = new List<Point>();
            Enemies = new List<Vector2>();
            wave = 0;
            health = 0;
            base.Initialize();
        }
        SoundEffectInstance song;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            debug = Content.Load<SpriteFont>("debug");
            background = Content.Load<Texture2D>("background");
            pausebutton = Content.Load<Texture2D>("pausebutton");
            explosion = Content.Load<SoundEffect>("explosion");
            hero = Content.Load<Texture2D>("hero");
            herom = Content.Load<Texture2D>("missilehero");
            heroshoot = Content.Load<SoundEffect>("heroshoot");
            enemy = Content.Load<Texture2D>("enemy");
            enemym = Content.Load<Texture2D>("missileenemy");
            enemyshoot = Content.Load<SoundEffect>("enemyshoot");
            music = Content.Load<SoundEffect>("music");
            song = music.CreateInstance();
        }
        int count;
        protected override void Update(GameTime gameTime)
        {
            if (song.State == SoundState.Stopped)
            {
                song.Pitch = -.50f;
                song.Play();
            }
            GamePadState s = GamePad.GetState(PlayerIndex.One);
                if (s.ThumbSticks.Left.X < .4f)
                    loc.X += 8;
                if (s.ThumbSticks.Left.X > .4f)
                    loc.X -= 8;
                if (s.IsButtonDown(Buttons.RightTrigger))
                {
                    Missiles.Add(new Point((int)loc.X + 20, GraphicsDevice.Viewport.Height / 4 * 3 - 30));
                    heroshoot.Play();
                }
            if (loc.X + 100 > GraphicsDevice.Viewport.Width)
                loc.X = GraphicsDevice.Viewport.Width - 100;
            if (loc.X < 0)
                loc.X = 0;
            if (Enemies.Count == 0)
            {
                health = 3;
                Emissiles = new List<Point>();
                Enemies = new List<Vector2>();
                Random r = new Random();
                for (int cnt = 0; cnt <= r.Next(9, 30); cnt++)
                {
                    Enemies.Add(new Vector2(r.Next(75, GraphicsDevice.Viewport.Width - 75), r.Next(-2500, -40)));
                }
                wave++;
            }
            else
            {
                for (int cnt = 0; cnt < Enemies.Count; cnt++)
                {
                    Vector2 tmp = Enemies[cnt];
                    tmp.Y += 3;
                    Enemies[cnt] = tmp;
                    if (Enemies[cnt].Y > GraphicsDevice.Viewport.Height)
                    {
                        Enemies.RemoveAt(cnt);
                        cnt--;
                        health--;
                    }
                }
            }
for (int cnte = 0; Enemies.Count > cnte; cnte++) { 
            for(int cntm = 0; Missiles.Count>cntm;cntm++){
                if(new Rectangle((int)Enemies[cnte].X, (int)Enemies[cnte].Y, 75, 38).Intersects(new Rectangle(Missiles[cntm].X, Missiles[cntm].Y, 50, 50))){
                    Enemies.RemoveAt(cnte);
                    cnte--;
                    Missiles.RemoveAt(cntm);
                    cntm--;
                    explosion.Play();
                }
            }
            }
            if (health == 0)
            {
               // System.Windows.MessageBox.Show("You Finished On Wave " + wave + "!", "Game Over!!!", System.Windows.MessageBoxButton.OK);
                health = 3;
                wave = 0;
                Enemies = new List<Vector2>();
            }
            if (Missiles.Count == 9)
            {
                Missiles.RemoveAt(0);
                explosion.Play(0.75f, 1, 1);
                lost++;
            }
            if (Missiles.Count != 0)
            {
                for (int cnt = 0; cnt < Missiles.Count; cnt++)
                {
                    Point tmp = Missiles[cnt];
                    tmp.Y -= 3;
                    Missiles[cnt] = tmp;
                    if (Missiles[cnt].Y == 0)
                    {
                        Missiles.RemoveAt(cnt);
                        explosion.Play();
                        lost++;
                    }
                    }
                }
                this.count++;
                base.Update(gameTime);
            }

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(pausebutton, new Rectangle(GraphicsDevice.Viewport.Width - 70, 0, 70, 70), Color.White);
            spriteBatch.Draw(hero,loc,Color.White);
            foreach (Point m in Missiles) {
                spriteBatch.Draw(herom, new Vector2(m.X,m.Y), Color.White);
            }
            foreach (Vector2 e in Enemies) {
                spriteBatch.Draw(enemy,e, Color.White);
            }
            spriteBatch.DrawString(debug, "Wave " + wave, Vector2.Zero, Color.MediumPurple);
            spriteBatch.DrawString(debug,Enemies.Count + " Enemies Left", new Vector2(0,20), Color.MediumPurple);
            spriteBatch.DrawString(debug, health + "/3 Lives Left", new Vector2(0, 40), Color.MediumPurple);
            //spriteBatch.DrawString(debug, "Version 1.0 ALPHA(Added Aliens, Wave/Health System)", new Vector2(0, GraphicsDevice.Viewport.Height - 27),Color.MediumPurple);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
