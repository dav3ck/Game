using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Prologue
{


    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch, FrontSpriteBatch;
        public static PrologueContent prologueContent;

        private MouseState oldMouseState;
        public static KeyboardState newKeyboardState, oldKeyboardState;

        public double GridSizeWidth, GridSizeHeight, GridSize;

        private Map map1;
        private Player player;
        private NPC npc1;
        private NPC npc2;
        private Objects TestObject;

        public bool intersect;




        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 600; //720 // 600
            graphics.PreferredBackBufferWidth = 1000; //1280 //1000
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            IsMouseVisible = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            FrontSpriteBatch = new SpriteBatch(GraphicsDevice);

            prologueContent = new PrologueContent(Content);

            //Some general Setup information
            Camera.InitializeCamera(new Vector2(0,0), true);

            Screen.ScreenWidth = GraphicsDevice.Viewport.Width;
            Screen.ScreenHeight = GraphicsDevice.Viewport.Height;

            Screen.GridSize = Screen.ScreenWidth / Screen.MinGridX;

            SpriteSheet.LoadedSpriteSheets.Add(new SpriteSheet("Textbox_SpriteSheet"));
            SpriteSheet.LoadedSpriteSheets.Add(new SpriteSheet("Player_Walk_SpriteSheet"));
            SpriteSheet.LoadedSpriteSheets.Add(new SpriteSheet("Test_Animation_SpriteSheet"));

            map1 = new Map(prologueContent, spriteBatch);
            player = new Player(20, 17, FrontSpriteBatch, prologueContent, Map.Tilelist);
            Console.WriteLine("Items Initialized!");
            npc1 = new NPC(21, 19, "Mathijs", FrontSpriteBatch, prologueContent);

            Textbox.LoadTextBoxData();

            Vector2 Location = new Vector2(100, 100);
            //Textbox.TextBoxes.Add(new InformationTextBox( "HALLO HOE GAAT ET ERMEE@ Persoonlijk gaat het wel redelijk met mij, de bedoeling van deze zin is is dat ie tering lang word zodat hij gesplit moet worden@ Maar.. Maar@ Ohhnee@stut@stut@stutterrr@Jahoor@volgende pagina aub dankuwel@", 600, 150, FrontSpriteBatch, prologueContent));

            //TestObject = new Objects(7, 7, 1, FrontSpriteBatch, prologueContent);




            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            //Keyboard Input Registration 
            if (newKeyboardState.IsKeyDown(Keys.A))
            {
                player.HorizontalMov("Left");
            }
            if (newKeyboardState.IsKeyDown(Keys.D))
            {
                player.HorizontalMov("Right");
            }
            if (newKeyboardState.IsKeyDown(Keys.W))
            {
                player.VerticalMov("Up");
            }
            if (newKeyboardState.IsKeyDown(Keys.S))
            {
                player.VerticalMov("Down");
            }
            if (oldKeyboardState.IsKeyDown(Keys.Q) && newKeyboardState.IsKeyUp(Keys.Q))
            {
                foreach (var x in Textbox.TextBoxes)
                {
                    if (x.Continue == false)
                    {
                        x.SkipText = false;
                        x.NextPage();
                    }

                    else { x.SkipText = true; }
                }

                if (Textbox.TextBoxes.Any(x => x.Finish == true))
                {
                    Textbox.Delete();
                }
            }
            if(newKeyboardState.IsKeyDown(Keys.X) && !oldKeyboardState.IsKeyDown(Keys.X))
            {
                List<Tuple<int,int>> TestPath = Utility.GeneratePath(Tuple.Create(1,1), Tuple.Create(256,256));
                foreach (Tuple<int, int> z in TestPath)
                {
                    Console.WriteLine(z);
                } 

                //Console.WriteLine(Utility.GeneratePath(Tuple.Create(0,0),Tuple.Create(256,256)));

                //NPC.ToggleFollow("Mathijs");

                /*
                List<Tuple<int, int>> input = new List<Tuple<int, int>>
                {
                    Tuple.Create(3, 6), Tuple.Create(4, 8), Tuple.Create(3, 10)
                };

                Objects.RemoveObj(input); */
            }
            if (newKeyboardState.IsKeyDown(Keys.E) && !oldKeyboardState.IsKeyDown(Keys.E))
            {
                Console.WriteLine("??????????????");
                EventHandler.EventList.Add( new EventHandler(1));
            }
            if (newKeyboardState.IsKeyDown(Keys.Space) && !oldKeyboardState.IsKeyDown(Keys.Space))
            {
                if(Player.Player1.Frozen != true)
                {
                    Objects obj = Objects.ObjectList.Find(x => x.CanInteract == true);
                    if (obj != null) { obj.Interact(); }
                }
            }


                //-----------------------------



                //Updating of all the classes, This is a temperary Test setup

                /* foreach(var _npc in NPC.NPClist)
                 {
                     _npc.Update();
                 } */

                if (Autowalker.AutowalkerList.Any())
            {
                foreach (var autowalker in Autowalker.AutowalkerList)
                {
                    autowalker.Update();
                }

                Autowalker.DeleteAutoWalker();
            }

            if(EventHandler.EventList.Count > 0)
            {
                if (EventHandler.Continue == true)
                {
                    EventHandler.EventUpdate();
                }
            }

            if (Textbox.TextBoxes.Any() == true)
            {
                Textbox.Update();
            }

            Player.Player1.TickUpdate();
            AnimationTick.TickUpdate();

            Camera.Update();

            base.Update(gameTime);

            oldKeyboardState = newKeyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            foreach(var item in SolidTile.LoadedTiles)
            {
                item.Draw();
            }
            spriteBatch.End();

            FrontSpriteBatch.Begin();
            player.Draw();

            foreach( var _npc in NPC.NPClist)
            {
                _npc.Draw();
            }
            foreach(var _object in SolidTile.LoadedObjects)
            {
                _object.Draw();
            }

            if (Textbox.TextBoxes.Any() == true)
            {
                Textbox.Draw();
            }

            //SpriteSheet.screenDraw("Textbox_SpriteSheet", 0, 1, new Vector2(100, 100), FrontSpriteBatch);

            FrontSpriteBatch.End();

            //Console.WriteLine(gameTime.TotalGameTime);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }


    }
}
