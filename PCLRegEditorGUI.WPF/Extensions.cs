namespace PCLRegEditorGUI.WPF;

public static class Extensions
{
    public static void UpdateTo<T>(this IList<T> collection, IList<T> newValues)
    {
        if (collection is null || newValues is null)
            return;
        int nowCount = collection.Count;
        int newCount = newValues.Count;
        int min = Math.Min(nowCount, newCount);
        int i = 0;
        while (i < min)
        {
            collection[i] = newValues[i];
            i++;
        }
        while (i < newCount)
            collection.Add(newValues[i++]);
        while (i < nowCount)
            collection.RemoveAt(--nowCount);
    }
}
