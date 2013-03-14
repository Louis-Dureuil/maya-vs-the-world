using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Battle.Interface;

namespace FLBS14.Battle
{
    public class BattleSituation
    {
        public List<IActive> Actors { get { return actors; } }
        public List<IMovable> Movables { get { return movables; } }
        public List<ITargetable> Targetables { get { return targetables; } }
        public List<ICommander> Commanders { get { return commanders; } }
        public float Ap 
        { 
            get { return ap; } 
            set 
            {
                float oldAp = ap;
                ap = value;
                apFlow = oldAp - ap;
            } 
        }
        public float ApFlow { get { return apFlow; } }
        public float Speed { get { return speed; } set { speed = value; } }


        #region private
        private List<IActive> actors = new List<IActive>();
        private List<IMovable> movables = new List<IMovable>();
        private List<ITargetable> targetables = new List<ITargetable>();
        private List<ICommander> commanders = new List<ICommander>();
        private float ap;
        private float apFlow;
        private float speed = GameConstants.Speed["Interactive"];
        #endregion
    }
}
