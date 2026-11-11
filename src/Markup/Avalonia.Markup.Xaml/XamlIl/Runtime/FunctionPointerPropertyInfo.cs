using System;
using Avalonia.Data.Core;

namespace Avalonia.Markup.Xaml.XamlIl.Runtime;

internal sealed unsafe class FunctionPointerPropertyInfo : IPropertyInfo
{
    private readonly IntPtr _getter;
    private readonly IntPtr _setter;

    public FunctionPointerPropertyInfo(string name, IntPtr getter, IntPtr setter, Type propertyType)
    {
        Name = name;
        _getter = getter;
        _setter = setter;
        PropertyType = propertyType;
    }

    public string Name { get; }
    public Type PropertyType { get; }
    public bool CanGet => _getter != IntPtr.Zero;
    public bool CanSet => _setter != IntPtr.Zero;

    public object? Get(object target)
    {
        if (_getter == IntPtr.Zero)
            throw new NotSupportedException("Property " + Name + " doesn't have a getter");
        return ((delegate* managed<object, object?>)_getter.ToPointer())(target);
    }

    public void Set(object target, object? value)
    {
        if (_setter == IntPtr.Zero)
            throw new NotSupportedException("Property " + Name + " doesn't have a setter");
        ((delegate* managed<object, object?, void>)_setter.ToPointer())(target, value);
    }
}

internal sealed unsafe class FunctionPointerPropertyInfo<TSource, TValue> : IPropertyInfo<TSource, TValue>
    where TSource : class
{
    private readonly IntPtr _getter;
    private readonly IntPtr _setter;

    public FunctionPointerPropertyInfo(string name, IntPtr getter, IntPtr setter)
    {
        Name = name;
        _getter = getter;
        _setter = setter;
    }

    public string Name { get; }
    public Type PropertyType => typeof(TValue);
    public bool CanGet => _getter != IntPtr.Zero;
    public bool CanSet => _setter != IntPtr.Zero;

    public TValue Get(TSource target)
    {
        if (_getter == IntPtr.Zero)
            throw new NotSupportedException("Property " + Name + " doesn't have a getter");
        return ((delegate* managed<TSource, TValue>)_getter.ToPointer())(target);
    }

    public void Set(TSource target, TValue value)
    {
        if (_setter == IntPtr.Zero)
            throw new NotSupportedException("Property " + Name + " doesn't have a setter");
        ((delegate* managed<TSource, TValue, void>)_setter.ToPointer())(target, value);
    }

    object? IPropertyInfo.Get(object target) => Get((TSource)target);
    void IPropertyInfo.Set(object target, object? value) => Set((TSource)target, (TValue)value!);
}
