//////////////////////////////////////////////////////////////////////////////
// Project : Lab 02 - Missile Command
// Submission code: 1201_2300_L02
// November 2 2020
// By Alan May
// ///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDIDrawer;

namespace AlanMay_Lab02_Missile_Command
{
    public partial class mainform : Form
    {
        //lists used to store missile data F for 'enemy missiles' and A for allied/ or user missiles
        List<Missile> _missileF = new List<Missile>();
        List<Missile> _missleA = new List<Missile>();

        Timer _timer = new Timer();
        CDrawer _canvas = null;
        int _lives = 5;         //lives will decrement when F missiles hit the bottom of GDI window
        int _score = 0;         //score increased by 100 when user successfully shoots down an enemy missile 
        int _turretNumb = 2;    //there are 3 turrets in the game, default 2 is for the 'middle' turret
        int _ammo;              //standard behaving missile ammo (shot via left click event)
        int _resupply = 0;      //when resupply hits 30 in main game loop add 1 to _ammo and 5 to _flakAmmo
        int _flakAmmo;          //short range smaller explosion missile
        int _flakMShot = 0;     //tracks how many flak missiles shot
        int _missileShot = 0;   //tracks how many flak missiles shot
        int _missilesKilled = 0;//missiles the user has killed
        //int _enemyMissleSpawn = 0;//number of enemy missiles that spawn

        //enum used in timer event handler to determine whether to run game or display message
        enum eGameState { Paused, Unstarted, Over, Running };
        eGameState _gameState = eGameState.Unstarted;
        public mainform()
        {
            InitializeComponent();
            _uiBtnStart.Click += _uiBtnStart_Click;
//<<<<<<< .mine
            _timer.Interval = 50;

            _timer.Tick += _timer_Tick;
            _timer.Start();
            _canvas = new CDrawer(800, 600, bContinuousUpdate: false);
            Missile.Canvas = _canvas;   //links the gdi window to class

            //event handlers for User Interface
            _canvas.MouseLeftClick += _canvas_MouseLeftClick;
            _canvas.MouseRightClick += _canvas_MouseRightClick;
            _canvas.KeyboardEvent += _canvas_KeyboardEvent;
            Shown += Mainform_Shown;
            FormClosing += Mainform_FormClosing;
            _uiNuDLives.Minimum = 1;
            _uiNuDEnemy.Minimum = 1;
            _uiNuDEnemy.Value = 5;
            _uiNuDLives.Value = 5;

        }


        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            _canvas.Close();
        }

        private void Mainform_Shown(object sender, EventArgs e)
        {
            _canvas.Position = new Point(Location.X + Width, Location.Y);
            
        }
        #region User Interface
        private void _canvas_KeyboardEvent(bool bIsDown, Keys keyCode, CDrawer dr)
        {
            switch (keyCode)
            {

                //asd keys will select various 'turrets' - places to launch missiles from
                case Keys.A:
                    _turretNumb = 1;
                    //Console.WriteLine($"turret {_turretNumb}");
                    break;
                case Keys.S:
                    _turretNumb = 2;
                    break;
                case Keys.D:
                    _turretNumb = 3;
                    break;
                    //P stops the timer from running the main game method
                case Keys.P:
                    _gameState = eGameState.Paused;
                    break;
                    //allows the timer to resume running game method
                case Keys.R:
                    _gameState = eGameState.Running;
                    break;

                    //starts a new game
                case Keys.Y:
                    StartGame();
                    //Console.WriteLine("start btn");
                    break;

                    //lowers the timer value therefore speeding the game up
                    //since a glitch causes each keystroke to fire twice this actually will decrement timer by 10
                    
                case Keys.Add:
                    
                    _timer.Stop();
                    if (_timer.Interval > 15)
                    {
                        //Console.WriteLine("Key - pressed");
                        _timer.Interval -= 5;
                    }
                    //_uiLblSpeed.Text = _timer.Interval.ToString();
                    _timer.Start();
                    //Console.WriteLine("Key + pressed");

                    break;

                //raisess the timer therefore speeding the game up
                //since a glitch causes each keystroke to fire twice this actually will increase timer by 10

                case Keys.Subtract:
                    _timer.Stop();
                    _timer.Interval += 5;
                   // _uiLblSpeed.Text = _timer.Interval.ToString();
                    _timer.Start();

                    break;

            }
            /*
            if(keyCode == Keys.A)
            {

                _turretNumb = 1;
                Console.WriteLine($"turret {_turretNumb}");
            }*/
        }

