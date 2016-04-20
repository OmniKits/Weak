public static class DelegateTargetEx
{
    public static DelegateTarget<T> ToWeakDelegateTarget<T>(this T target, bool trackResurrection = false)
        where T : class
        => new DelegateTarget<T>(target, trackResurrection);
}
