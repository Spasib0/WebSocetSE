using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace WebSocketSE
{
    public abstract class EventSE2 : IResponce, IData, IParticipants
    {

        EventSE.TypeSE Type => {};

        public List<ParticipantSE> GetParticipants()
        {
            throw new System.NotImplementedException();
        }
    }

    public class EventSE : IResponce, IData, IParticipants
    {
        private static List<TypeSE> serviceTypes = new List<TypeSE> { TypeSE.ServerResponse, TypeSE.Disconnect, TypeSE.Kick, TypeSE.Close };
        private static List<TypeSE> roomTypes = new List<TypeSE> { TypeSE.ParticipantEnter, TypeSE.ParticipantExit, TypeSE.RolesApplied };

        private static List<TypeSE> dataTypes = System.Enum.GetValues(typeof(TypeSE)).Cast<TypeSE>().Where(value => !serviceTypes.Contains(value)).ToList();
        private static List<TypeSE> titledTypes = new List<TypeSE> { TypeSE.Broadcast, TypeSE.Direct, TypeSE.Petition};

        public JObject _jobject;

        public abstract JToken GetData();

        //IType
        public TypeSE Type => type;
        private TypeSE type;


        //IDataType
        private JToken jdata;


        //IParticipants
        public List<ParticipantSE> Participants => participants;
        private List<ParticipantSE> participants;




        //IUpdate (клиентский)
        public bool HasUpdate = false;
        public List<ParticipantSE> Updated { get; private set; }
        public void SetUpdated(List<ParticipantSE> oldUsers)
        {
            Updated = GetUpdated(oldUsers);
        }
        private List<ParticipantSE> GetUpdated(List<ParticipantSE> oldUsers)
        {
            //todo swich по Type
            return Participants.Where(current => !oldUsers.Any(old => current.id == old.id)).Concat(oldUsers.Where(old => !Participants.Any(current => old.id == current.id))).ToList();
        }




        //IRoom
        public string RoomId => roomId;
        private string roomId;




        //IMessage
        private MessageData messageData;
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
                    title = ((JObject)data).Properties().FirstOrDefault().Name;
                    text = (string)data[title];
                }
                else
                {
                    title = text = "";
                }
            }
        }




        //IControl
        private ControlData controlData;






        

        public EventSE(string data)
        {
            _jobject = JObject.Parse(data);

            //IResponce, IType
            type = _jobject["type"].ToObject<TypeSE>();



            //IDataType
            jdata = dataTypes.Contains(type) ? _jobject["data"] : null;

            //IRoomInfoType
            roomId = roomTypes.Contains(type) ? (string)_jobject["id"] : "-1";

            //IParticipantsType
            if (roomTypes.Contains(type) || type == TypeSE.Control || type == TypeSE.Kick)
            {
                HasUpdate = true;
                participants = jdata["participants"].ToObject<List<ParticipantSE>>();
            }

            messageData = new MessageData(titledTypes.Contains(type) ? jdata["data"] : null);
            controlData = new ControlData(type == TypeSE.Control ? jdata["data"] : null);
        }

        public static EventSE Create(string data)
        {
            return new EventSE(data);
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



    //IControl
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
