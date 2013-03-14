using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FLBS14.Battle.Interface;

namespace FLBS14.Battle
{
    public class Party 
    {
        public List<IPartyMember> Members { get { return members; } }
        public List<Party> FriendlyParties { get { return friendlyParties; } }
        public List<Party> EnemyParties { get { return enemyParties; } }

        public string Name { get { return name;}}

        public Party(string name)
        {
            this.name = name;
        }

        #region private
        private string name;

        private List<IPartyMember> members = new List<IPartyMember>();
        private List<Party> friendlyParties = new List<Party>();
        private List<Party> enemyParties = new List<Party>();
        #endregion
    }
}
