﻿using System.Collections.Generic;
using System.Timers;
using BattleCity.Model.Components;
using BattleCity.Views;

namespace BattleCity.Model.Game
{
    class OnePlayerGame : GameBase
    {
        private GameTimer GameTimer;

        // Constructors:
        public OnePlayerGame(Level levelInfo)
            : base(levelInfo.FieldInfo)
        {
            LvlName = levelInfo.Name;
            //
            Player = new PlayerModel(position: levelInfo.PlayerInfo, field: Field, game: this);
            Player.DieEvent += () => GameState.Lose();
            //
            NPCs = new List<NPCModel>();
            foreach ((int, int) position in levelInfo.NPCsInfo)
            {
                NPCs.Add(new NPCModel( position, Field, Player, game: this));
            }
            //
            Bullets = new List<Bullet>();
            //
            GameTimer = new GameTimer(loop_function: MainLoop, interval: 120);
        }

        // Overriden methods:
        protected override void StartLoop()
        {
            GameTimer.StartTimer();
        }
        public override void Quit() => GameState.Lose();

        // Private methods:
        private void StopTheGame()
        {
            GameTimer.StopTimer();
            GameOver = true;
            View.PrintGameOver(Won);
        }

        private void MainLoop(object sender, ElapsedEventArgs e)
        {                
            // MOVING
            Player.MoveHero();
            if (_npcTime)
            {
                InvokeNPCs();
            }


            // INVOKING BULLETS
            InvokeBullets();


            // SHOOTING
            if (_npcTime)
            {
                InvokeNPCs(shoot: true);
            }
            Player.Shoot();

            
            // RENDERING THE MAP
            _npcTime = !_npcTime;
            FieldViewer.GetInstance().Render();


            // CHECK IF THE GAME IS OVER
            if ( NPCs.Count == 0 || GameState.Died() )
            {
                GameState.Win();
                StopTheGame();
            }
        }
    }
}
