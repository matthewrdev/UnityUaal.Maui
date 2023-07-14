namespace UnityUaalMaui.Unity
{
    public class UnityContentReceivedEventArgs : EventArgs
	{
        public UnityContentReceivedEventArgs(string eventName, string eventContent)
        {
            EventName = eventName;
            EventContent = eventContent;
        }

		public string EventName { get; }

		public string EventContent { get; }
	}
}

