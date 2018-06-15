using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh.GenericsTests
{
    // --- Sample
    /*
    public abstract class BaseManager<TComponent, TManager>
    where TComponent : BaseComponent<TComponent, TManager>
    where TManager : BaseManager<TComponent, TManager>
    { }

    public abstract class BaseComponent<TComponent, TManager>
    where TComponent : BaseComponent<TComponent, TManager>
    where TManager : BaseManager<TComponent, TManager>
    { }

    // So then you'd have:

    public class PhysicsManager : BaseManager<PhysicsComponent, PhysicsManager>

    public class PhysicsComponent : BaseComponent<PhysicsComponent, PhysicsManager>
    */
    // --- My implementation

    public abstract class PositionedNode<TLink, TNode>
    where TLink : Link<TLink, TNode>
    where TNode : PositionedNode<TLink, TNode>
    {
    }

    public abstract class Link<TLink, TNode>
    where TLink : Link<TLink, TNode>
    where TNode : PositionedNode<TLink, TNode>
    {
    }

    public class MyNode : PositionedNode<MyLink, MyNode>
    {
    }

    public class MyLink : Link<MyLink, MyNode>
    {
    }
}
