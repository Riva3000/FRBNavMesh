using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FRBNavMesh.GenericsTests
{
    public interface INode<ITLink> { }

    public interface ILink<ITNode> { }

    public class Node<TNode,TLink> : INode<TLink> where TLink : class, ILink<TNode>, new()
    {
        TLink btvar;
    }

    public class Link<TLink,TNode> : ILink<TNode> where TNode : class, INode<TLink>, new()
    {
        TNode atvar;
    }

    public class ClassADerivedClosed : Node<ClassADerivedClosed, ClassBDerivedClosed> { }

    public class ClassBDerivedClosed : Link<ClassBDerivedClosed, ClassADerivedClosed> { }
}
