using System;

public abstract class Reference<T> : WeakReference
    where T : class
{
    public override object Target
    {
        get { return base.Target; }
        set { base.Target = (T)value; }
    }

    internal Reference(T target, bool trackResurrection)
        : base(target, trackResurrection)
    { }
}
