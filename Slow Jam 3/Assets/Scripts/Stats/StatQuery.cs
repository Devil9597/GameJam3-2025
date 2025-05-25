namespace Systems.Stats
{
	public struct StatQuery<T>
		where T : struct
	{
		public delegate void Delegate(ref StatQuery<T> query);

		public T BaseValue { get; }
		public T Value { get; set; }

		public StatQuery(T baseValue)
		{
			this.BaseValue = baseValue;
			this.Value = baseValue;
		}
	}
}