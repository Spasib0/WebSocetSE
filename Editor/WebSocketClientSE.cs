using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static WebSocketSE.EventSE.TypeSE;

namespace WebSocketSE
{
	public class WebSocketClientSE
	{
		public bool TASK_DEBUG;

		private ClientWebSocket _client;
		private Uri _url;
		private CancellationTokenSource _cancellation;

		public Action<EventSE> OnEventReceive;

		public WebSocketClientSE(Uri url)
        {
			_url = url;
			_cancellation = new CancellationTokenSource();
			_client = new ClientWebSocket();
		}


		public WebSocketState GetState()
        {
			return _client.State;
        }

		public void SetRequestHeader(string name, string value)
		{
			_client.Options.SetRequestHeader(name, value);
		}

		public Task SendMessage(EventSE message)
		{
			//TaskDebug(Task.CurrentId.ToString(), "Send", $"{message._jobject}");
			byte[] bytes = Encoding.UTF8.GetBytes(message._jobject.ToString());
			return _client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cancellation.Token);
		}


		public async void Connect()
        {

			//TaskDebug(Task.CurrentId.ToString(), "Con", $"Start");

			await _client.ConnectAsync(_url, _cancellation.Token).ConfigureAwait(false);

			while (true) //вроде нет смысла лишний раз чота проверять _client.State != WebSocketState.None
			{
				//TaskDebug(Task.CurrentId.ToString(), "Con", $"Call");
				var data = await Task.Run(GetData);
                ProcessData(data);
				//TaskDebug(Task.CurrentId.ToString(), "Con", $"Call finished");
			}
		}

		// todo cancel task 
		private async Task<EventSE> GetData()
		{
			//TaskDebug(Task.CurrentId.ToString(), "Rcv", $"Start");
			var bytes = new byte[1024];
			var buffer = new ArraySegment<byte>(bytes);
			string data = "";

			while (true)
			{
				//TaskDebug(Task.CurrentId.ToString(), "Rcv", $"Call");
				var recievedData = await _client.ReceiveAsync(buffer, _cancellation.Token);
				byte[] dataBytes = buffer.Skip(buffer.Offset).Take(recievedData.Count).ToArray();
				data += Encoding.UTF8.GetString(dataBytes);

				if (recievedData.EndOfMessage)
				{
					//TaskDebug(Task.CurrentId.ToString(), "Rcv", $"Received: {data}");

					return new EventSE(data);
				}
			}
		}

		//Интерфейс на процессор
		private void ProcessData(EventSE eventData)
        {
			//TaskDebug(Task.CurrentId.ToString(), "Prc", $"{eventData._jobject} 1");
			var Jobject = JObject.Parse(eventData._jobject.ToString());
			var messageType = Jobject["type"].ToObject<EventSE.TypeSE>();

            if (eventData.HasUserUpdates)
            {
				RoomManager.UpdateRoom(eventData);
			}

			TaskDebug(Task.CurrentId.ToString(), "Prc", $"[{eventData.Type}]{eventData._jobject}");
			OnEventReceive?.Invoke(eventData);
		}

		//private int lenTask = 12, lenTread = 30;
		
		private void TaskDebug(string task, string scope, string debugText)
        {
			string tread = $"Tread: {Thread.CurrentThread.ManagedThreadId}";
			task = $"Task: {task}";

			/*if (tread.Length > lenTread) 
				lenTread = tread.Length;

			while (tread.Length < lenTread)
				tread = " " + tread;


			if (task.Length > lenTask)
				lenTask = task.Length;

			while (task.Length < lenTask)
				task += " ";*/

			Debug.Log($"{tread}|{task}|{scope}|{tread.Length}|{debugText}");
        }

	}
}




