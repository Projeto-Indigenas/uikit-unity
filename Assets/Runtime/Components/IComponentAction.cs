using System;

namespace UIKit.Components
{
    public interface IComponentAction
    {
        Action action { get; set; }
    }

    public interface IComponentAction<TParameter>
    {
        Action<TParameter> action { get; set; }
    }
}
