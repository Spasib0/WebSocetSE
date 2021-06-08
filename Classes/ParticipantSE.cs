using System.Collections.Generic;

namespace WebSocketSE
{
	public class ParticipantSE
	{
        public string name;
        public int room;
        public bool admin;
        public string[] authorities;
        public string id;

        public ParticipantSE(string name, int room, bool admin, string[] authorities, string id)
        {
            this.name = name;
            this.room = room;
            this.admin = admin;
            this.authorities = authorities;
            this.id = id;
        }
    }
}

