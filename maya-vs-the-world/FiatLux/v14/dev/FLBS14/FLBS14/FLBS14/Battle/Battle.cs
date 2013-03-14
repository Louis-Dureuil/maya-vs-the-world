using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Battle.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FLBS14.Primitives;
using FLBS14.Manager;

namespace FLBS14.Battle
{
    class Battle : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Sprite randi;
        Manager.ImageManager imageManager;
        Manager.AnimationManager animationManager;
        Manager.SpriteManager spriteManager;
        Data.Sprite dataSprite;
        bool loading;
        int isDead = 0;

        public Battle()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //DumpSprite();
        }

        public void DumpSprite()
        {
            List<Data.Image> walkingLeft = new List<Data.Image>();
            for (int i = 0; i < 6; i++)
            {
                walkingLeft.Add(new Data.Image());
                walkingLeft[i].File = "Chara\\Randi\\WalkingLeft";
                walkingLeft[i].X = i * 24;
                walkingLeft[i].Y = 0;
                walkingLeft[i].W = 24;
                walkingLeft[i].H = 36;
                Data.DataManager.SaveAsynchronously<Data.Image>(walkingLeft[i], "Data\\Images\\WalkingLeft" + i.ToString() + ".xml");
            }
            Data.Animation walkingLeftA = new Data.Animation();
            walkingLeftA.IsLoop = true;
            walkingLeftA.Images = new Data.SerializableDictionary<string, double>();
            for (int i = 0; i < 6; i++)
            {
                walkingLeftA.Images.Add("Data\\Images\\WalkingLeft" + i.ToString() + ".xml", 0.04);
            }
            Data.DataManager.SaveAsynchronously<Data.Animation>(walkingLeftA,"Data\\Animations\\WalkingLeft.xml");
           
            Data.Sprite randiSprite = new Data.Sprite();
            randiSprite.A = 255;
            randiSprite.Angle = 0;
            randiSprite.B = 255;
            randiSprite.G = 255;
            randiSprite.H = 72;
            randiSprite.W = 48;
            randiSprite.R = 255;
            randiSprite.Animations = new Data.SerializableDictionary<string, string>();
            randiSprite.Animations.Add("Data\\Animations\\WalkingLeft.xml", "walkingLeft");
            Data.DataManager.SaveAsynchronously<Data.Sprite>(randiSprite, "Data\\Sprites\\Randi.xml");
        }

        protected override void Initialize()
        {
            Fighter Lucien = new Fighter(situation, allies);
            Fighter Rakun = new Fighter(situation, allies);
            Fighter Chabada = new Fighter(situation, enemies);
            allies.EnemyParties.Add(enemies);
            enemies.EnemyParties.Add(allies);
            situation.Actors.Add(Lucien);
            situation.Commanders.Add(Lucien);
            situation.Movables.Add(Lucien);
            situation.Targetables.Add(Lucien);

            situation.Actors.Add(Rakun);
            situation.Commanders.Add(Rakun);
            situation.Movables.Add(Rakun);
            situation.Targetables.Add(Rakun);
            
            situation.Actors.Add(Chabada);
            situation.Commanders.Add(Chabada);
            situation.Movables.Add(Chabada);
            situation.Targetables.Add(Chabada);
            fighters.Add(Lucien);
            fighters.Add(Rakun);
            fighters.Add(Chabada);

            allies.Members.Add(Lucien);
            allies.Members.Add(Rakun);
            enemies.Members.Add(Chabada);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameCommon.SpriteFont = Content.Load<SpriteFont>("SpriteFont1");
            imageManager = new Manager.ImageManager(this.Services);
            animationManager = new Manager.AnimationManager();
            spriteManager = new Manager.SpriteManager(spriteBatch);
            Data.Sprite sprite = Data.DataManager.Load<Data.Sprite>("Data\\Sprites\\Randi.xml");
            randi = new Sprite(spriteManager);
            randi.LoadAsynchronously(sprite, animationManager, imageManager);
            //randi.W = 48;
            //randi.H = 72;
            //randi.X = 400;
            //randi.Y = 200;
        }

        private void UpdateActive(GameTime gameTime)
        {
            // Look forward for some active actor
            situation.Ap -= (float)gameTime.ElapsedGameTime.TotalSeconds * GameConstants.Speed["Inactive"];
            float maxAp = situation.Ap;
            foreach (IActive active in situation.Actors)
            {
                if (active.IsActive && active.Ap > maxAp)
                    maxAp = active.Ap;
            }
            situation.Ap = maxAp;

            situation.Ap -= (float)gameTime.ElapsedGameTime.TotalSeconds * situation.Speed;
            situation.Speed = GameConstants.Speed["Normal"];
            foreach (IActive active in situation.Actors)
            {
                if (active.IsActive)
                    active.OnActive();
            }
            foreach (IMovable movable in situation.Movables)
            {
                if (movable.IsMoving)
                    movable.OnMove();
            }
            if (situation.Ap <= 0)
            {
                state = BattleState.TurnEnding;
            }
            if (situation.Actors[0].IsActive)
            {
                randi.SetCurrent("walkingLeft");
                randi.X -= situation.ApFlow * 6;
                if (randi.X < -36)
                {
                    randi.X = 800;
                }
                if (randi.CurrentAnimation != null)
                    randi.CurrentAnimation.Speed = situation.ApFlow;
            }
        }

        private void UpdateEnded(GameTime gameTime)
        {
            situation.Ap += (float)gameTime.ElapsedGameTime.TotalSeconds * GameConstants.Speed["Ended"];
            if (situation.Ap >= GameConstants.MaxAp)
            {
                situation.Ap = GameConstants.MaxAp;
                state = BattleState.TurnStarting;
            }
        }

        private void UpdateStarted(GameTime gameTime)
        {
            state = BattleState.TurnActive;
        }

        private void UpdateEnding(GameTime gameTime)
        {
            state = BattleState.TurnEnded;
            situation.Ap = 0;
            foreach (IActive active in situation.Actors)
            {
                active.OnTurnEnd();
            }
        }

        private void UpdateStarting(GameTime gameTime)
        {
            state = BattleState.TurnStarted;
            float sum = 0.0F;

            foreach (IActive active in situation.Actors)
            {
                sum += active.Initiative;
            }

            float avg = sum / situation.Actors.Count;

            foreach (IActive active in situation.Actors)
            {
                active.OnTurnStart(avg);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One).Buttons.A == 
                Microsoft.Xna.Framework.Input.ButtonState.Pressed && 
                !loading)
            {
                Data.DataManager.LoadAsynchronously<Data.Sprite>("Data\\Sprites\\Randi.xml", (t) => { dataSprite = t; });
                loading = true;
            }

            if (dataSprite != null)
            {
                randi.LoadAsynchronously(dataSprite, animationManager, imageManager);
                dataSprite = null;
                loading = false;
            }
            spriteManager.Update(gameTime);
            if (randi.CurrentAnimation != null)
                randi.CurrentAnimation.Speed = 0;
            randi.SetCurrent("lookingLeft");
            switch (state)
            {
                case BattleState.TurnActive:
                    UpdateActive(gameTime);
                    break;
                case BattleState.TurnEnded:
                    UpdateEnded(gameTime);
                    break;
                case BattleState.TurnEnding:
                    UpdateEnding(gameTime);
                    break;
                case BattleState.TurnStarted:
                    UpdateStarted(gameTime);
                    break;
                case BattleState.TurnStarting:
                    UpdateStarting(gameTime);
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(GameCommon.SpriteFont, Math.Ceiling(situation.Ap).ToString(), Vector2.Zero, Color.White);
            Vector2 text = new Vector2(0, 32);
            foreach (Fighter fighter in fighters)
            {
                if (isDead < 1)
                {
                    Animation walkingLeft = new Animation(animationManager);
                    List<Image> walkingLeftImages = new List<Image>();
                    Animation lookingLeft = new Animation(animationManager);
                    Image lookingLeftImage = new Image(imageManager, "Chara\\Randi\\LookingLeft");
                    lookingLeftImage.SrcX = 0;
                    lookingLeftImage.SrcY = 0;
                    lookingLeftImage.SrcW = 24;
                    lookingLeftImage.SrcH = 36;
                    lookingLeft.Add(lookingLeftImage, 1000.0);
                   

                    for (int i = 0; i < 6; i++)
                    {
                        Image walk = new Image(imageManager, "Chara\\Randi\\WalkingLeft");
                        walk.SrcX = i * 24;
                        walk.SrcY = 0;
                        walk.SrcW = 24;
                        walk.SrcH = 36;
                        walkingLeft.Add(walk, 0.03);
                    }
                    //randi.Animations.Add("walkingLeft", walkingLeft);
                    //randi.Animations.Add("lookingLeft", lookingLeft);
                    randi.SetCurrent("lookingLeft");
                    isDead++;
                }
                spriteBatch.DrawString(GameCommon.SpriteFont, "AP: " + Math.Ceiling(fighter.Ap).ToString() + "\t" +
                "End: " + fighter.End.Val.ToString() + "\t" +
                "MP: " + fighter.Mp.Val.ToString() + "\t" +
                "PhyAtt: " + fighter.AttPhy.Val.ToString() + "\t" +
                "PhyDef: " + fighter.DefPhy.Val.ToString(), text, fighter.StateColor);
                text.Y += 32;
                if (gameTime.IsRunningSlowly)
                {
                    spriteBatch.DrawString(GameCommon.SpriteFont, "SLOW!", new Vector2(300, 200), Color.Red);
                }
            }
            spriteBatch.End();
            if (isDead > 0)
            {
                spriteManager.Draw(gameTime);
            }
        }

        #region private
        private BattleSituation situation = new BattleSituation();
        private BattleState state;
        private Party allies = new Party("Allies");
        private Party enemies = new Party("Enemies");
        private List<Fighter> fighters = new List<Fighter>();
        #endregion
    }
}
