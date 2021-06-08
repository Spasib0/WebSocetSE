using System;
using System.Collections.Generic;
using static WebSocketSE.EventSE;

namespace WebSocketSE
{
    public interface IParticipants : IType
    {
        //HasUpdate = true, с RoomManager можно выставлять false, во время сравнения изменилось ли что-то (мб полезно для Гуи, сообщений много и не все "полезные")
        //bool HasUpdate { get; set; }    //TODO Conditions для сравнений для разных типов мы не знаем, надо разобраться с Predicate и как их предоставить процессору
        //List<ParticipantSE> Participants { get; }

        List<ParticipantSE> GetParticipants();
        //Для конструкций связанных с ParticipantSE полезным будет знать для каких именно пользователей пришли обновления
        //List<ParticipantSE> UpdatedParticipants { get; }

        //void SetUpdated(List<ParticipantSE> value);
    }
}
