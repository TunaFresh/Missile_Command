//////////////////////////////////////////////////////////////////////////////
// Project : Lab 02 - Missile Command 
// Submission code: 1201_2300_L02
// November 2 2020
// By Alan May
// ///////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDIDrawer;
using Microsoft.SqlServer.Server;

namespace AlanMay_Lab02_Missile_Command
{
    class Missile
    {
        //point origin of missile
        public Point _source = new Point();

        //double(3)
        //double angle, path length, height(or altitude) of destination
        private double mAngle;
        private double pLength;
        public double destination;


        //int(3)
        //int radius(5), alpha value(255), speed
        private int _radius = 5;
        private int _alpha = 255;
        private int _speed;


        //alpha depicts a friendly missile fading out of existence after exploding
        public int Alpha
        {
            get
            {
                return _alpha;
            }

        }


        //enum used to track launch point and types of missiles
        public enum MissileType
        {
            Foe,
            //ASD a the end of missile name used to identify which turret missile launches from
            FlakA,
            FlakS,
            FlakD,
            MissileA,
            MissileS,
            MissileD
        }

        public MissileType MType
        {
            set
            {
                _mType = value;

            }
        }
        //instead of using a bool to determine if an enemy is a friend of foe
        //an enum was used due to the number of differnt types of missile
        //and launch points
        private MissileType _mType;
        //bool automatic property hidden(private) set, public get
        // public bool Foe { get; private set; }
        //represents friend or foe
        //public bool Dead { get; private set; }

        private static CDrawer _canvas = null;


        //static CDrawer set only, this ties _canvas on main form to _canvas instances in class
        public static CDrawer Canvas
        {
            set
            {
                _canvas = value;
            }
        }

        //static int set only manual, explosion radius

        //static random field CDrawerer null explosion radius 50
        static Random _rng = new Random();


        //explosion radius. When missiles reach destination altitude (or for flak missiles max range)
        static int _exRadius = 50;


        //default constructor used to construct enemy(foe) missiles
        //missiles start at top of GDI window fly downward
        //source point (starting position) set to random (anywhere across top GDI window)
        //troubleshooting tip start 1/4 of the way down to check trajectories
        //initilize angle flightpath to a random angle between 3pi/4 and 5pi/4
        //set length to 5. Y destination to 0(bottom) speed 5 and flag (bool) set to foe

        /// <summary>
        /// generate 'friendly' missiles. Missiles will be shot from 1 of 3 locations
        /// and be 1 of 2 types. This is determined by enum MissileType
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="DestinationX">x plane coordinate of destination</param>
        /// <param name="DestinationY">y plane coordinate of destination</param>
        /// <param name="speed"> speed missile travels</param>
        /// <param name="MissileType">enum to determine type and launch point of missile</param>
        public Missile(double angle, double DestinationX, double DestinationY, int speed, Missile.MissileType MissileType)
        {
            //public Missile(double angle, Point Destination, int speed, Missile.MissileType MissileType)
            //{

            //sets missile type _mType property based on passed in value
            MType = MissileType;
            destination = DestinationY;
            double angleCalc = 0;
            //_source.X = (int)(_rng.NextDouble() * (_canvas.ScaledWidth+(_radius*2))-(_radius*2));

            //shoot missiles from 'left' turret
            if (MissileType == MissileType.FlakA || MissileType == MissileType.MissileA)
            {
                _source.X = _canvas.ScaledWidth / 4;
                //_source.Y = _canvas.ScaledHeight;
                //destination = Y;
                angleCalc = Math.Atan(-1 * (DestinationX - (_canvas.ScaledWidth / 4)) / (DestinationY - _canvas.ScaledHeight));
            }//middle turret missiles
            else if (MissileType == MissileType.FlakS || MissileType == MissileType.MissileS)
            {
                _source.X = _canvas.ScaledWidth / 2;
                //_source.Y = _canvas.ScaledHeight;
                // destination = Destination.Y;
                //Console.WriteLine($"x{_source.X},y:{_source.Y}");
                angleCalc = Math.Atan(-1 * ((DestinationX - (_canvas.ScaledWidth / 2)) / (DestinationY - _canvas.ScaledHeight)));
            }
            else//right turret missiles
            {

                angleCalc = Math.Atan(-1 * (DestinationX - (_canvas.ScaledWidth - (_canvas.ScaledWidth / 4))) / (DestinationY - _canvas.ScaledHeight));
                _source.X = _canvas.ScaledWidth - (_canvas.ScaledWidth / 4);
                
                //destination = Destination.Y;
            }

            _source.Y = _canvas.ScaledHeight;
            mAngle = angleCalc;
            //Foe = false;
            pLength = 5;
            _radius = 5;
            _speed = speed;

        }

