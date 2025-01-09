﻿using XPrism.Core.DI;

namespace XPrism.Core.DataContextWindow;

[AttributeUsage(AttributeTargets.Class)]
public class XPrismBaseAttribute : Attribute {
    public ServiceLifetime Lifetime { get; }

    public XPrismBaseAttribute(ServiceLifetime lifetime) {
        Lifetime = lifetime;
    }
}