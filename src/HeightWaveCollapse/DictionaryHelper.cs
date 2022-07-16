namespace HeightWaveCollapse
{
	internal static class DictionaryHelper
	{
		public static TValue ForceGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValue) where TKey : notnull =>
		dictionary.TryGetValue(key, out var value) ? value : dictionary[key] = defaultValue();
	}
}