        /// <summary>
        /// user right clicks on GDI window, creating a destination for a missile
        /// if _turretNumb will determine the launch location of a standard missile
        /// there is a set amount for ammo so missiles can only be launched if there is _ammo
        /// </summary>
        /// <param name="pos">Mouse click location</param>
        /// <param name="dr">GDI reference - not used</param>
        private void _canvas_MouseRightClick(Point pos, CDrawer dr)
        {
            //_ammo is the amount of ammo for a standard missile. _turretNumb represents the launch point for missile
            //signifying different turrets
            if (_ammo > 0)
            {
                if (_turretNumb == 1)
                {
                    _missleA.Add(new Missile(0, pos.X, pos.Y, 15, Missile.MissileType.MissileA));
                }
                else if (_turretNumb == 2)
                {
                    _missleA.Add(new Missile(0, pos.X, pos.Y, 15, Missile.MissileType.MissileS));
                }
                else
                {
                    _missleA.Add(new Missile(0, pos.X, pos.Y, 15, Missile.MissileType.MissileD));
                }
                --_ammo;
                ++_missileShot;
            }
        }
        /// <summary>
        /// user left clicks on GDI window, creating a destination for a missile
        /// if _turretNumb will determine the launch location of a standard missile
        /// there is a set amount for ammo so missiles can only be launched if there is _flakAmmo
        /// these missiles fly faster than 'standard' missiles but have limited range and a smaller explosion radius
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dr"></param>
        private void _canvas_MouseLeftClick(Point pos, CDrawer dr)
        {
            //_flakAmmo is the amount of ammo for a faster shorter range missile. _turretNumb represents the launch point for missile
            //signifying different turrets
            if (_flakAmmo > 0)
            {
                if (_turretNumb == 1)
                {

                    _missleA.Add(new Missile(0, pos.X, pos.Y, 30, Missile.MissileType.FlakA));
                }
                else if (_turretNumb == 2)
                {
                    _missleA.Add(new Missile(0, pos.X, pos.Y, 30, Missile.MissileType.FlakS));
                }
                else
                {
                    _missleA.Add(new Missile(0, pos.X, pos.Y, 30, Missile.MissileType.FlakD));
                }
                --_flakAmmo;
                ++_flakMShot;
            }
        }
        /// <summary>
        /// clears & resets all values and resets gamestate to a 'new game'
        /// _gamestate is tracked in _timer_tick event handler
        /// which is used to determine whether to run game loop or not
        /// </summary>
        private void StartGame()
        {
            _uiNuDEnemy.Visible = false;
            _uiNuDLives.Visible = false;
            //clears missile lists
            _missileF.Clear();
            _missleA.Clear();
            _lives = (int)_uiNuDLives.Value;           //amount of lives (times _missileF missiles hit bottom of GDI window)
            _ammo = 10;                               //standard missile starting(new game) ammo
            _flakAmmo = 20;                           //ammo for faster, smaller exploding missiles
            _gameState = eGameState.Running;          //used by _timer_tick event handler to run game method
            _missilesKilled = 0;
            //_enemyMissleSpawn = (int)_uiNuDEnemy.Value;
        }

