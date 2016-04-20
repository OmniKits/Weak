using System;

public class DelegateTarget<T> : Reference<T>
    where T : class
{
    new public T Target => (T)base.Target;

    public DelegateTarget(T target, bool trackResurrection = false)
        : base(target, trackResurrection)
    { }

    public Action GetAction(Action<T> proto)
    {
        if (proto.Target == base.Target)
            throw new ArgumentException();
        return () => proto(Target);
    }
    public Action<TArgs> GetAction<TArgs>(Action<T, TArgs> proto)
    {
        if (proto.Target == base.Target)
            throw new ArgumentException();
        return args => proto(Target, args);
    }
    public Func<TResult> GetFunc<TResult>(Func<T, TResult> proto)
    {
        if (proto.Target == base.Target)
            throw new ArgumentException();
        return () => proto(Target);
    }
    public Func<TArgs, TResult> GetFunc<TArgs, TResult>(Func<T, TArgs, TResult> proto)
    {
        if (proto.Target == base.Target)
            throw new ArgumentException();
        return args => proto(Target, args);
    }
    public EventHandler<TArgs> GetHandler<TArgs>(Action<T, object, TArgs> proto)
        where TArgs : EventArgs
    {
        if (proto.Target == base.Target)
            throw new ArgumentException();
        return (sender, args) => proto(Target, sender, args);
    }
    public EventHandler<TArgs> GetHandler<TArgs>(EventHandler<TArgs> proto)
       where TArgs : EventArgs
    {
        if (proto.Target != base.Target)
            throw new ArgumentException();
        var mi = proto.Method;
        return GetHandler<TArgs>((target, sender, args) =>
        {
            if (target == null) return;
            mi.Invoke(target, new[] { sender, args });
        });
    }

    public EventHandler GetHandler(Action<T, object, EventArgs> proto)
    {
        if (proto.Target == base.Target)
            throw new ArgumentException();
        return (sender, args) => proto(Target, sender, args);
    }
    public EventHandler GetHandler(EventHandler proto)
    {
        if (proto.Target != base.Target)
            throw new ArgumentException();
        var mi = proto.Method;
        return GetHandler((target, sender, args) =>
        {
            if (target == null) return;
            mi.Invoke(target, new[] { sender, args });
        });
    }
}
