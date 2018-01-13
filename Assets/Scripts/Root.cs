using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;
using strange.extensions.context.api;

namespace Approximator
{
    public class Root : ContextView
    {
        void Awake()
        {
            context = new MainConext(this);
        }
    }
}