        /// <summary>
        /// enemy missiles are created using this constructor
        /// </summary>
        public Missile()
        {
            //generate a random angle which determines the trajectory of a missile
            mAngle = ((_rng.NextDouble() * (((5 * Math.PI) / 4) - ((3 * Math.PI) / 4))) + ((3 * Math.PI) / 4));
            destination = 0;
            _source.X = (int)(_rng.NextDouble() * (_canvas.ScaledWidth + (5 * 2)) - (5 * 2));
            //_source.Y = 100;
            //set _mType missile to Foe
            _mType = MissileType.Foe;
            _speed = 5; //how far the missile travels each 'timer tick'
        }

        /// <summary>
        /// determines the current location of a missile and returns that value
        /// </summary>
        /// <returns></returns>
        public Point Where()
        {
            Point Here = new Point();
            Here.X = (int)((pLength * Math.Sin(mAngle)) + _source.X);
            Here.Y = (int)(pLength * -1 * Math.Cos(mAngle) + _source.Y);
            //Console.WriteLine($"fx{Here.X}");
            return Here;
        }

        //friendly missile constructor
        //requires a point for a destination, source point set to middle of screen (bottom)
        //initilize source point as the turret
        //calculate angle from source point to destination. when Missile reaches Y destination
        //explode
        //set radius to 10 and speed to 20 bool to friendly

        //create method Where() return a point (where missile currently is located)
        //calculate missile location based on an angle (not x/y locations)

        //method Move()
        //evaluate move and adjust missile for every step of the game
        //foe add current speed to length
        //friend determine missile state & location
        //use utility function to determine location
        //if destination altitude is achieved cease movement and 'explode'
        //increase explosion radius by 5 until explosion radius threshold is reached
        //once threshold is achieved decrease alpha value by 10
        //missile is removed based on alpha value via predicate
        /// <summary>
        /// Moves missiles
        /// for enemy missiles just adds _speed (speed of missile) to length
        /// for friendly missiles, explodes missiles if they reach destination (click Y altitude (or for flak a max distance (if reacher sooner))
        /// for friendly missiles that have hit the destination they 'explode' and then fade out
        /// so first increase missile Radius, then once radius => _exRadius decrment alpha to cause them to fade from view
        /// </summary>
        public void Move()
        {
            if (_mType == MissileType.Foe)
            {
                pLength += _speed;
            }
            else//friendly missile 
            {
                // right click missiles, either move, explode (increase radius) or fade (decrease alpha value - causeing them to fade)
                if (_mType == MissileType.MissileA || _mType == MissileType.MissileD || _mType == MissileType.MissileS)
                {
                    if (this.Where().Y < destination)
                    {
                        if (this._radius >= _exRadius)
                        {
                            //this._alpha -= 5;

                            this._alpha -= 10;
                        }
                        else
                        {

                            this._speed = 0;
                            this._radius += 5;
                        }
                        //Die();
                    }
                    else
                    {
                        pLength += _speed;
                    }
                }
                else//flak missiles 
                {
                    if (this.Where().Y < _canvas.ScaledHeight - _canvas.ScaledHeight / 3)//max y value 'height' reached
                    {
                        FlakDie();
                    }
                    //flak missiles fired from left turret
                    if (_mType == MissileType.FlakA)
                    {
                        //max left and right range left/right range
                        if (this.Where().X > _canvas.ScaledWidth / 4 + 200 || this.Where().X < _canvas.ScaledWidth / 4 - 150)
                        {
                            FlakDie();
                        }
                    }
                    else if (_mType == MissileType.FlakD) //right turret flak missiles left/right range
                    {
                        if (this.Where().X > _canvas.ScaledWidth - (_canvas.ScaledWidth / 4) + 150 || this.Where().X < _canvas.ScaledWidth - (_canvas.ScaledWidth / 4) - 200)
                        {
                            FlakDie();
                        }
                    }//middle turret missiles left/right range
                    else if (_mType == MissileType.FlakS)
                    {
                        if (this.Where().X > _canvas.ScaledWidth / 2 + 200 || this.Where().X < _canvas.ScaledWidth / 2 - 200)
                        {
                            FlakDie();
                        }
                    }
                    if (this.Where().Y < destination)
                    {
                        FlakDie();
                    }
                }
                pLength += _speed;
            }
        }