        /// <summary>
        /// user clicks start button on main form thus starting a new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _uiBtnStart_Click(object sender, EventArgs e)
        {
            StartGame();

        }
        #endregion
        #region timer(enum conditions used)
        /// <summary>
        /// timer event hanlder reads the eGameState value and either runs game loop code or displays a message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Tick(object sender, EventArgs e)
        {

            //eGamestate has 4 possibilities
            //Running - allows the game to run
            //Paused - pauses the current state of the game
            //Unstarted - initial state of the program upon starting
            //game over - user has run out of lives
            if (_gameState == eGameState.Running)
            {
                Running();
            }
            else if (_gameState == eGameState.Paused)
            {
                _canvas.Clear();
                _canvas.AddText("paused, press R to resume", 40, Color.Ivory);
                _canvas.Render();
            }
            else if (_gameState == eGameState.Unstarted)
            {
                _canvas.Clear();
                _canvas.AddText("To start game, either click start button or \r\n click on GDI window and press y." +
                    "\r\n see adjectnt form for controls and instructions", 30, Color.Ivory);
                _canvas.Render();
            }
            else if (_gameState == eGameState.Over)
            {
                _canvas.Clear();
                _canvas.AddText($"Gameover Final Score: {_score} \r\n play again(y)?", 40, Color.White);
                _canvas.Render();
            }
        }
        #endregion
        #region game running code
        /// <summary>
        /// Main game method
        /// Enemy missiles will spawn at the top of GDI window if there are less than 5 enemy missiles
        /// on GDI window. Enemey missiles will be removed if fly out of bounds (beyond the left or right side of
        /// GDI window) if a missile hits the bottom of GDI window a life will be removed and the missile destroyed
        /// </summary>
        public void Running()
        {

            //int foe = 0;
            //count is used to track if user has destroyed an enemy missile
            //if count1 > count2 a missile has been destroyed thus adding to score
            int count1;
            int count2;
            //foe = _missile.Count;

            //generate new enemy missiles if needed
            while (_missileF.Count < _uiNuDEnemy.Value)
            {
                _missileF.Add(new Missile());
                //++foe;
            }
            //Missile.Loading = true;
            //clears GDI window
            _canvas.Clear();

            //iterate through enemy missiles moving and rending them
            for (int c = 0; c < _missileF.Count; ++c)
            {
                _missileF[c].Move();
                _missileF[c].Render();
            }

            //iterate through friendly missiles moving and rending them
            for (int c = 0; c < _missleA.Count; ++c)
            {
                _missleA[c].Move();
                _missleA[c].Render();

            }
            //allied missiles that explode will 'fade out' as tracked by their Alpha value
            //when the alpha value goes below 10 the missiles are removed from the list
            _missleA.RemoveAll(m => m.Alpha < 10);
            /*
            for(int c=0;c<_missleA.Count;++c)
            {
             
            }*/
            //Missile.Loading = false;

            //text to display game stats to user
            _canvas.AddText($"lives: {_lives} Score: {_score}", 40, Color.White);
            _canvas.AddText($"ammo: {_ammo} flak ammo: {_flakAmmo}", 10, new Rectangle(_canvas.ScaledWidth / 2 - 100, _canvas.ScaledHeight - (_canvas.ScaledHeight / 4), 200, 20), Color.Ivory);
            
            //render a small circle to depict which turret is selected (graphic of where user(friendly) missiles fire from)
            switch (_turretNumb)
            {
                case 1:
                    _canvas.AddCenteredEllipse((_canvas.ScaledWidth / 4), _canvas.ScaledHeight, 10, 10, Color.Yellow);
                    break;
                case 2:
                    _canvas.AddCenteredEllipse(_canvas.ScaledWidth / 2, _canvas.ScaledHeight, 10, 10, Color.Yellow);
                    break;
                case 3:
                    _canvas.AddCenteredEllipse(_canvas.ScaledWidth - (_canvas.ScaledWidth / 4), _canvas.ScaledHeight, 10, 10, Color.Yellow);
                    break;
            }

            _canvas.Render();
            //create a new list of missile collisions (between user and foe)
            List<Missile> hits = _missleA.Intersect(_missileF).ToList();
            

            //count the number of enemy missiles
            count1 = _missileF.Count;
            //compare enemy missiles to the lits of 'hits' removing any missiles that meet conditions
            foreach (Missile hit in hits)
            {
                while (_missileF.Remove(hit)) ;
                
            }
            //recount the number of enemy missiles
            count2 = _missileF.Count;
            //score will increase if there is a difference between count1 & count2
            if (count2 < count1)
            {
                _missilesKilled += count1-count2;
                _score += (count1 - count2) * 100;
            }
            //remove enemy missiles that reach the left or right edge of the GDI canvas
            for (int c = 0; c < _missileF.Count; ++c)
            {
                if ((_missileF[c].Where().X <= 0) || (_missileF[c].Where().X >= _canvas.ScaledWidth))
                {
                    _missileF.RemoveAt(c);
                }
            }

            //missiles that reach the bottom of the GDI window are removed, also lower users 'life'
            for (int c = 0; c < _missileF.Count; ++c)
            {
                if ((_missileF[c].Where().Y >= _canvas.ScaledHeight))
                {
                    --_lives;
                    _missileF.RemoveAt(c);
                }
            }
            //user will gain ammo over time based on resupply amount
            _resupply += 1;
            if (_resupply >= 30)
            {
                //standard missile ammo increases by 1 whereas 'flak' gains 3
                ++_ammo;
                _flakAmmo += 3;
                _resupply = 0;
            }
            //when lives run out switch the gamestate thus stopping this game loop
            if (_lives <= 0)
            {
                _gameState = eGameState.Over;

            }
            _uiLblFKilled.Text = _missilesKilled.ToString();
            _uiLblFMissile.Text = _flakMShot.ToString();
            _uiLblRegMissile.Text = _missileShot.ToString();
        }

       
        #endregion


        //a way to start, stop & pause game needed
        //timer default 100ms - user can adjust in +/- 10ms increments

        //enum eGameState {paused, Unstarted, Over, Running}
        //instructions to be shown when 'unstarted' active
        //unique messages for paused and over

        //processing to include: game over? less than 5 missiles add 1
        //user clicked drawer create and send missile
        //move all incoming, check/remove any side boundary crossing
        //remove any missiles hitting ground (and decrease player life)
        //move all friendly missiles, remove exploded missiles
        //remove all foe missiles hitting friendly missiles
        //add 100 to score for each missile destroyed
        //render all objects including score
        //stats to be shown in main window
    }
}
