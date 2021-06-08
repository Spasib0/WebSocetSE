using System;
using static WebSocketSE.EventSE;

namespace WebSocketSE {
	public interface IEventBase
	{
		TypeSE Type { get; }
	} 
}
