using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketSE
{
    public class EventSE
    {
        private static List<TypeSE> serviceTypes = new List<TypeSE> { TypeSE.ServerResponse, TypeSE.Disconnect, TypeSE.Kick, TypeSE.Close };
        private static List<TypeSE> roomTypes = new List<TypeSE> { TypeSE.ParticipantEnter, TypeSE.ParticipantExit, TypeSE.RolesApplied };

        private static List<TypeSE> dataTypes = System.Enum.GetValues(typeof(TypeSE)).Cast<TypeSE>().Where(value => !serviceTypes.Contains(value)).ToList();
        private static List<TypeSE> titledTypes = new List<TypeSE> { TypeSE.Broadcast, TypeSE.Direct, TypeSE.Petition};

        public JObject _jobject;

        public TypeSE Type => type;
        private TypeSE type;

        public List<ParticipantSE> Participants => participants;
        private List<ParticipantSE> participants;
        public List<ParticipantSE> UpdatedUsers { get; private set; }

        public string RoomId => roomId;
        private string roomId;

        private MessageData messageData;
        private ControlData controlData;

        private JToken jdata;
        
        public bool HasUserUpdates = false;

        public EventSE(string data)
        {
            _jobject = JObject.Parse(data);
            type = _jobject["type"].ToObject<TypeSE>();

            jdata = dataTypes.Contains(type) ? _jobject["data"] : null;
            roomId = roomTypes.Contains(type) ? (string)_jobject["id"] : "-1";

            if (roomTypes.Contains(type) || type == TypeSE.Control || type == TypeSE.Kick)
            {
                HasUserUpdates = true;
                participants = jdata["participants"].ToObject<List<ParticipantSE>>();
            }

            messageData = new MessageData(titledTypes.Contains(type) ? jdata["data"] : null);
            controlData = new ControlData(type == TypeSE.Control ? jdata["data"] : null);
        }

        public static EventSE Create(string data)
        {
            return new EventSE(data);
        }

        public void SetUpdatedUsers(List<ParticipantSE> oldUsers)
        {
            UpdatedUsers = GetUpdated(oldUsers);
        }

        private List<ParticipantSE> GetUpdated(List<ParticipantSE> oldUsers)
        {
            //todo swich по Type
            return Participants.Where(current => !oldUsers.Any(old => current.id == old.id)).Concat(oldUsers.Where(old => !Participants.Any(current => old.id == current.id))).ToList();
        }



        public enum TypeSE
        {
            ParticipantEnter = 0,   //  jdata               roomID  users
            ParticipantExit = 1,    //  jdata               roomID  users
            RolesApplied = 2,       //  jdata               roomID  users
            ServerResponse = 3,     //          service
            Broadcast = 4,          //  jdata                               text
            Direct = 5,             //  jdata                       users   text
            Control = 6,            //  jdata                       users
            Petition = 7,           //  jdata                               text
            Disconnect = 8,         //          service
            Kick = 9,               //          service             users
            Close = 10              //          service
        }

        
    }

    public class MessageData
    {
        string title;
        string text;

        public MessageData(string title, string text)
        {
            this.title = title;
            this.text = text;
        }

        public MessageData(JToken data)
        {
            if (data != null)
            {
                title = ((JObject) data).Properties().FirstOrDefault().Name;
                text = (string) data[title];
            }
            else
            {
                title = text = "";
            }
        }
    }

    public class ControlData
    {
        bool allow;
        string[] authorities;

        public ControlData(JToken data)
        {
            if (data != null)
            {
                allow = (bool) data["allow"];
                authorities = data["authorities"].Values<string>().ToArray();
            }
            else
            {
                allow = false;
                authorities = new string[0];
            }
        }
    }
}
