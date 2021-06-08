using Newtonsoft.Json.Linq;
using System;


namespace WebSocketSE
{
	public interface IData : IType
	{
		abstract IData GetData();
	}
}