        /// <summary>
        /// explodes (increase radius of flak missile) or fades (decrease Alpha value) a flak missile
        /// </summary>
        private void FlakDie()
        {
            if (this._radius >= (_exRadius / 5))
            {
                //this._alpha -= 5;

                this._alpha -= 10;
            }
            else
            {

                this._speed = 0;
                this._radius += 5;
            }
        }
        //determine if a missile hits a foe(s)
        //use equals override to determine if a missile is hit (pythagoras)
        //enemy missiles do not have any effect if they explode, they land and are no
        //longer in play. A friendly missile could hit multiple missiles while traveling
        //to target altitude - an enemy missile will never stop a friendly missile


        //create a method to render missiles. Foes are red & friendlies in green
        //use utility method to determine current location
        //use AddLine & AddCenteredEllipse to draw the action
        public void Render()
        {
            //_canvas.AddCenteredEllipse((int)(pLength * Math.Sin(mAngle)) + _source.X, (int)(pLength *-1* Math.Cos(mAngle)) + _source.Y,_radius*2,_radius*2,Color.Red);
            //_canvas.AddLine(_source.X,_source.Y, (int)(pLength * Math.Sin(mAngle)) + _source.X, (int)(pLength *-1* Math.Cos(mAngle)) + _source.Y,Color.Red);
            //_canvas.AddCenteredEllipse((int)(Where)

            //note The colors pink, Blue & Yellow were chosen to try to make the program more friendly for individuals that are color blind
            //source https://davidmathlogic.com/colorblind/#%23D81B60-%231E88E5-%23FFC107-%23004D40
            Color color = new Color();
            if (_mType == MissileType.Foe)
            {
                _canvas.AddCenteredEllipse(Where().X, Where().Y, _radius * 2, _radius * 2, Color.Pink);
                _canvas.AddLine(_source.X, _source.Y, Where().X, Where().Y, Color.Pink);
            }
            else
            {
                //Console.WriteLine(_radius*2);
                if (_mType == MissileType.MissileA || _mType == MissileType.MissileD || _mType == MissileType.MissileS)
                {
                    color = Color.Blue;
                }
                else
                {
                    color = Color.Yellow;
                }
                _canvas.AddCenteredEllipse(Where().X, Where().Y, _radius * 2, _radius * 2, Color.FromArgb(_alpha, color));
                _canvas.AddLine(_source.X, _source.Y, Where().X, Where().Y, color);
            }
        }

        /// <summary>
        /// used to determine the distance between 2 missiles
        /// by calling (where() for both missiles being compared
        /// modified from ICA05
        /// </summary>
        /// <param name="tMissile"></param>
        /// <returns></returns>
        public float GetDistance(Missile tMissile)
        {
            //get location of missiles (call GetLocation())
            //use those values to calculate distance between objects
            //return distance
            //adapted from ICA05
            float distance = 0;
            float xDel = tMissile.Where().X - Where().X;
            float yDel = tMissile.Where().Y - Where().Y;
            distance = (float)(Math.Pow((xDel), 2) + Math.Pow((yDel), 2));
            distance = (float)Math.Sqrt(distance);
            return distance;

        }

        //override equals lifted from ICA05
        /// <summary>
        /// returns a value greater than -1 if an overlap of a friendly and enemy missile occurs
        /// uses getdistance to assist with this
        /// Equals Method modified from ICA05 - Be ordering my balls
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Missile arg)) return false;
            //Console.WriteLine($"rad:{this._radius} arg rad:{arg._radius} dis:{this.GetDistance(arg)}");
            return (this._radius + arg._radius - this.GetDistance(arg) > -1);
        }
        //gethashcode
        public override int GetHashCode()
        {
            return 1;
        }
    }
}
