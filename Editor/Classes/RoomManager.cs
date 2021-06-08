using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketSE
{
	public class RoomManager
	{
		private static List<Room> rooms = new List<Room>();
		//private static Predicate<Room> compareIds = delegate (string id) {return id => room.id == id };


		//Если в приложении несколько комнат одновременно 
		#region 
		public Room this[int index] => rooms[index];

		/*public static int IndexOf(Room _room)
        {
			return rooms.FindIndex(room => room.id == _room.id);
        }*/

		public static int FindIndex(Predicate<Room> match)
        {
			return rooms.FindIndex(match);
		}
        #endregion


        public static void UpdateRoom(EventSE eventObj)
        {
			int index = FindIndex(room => room.id == eventObj.RoomId);

			if(index < 0)
            {
                rooms.Add(new Room(eventObj.RoomId));
				UpdateRoom(eventObj);
			}
            else
            {
				rooms[index].SetUsers(eventObj);
            }
		}

		public class Room
		{
			public string id;
			public int index;
			public List<ParticipantSE> users;
			

			public Room(string id)
			{
				this.id = id;
				users = new List<ParticipantSE>();
				rooms.Add(this);
				index = FindIndex(room => room.id == id);
			}

			public void SetUsers(IType eventObj)
			{
				eventObj.SetUpdatedUsers(users);
				users = eventObj.Participants;
			}

			
		}
	}
}
