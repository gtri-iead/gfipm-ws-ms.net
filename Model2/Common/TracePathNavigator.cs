using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;

namespace Common
{
    [DebuggerDisplay("TraceXPathNavigator")]
    public class TraceXPathNavigator : XPathNavigator
    {
        // Fields
        private bool _closed;
        private TraceNode _current;
        private ElementNode _root;
        private XPathNodeType _state = XPathNodeType.Element;

        // Methods
        public void AddAttribute(string name, string value, string xmlns, string prefix)
        {
            if (!this._closed && (this.CurrentElement != null))
            {
                this.CurrentElement.Attributes.Add(new AttributeNode(name, prefix, value, xmlns, this.CurrentElement));
            }
        }

        public void AddComment(string text)
        {
            if (!this._closed && (this.CurrentElement != null))
            {
                this.CurrentElement.Add(
                    new CommentNode(
                        text, 
                        this.CurrentElement));
            }
        }

        public void AddElement(string prefix, string name, string xmlns)
        {
            if (!this._closed)
            {
                ElementNode node = new ElementNode(name, prefix, this.CurrentElement, xmlns);
                if (this.CurrentElement == null)
                {
                    this._root = node;
                    this._current = this._root;
                }
                else if (!this._closed)
                {
                    this.CurrentElement.Add(node);
                    this._current = node;
                }
            }
        }

        public void AddProcessingInstruction(string name, string text)
        {
            if (this.CurrentElement != null)
            {
                ProcessingInstructionNode node = new ProcessingInstructionNode(name, text, this.CurrentElement);
                this.CurrentElement.Add(node);
            }
        }

        public void AddText(string value)
        {
            if (!this._closed && (this.CurrentElement != null))
            {
                if (this.CurrentElement.TextNode == null)
                {
                    this.CurrentElement.TextNode = new TextNode(value, this.CurrentElement);
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    this.CurrentElement.TextNode.AddText(value);
                }
            }
        }

        public override XPathNavigator Clone()
        {
            return this;
        }

        public void CloseElement()
        {
            if (!this._closed)
            {
                this._current = this.CurrentElement.Parent;
                if (this._current == null)
                {
                    this._closed = true;
                }
            }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            return false;
        }

        public override string LookupPrefix(string ns)
        {
            return this.LookupPrefix(ns, this.CurrentElement);
        }

        private string LookupPrefix(string ns, ElementNode node)
        {
            string prefix = null;
            if (string.Compare(ns, node.NameSpace, StringComparison.Ordinal) == 0)
            {
                prefix = node.Prefix;
            }
            else
            {
                foreach (AttributeNode node2 in node.Attributes)
                {
                    if ((string.Compare("xmlns", node2.Prefix, StringComparison.Ordinal) == 0) && (string.Compare(ns, node2.NodeValue, StringComparison.Ordinal) == 0))
                    {
                        prefix = node2.Name;
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(prefix) && (node.Parent != null))
            {
                prefix = this.LookupPrefix(ns, node.Parent);
            }
            return prefix;
        }

        public override bool MoveTo(XPathNavigator other)
        {
            return false;
        }

        public override bool MoveToFirstAttribute()
        {
            if (this.CurrentElement == null)
            {
                return false;
            }
            bool flag = this.CurrentElement.MoveToFirstAttribute();
            if (flag)
            {
                this._state = XPathNodeType.Attribute;
            }
            return flag;
        }

        public override bool MoveToFirstChild()
        {
            if (this.CurrentElement == null)
            {
                return false;
            }
            bool flag = false;
            if ((this.CurrentElement.ChildNodes != null) && (this.CurrentElement.ChildNodes.Count > 0))
            {
                this._current = this.CurrentElement.ChildNodes[0];
                this._state = this._current.NodeType;
                return true;
            }
            if (((this.CurrentElement.ChildNodes == null) || (this.CurrentElement.ChildNodes.Count == 0)) && (this.CurrentElement.TextNode != null))
            {
                this._state = XPathNodeType.Text;
                this.CurrentElement.MovedToText = true;
                flag = true;
            }
            return flag;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        public override bool MoveToId(string id)
        {
            return false;
        }

        public override bool MoveToNext()
        {
            if (this.CurrentElement == null)
            {
                return false;
            }
            bool flag = false;
            if (this._state != XPathNodeType.Text)
            {
                ElementNode parent = this.CurrentElement.Parent;
                if (parent == null)
                {
                    return flag;
                }
                TraceNode node2 = parent.MoveToNext();
                if (((node2 == null) && (parent.TextNode != null)) && !parent.MovedToText)
                {
                    this._state = XPathNodeType.Text;
                    parent.MovedToText = true;
                    this._current = parent;
                    return true;
                }
                if (node2 != null)
                {
                    this._state = node2.NodeType;
                    flag = true;
                    this._current = node2;
                }
            }
            return flag;
        }

        public override bool MoveToNextAttribute()
        {
            if (this.CurrentElement == null)
            {
                return false;
            }
            bool flag = this.CurrentElement.MoveToNextAttribute();
            if (flag)
            {
                this._state = XPathNodeType.Attribute;
            }
            return flag;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        public override bool MoveToParent()
        {
            if (this.CurrentElement == null)
            {
                return false;
            }
            bool flag = false;
            switch (this._state)
            {
                case XPathNodeType.Element:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                    if (this._current.Parent != null)
                    {
                        this._current = this._current.Parent;
                        this._state = this._current.NodeType;
                        flag = true;
                    }
                    return flag;

                case XPathNodeType.Attribute:
                    this._state = XPathNodeType.Element;
                    return true;

                case XPathNodeType.Namespace:
                    this._state = XPathNodeType.Element;
                    return true;

                case XPathNodeType.Text:
                    this._state = XPathNodeType.Element;
                    return true;

                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    return flag;
            }
            return flag;
        }

        public override bool MoveToPrevious()
        {
            return false;
        }

        public override void MoveToRoot()
        {
            this._current = this._root;
            this._state = XPathNodeType.Element;
            this._root.Reset();
        }

        public override string ToString()
        {
            this.MoveToRoot();
            StringBuilder sb = new StringBuilder();
            new XmlTextWriter(new StringWriter(sb, CultureInfo.CurrentCulture)).WriteNode(this, false);
            return sb.ToString();
        }

        // Properties
        public override string BaseURI
        {
            get
            {
                return string.Empty;
            }
        }

        private CommentNode CurrentComment
        {
            get
            {
                return (this._current as CommentNode);
            }
        }

        private ElementNode CurrentElement
        {
            get
            {
                return (this._current as ElementNode);
            }
        }

        private ProcessingInstructionNode CurrentProcessingInstruction
        {
            get
            {
                return (this._current as ProcessingInstructionNode);
            }
        }

        public override bool IsEmptyElement
        {
            get
            {
                bool flag = true;
                if (this._current != null)
                {
                    flag = (this.CurrentElement.TextNode != null) || (this.CurrentElement.ChildNodes.Count > 0);
                }
                return flag;
            }
        }

        [DebuggerDisplay("")]
        public override string LocalName
        {
            get
            {
                return this.Name;
            }
        }

        [DebuggerDisplay("")]
        public override string Name
        {
            get
            {
                if (this.CurrentElement != null)
                {
                    switch (this._state)
                    {
                        case XPathNodeType.Element:
                            return this.CurrentElement.Name;

                        case XPathNodeType.Attribute:
                            return this.CurrentElement.CurrentAttribute.Name;

                        case XPathNodeType.ProcessingInstruction:
                            return this.CurrentProcessingInstruction.Name;
                    }
                }
                return string.Empty;
            }
        }

        [DebuggerDisplay("")]
        public override string NamespaceURI
        {
            get
            {
                if (this.CurrentElement != null)
                {
                    switch (this._state)
                    {
                        case XPathNodeType.Element:
                            return this.CurrentElement.NameSpace;

                        case XPathNodeType.Attribute:
                            return this.CurrentElement.CurrentAttribute.NameSpace;

                        case XPathNodeType.Namespace:
                            return null;
                    }
                }
                return string.Empty;
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return null;
            }
        }

        [DebuggerDisplay("")]
        public override XPathNodeType NodeType
        {
            get
            {
                return this._state;
            }
        }

        [DebuggerDisplay("")]
        public override string Prefix
        {
            get
            {
                string str = string.Empty;
                if (this.CurrentElement != null)
                {
                    switch (this._state)
                    {
                        case XPathNodeType.Element:
                            return this.CurrentElement.Prefix;

                        case XPathNodeType.Attribute:
                            return this.CurrentElement.CurrentAttribute.Prefix;

                        case XPathNodeType.Namespace:
                            return null;
                    }
                }
                return str;
            }
        }

        [DebuggerDisplay("")]
        public override string Value
        {
            get
            {
                if (this.CurrentElement != null)
                {
                    switch (this._state)
                    {
                        case XPathNodeType.Attribute:
                            return this.CurrentElement.CurrentAttribute.NodeValue;

                        case XPathNodeType.Text:
                            return this.CurrentElement.TextNode.NodeValue;

                        case XPathNodeType.ProcessingInstruction:
                            return this.CurrentProcessingInstruction.NodeValue;

                        case XPathNodeType.Comment:
                            return this.CurrentComment.NodeValue;
                    }
                }
                return string.Empty;
            }
        }

        public WriteState WriteState
        {
            get
            {
                WriteState error = WriteState.Error;
                if (this.CurrentElement == null)
                {
                    return WriteState.Start;
                }
                if (this._closed)
                {
                    return WriteState.Closed;
                }
                switch (this._state)
                {
                    case XPathNodeType.Element:
                        return WriteState.Element;

                    case XPathNodeType.Attribute:
                        return WriteState.Attribute;

                    case XPathNodeType.Namespace:
                        return error;

                    case XPathNodeType.Text:
                        return WriteState.Content;

                    case XPathNodeType.Comment:
                        return WriteState.Content;
                }
                return error;
            }
        }

        // Nested Types
        public class AttributeNode : TraceXPathNavigator.TraceNode
        {
            // Fields
            private string _name;
            private string _nodeValue;
            private string _prefix;
            private string _xmlns;

            // Methods
            public AttributeNode(string name, string prefix, string nodeValue, string xmlns, TraceXPathNavigator.ElementNode parent) : base(XPathNodeType.Attribute, parent)
            {
                this._name = name;
                this._nodeValue = nodeValue;
                this._prefix = prefix;
                this._xmlns = xmlns;
            }

            // Properties
            public string Name
            {
                get
                {
                    return this._name;
                }
            }

            public string NameSpace
            {
                get
                {
                    return this._xmlns;
                }
            }

            public override string NodeValue
            {
                get
                {
                    return this._nodeValue;
                }
            }

            public string Prefix
            {
                get
                {
                    return this._prefix;
                }
            }

            public override int Size
            {
                get
                {
                    int num = (this._name.Length + this._nodeValue.Length) + 5;
                    if (!string.IsNullOrEmpty(this._prefix))
                    {
                        num += this._prefix.Length + 1;
                    }
                    if (!string.IsNullOrEmpty(this._xmlns))
                    {
                        num += this._xmlns.Length + 9;
                    }
                    return num;
                }
            }
        }

        public class CommentNode : TraceXPathNavigator.TraceNode
        {
            // Fields
            public string _nodeValue;

            // Methods
            public CommentNode(string text, TraceXPathNavigator.ElementNode parent) : base(XPathNodeType.Comment, parent)
            {
                this._nodeValue = text;
            }

            // Properties
            public override string NodeValue
            {
                get
                {
                    return this._nodeValue;
                }
            }

            public override int Size
            {
                get
                {
                    return (this._nodeValue.Length + 8);
                }
            }
        }

        public class ElementNode : TraceXPathNavigator.TraceNode
        {
            // Fields
            private int _attributeIndex;
            private List<TraceXPathNavigator.AttributeNode> _attributes;
            private List<TraceXPathNavigator.TraceNode> _childNodes;
            private int _elementIndex;
            private bool _movedToText;
            private string _name;
            private string _prefix;
            private TraceXPathNavigator.TextNode _textNode;
            private string _xmlns;

            // Methods
            public ElementNode(string name, string prefix, TraceXPathNavigator.ElementNode parent, string xmlns) : base(XPathNodeType.Element, parent)
            {
                this._childNodes = new List<TraceXPathNavigator.TraceNode>();
                this._attributes = new List<TraceXPathNavigator.AttributeNode>();
                this._name = name;
                this._prefix = prefix;
                this._xmlns = xmlns;
            }

            public void Add(TraceXPathNavigator.TraceNode node)
            {
                this._childNodes.Add(node);
            }

            public IEnumerable<TraceXPathNavigator.ElementNode> FindSubnodes(string[] headersPath)
            {
                <FindSubnodes>d__0 d__ = new <FindSubnodes>d__0(-2);
                d__.<>4__this = this;
                d__.<>3__headersPath = headersPath;
                return d__;
            }

            public bool MoveToFirstAttribute()
            {
                this._attributeIndex = 0;
                return ((this._attributes != null) && (this._attributes.Count > 0));
            }

            public TraceXPathNavigator.TraceNode MoveToNext()
            {
                TraceXPathNavigator.TraceNode node = null;
                if ((this._elementIndex + 1) < this._childNodes.Count)
                {
                    this._elementIndex++;
                    node = this._childNodes[this._elementIndex];
                }
                return node;
            }

            public bool MoveToNextAttribute()
            {
                bool flag = false;
                if ((this._attributeIndex + 1) < this._attributes.Count)
                {
                    this._attributeIndex++;
                    flag = true;
                }
                return flag;
            }

            public void Reset()
            {
                this._attributeIndex = 0;
                this._elementIndex = 0;
                this._movedToText = false;
                if (this._childNodes != null)
                {
                    foreach (TraceXPathNavigator.TraceNode node in this._childNodes)
                    {
                        if (node.NodeType == XPathNodeType.Element)
                        {
                            TraceXPathNavigator.ElementNode node2 = node as TraceXPathNavigator.ElementNode;
                            if (node2 != null)
                            {
                                node2.Reset();
                            }
                        }
                    }
                }
            }

            // Properties
            public List<TraceXPathNavigator.AttributeNode> Attributes
            {
                get
                {
                    return this._attributes;
                }
            }

            public List<TraceXPathNavigator.TraceNode> ChildNodes
            {
                get
                {
                    return this._childNodes;
                }
            }

            public TraceXPathNavigator.AttributeNode CurrentAttribute
            {
                get
                {
                    return this._attributes[this._attributeIndex];
                }
            }

            public bool MovedToText
            {
                get
                {
                    return this._movedToText;
                }
                set
                {
                    this._movedToText = value;
                }
            }

            public string Name
            {
                get
                {
                    return this._name;
                }
            }

            public string NameSpace
            {
                get
                {
                    return this._xmlns;
                }
            }

            public override string NodeValue
            {
                get
                {
                    return string.Empty;
                }
            }

            public string Prefix
            {
                get
                {
                    return this._prefix;
                }
            }

            public override int Size
            {
                get
                {
                    int num = (2 * this._name.Length) + 6;
                    if (!string.IsNullOrEmpty(this._prefix))
                    {
                        num += this._prefix.Length + 1;
                    }
                    if (!string.IsNullOrEmpty(this._xmlns))
                    {
                        num += this._xmlns.Length + 9;
                    }
                    return num;
                }
            }

            public TraceXPathNavigator.TextNode TextNode
            {
                get
                {
                    return this._textNode;
                }
                set
                {
                    this._textNode = value;
                }
            }

            // Nested Types
            [CompilerGenerated]
            private sealed class <FindSubnodes>d__0 : IEnumerable<TraceXPathNavigator.ElementNode>, IEnumerable, IEnumerator<TraceXPathNavigator.ElementNode>, IEnumerator, IDisposable
            {
                // Fields
                private int <>1__state;
                private TraceXPathNavigator.ElementNode <>2__current;
                public string[] <>3__headersPath;
                public TraceXPathNavigator.ElementNode <>4__this;
                public List<TraceXPathNavigator.TraceNode>.Enumerator <>7__wrap6;
                private int <>l__initialThreadId;
                public TraceXPathNavigator.TraceNode <child>5__4;
                public TraceXPathNavigator.ElementNode <childNode>5__5;
                public int <i>5__2;
                public TraceXPathNavigator.ElementNode <node>5__1;
                public TraceXPathNavigator.ElementNode <subNode>5__3;
                public string[] headersPath;

                // Methods
                [DebuggerHidden]
                public <FindSubnodes>d__0(int <>1__state)
                {
                    this.<>1__state = <>1__state;
                    this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
                }

                private void <>m__Finally7()
                {
                    this.<>1__state = -1;
                    this.<>7__wrap6.Dispose();
                }

                private bool MoveNext()
                {
                    try
                    {
                        switch (this.<>1__state)
                        {
                            case 0:
                                this.<>1__state = -1;
                                if (this.headersPath != null)
                                {
                                    this.<node>5__1 = this.<>4__this;
                                    if (string.CompareOrdinal(this.<node>5__1._name, this.headersPath[0]) != 0)
                                    {
                                        this.<node>5__1 = null;
                                    }
                                    this.<i>5__2 = 0;
                                    while ((this.<node>5__1 != null) && (++this.<i>5__2 < this.headersPath.Length))
                                    {
                                        this.<subNode>5__3 = null;
                                        if (this.<node>5__1._childNodes == null)
                                        {
                                            goto Label_014F;
                                        }
                                        this.<>7__wrap6 = this.<node>5__1._childNodes.GetEnumerator();
                                        this.<>1__state = 1;
                                    Label_0139:
                                        while (this.<>7__wrap6.MoveNext())
                                        {
                                            this.<child>5__4 = this.<>7__wrap6.Current;
                                            if (this.<child>5__4.NodeType == XPathNodeType.Element)
                                            {
                                                this.<childNode>5__5 = this.<child>5__4 as TraceXPathNavigator.ElementNode;
                                                if ((this.<childNode>5__5 != null) && (string.CompareOrdinal(this.<childNode>5__5._name, this.headersPath[this.<i>5__2]) == 0))
                                                {
                                                    if (this.headersPath.Length == (this.<i>5__2 + 1))
                                                    {
                                                        this.<>2__current = this.<childNode>5__5;
                                                        this.<>1__state = 2;
                                                        return true;
                                                    }
                                                    this.<subNode>5__3 = this.<childNode>5__5;
                                                    break;
                                                }
                                            }
                                        }
                                        this.<>m__Finally7();
                                    Label_014F:
                                        this.<node>5__1 = this.<subNode>5__3;
                                    }
                                }
                                break;

                            case 2:
                                this.<>1__state = 1;
                                goto Label_0139;
                        }
                        return false;
                    }
                    fault
                    {
                        this.System.IDisposable.Dispose();
                    }
                }

                [DebuggerHidden]
                IEnumerator<TraceXPathNavigator.ElementNode> IEnumerable<TraceXPathNavigator.ElementNode>.GetEnumerator()
                {
                    TraceXPathNavigator.ElementNode.<FindSubnodes>d__0 d__;
                    if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                    {
                        this.<>1__state = 0;
                        d__ = this;
                    }
                    else
                    {
                        d__ = new TraceXPathNavigator.ElementNode.<FindSubnodes>d__0(0);
                        d__.<>4__this = this.<>4__this;
                    }
                    d__.headersPath = this.<>3__headersPath;
                    return d__;
                }

                [DebuggerHidden]
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.System.Collections.Generic.IEnumerable<Microsoft.IdentityModel.Diagnostics.TraceXPathNavigator.ElementNode>.GetEnumerator();
                }

                [DebuggerHidden]
                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }

                void IDisposable.Dispose()
                {
                    switch (this.<>1__state)
                    {
                        case 1:
                        case 2:
                            try
                            {
                            }
                            finally
                            {
                                this.<>m__Finally7();
                            }
                            return;
                    }
                }

                // Properties
                TraceXPathNavigator.ElementNode IEnumerator<TraceXPathNavigator.ElementNode>.Current
                {
                    [DebuggerHidden]
                    get
                    {
                        return this.<>2__current;
                    }
                }

                object IEnumerator.Current
                {
                    [DebuggerHidden]
                    get
                    {
                        return this.<>2__current;
                    }
                }
            }
        }

        public class ProcessingInstructionNode : TraceXPathNavigator.TraceNode
        {
            // Fields
            private string _name;
            private string _nodeValue;

            // Methods
            public ProcessingInstructionNode(string name, string text, TraceXPathNavigator.ElementNode parent) : base(XPathNodeType.ProcessingInstruction, parent)
            {
                this._name = name;
                this._nodeValue = text;
            }

            // Properties
            public string Name
            {
                get
                {
                    return this._name;
                }
            }

            public override string NodeValue
            {
                get
                {
                    return this._nodeValue;
                }
            }

            public override int Size
            {
                get
                {
                    return ((this._name.Length + this._nodeValue.Length) + 12);
                }
            }
        }

        public class TextNode : TraceXPathNavigator.TraceNode
        {
            // Fields
            private string _nodeValue;

            // Methods
            public TextNode(string nodeValue, TraceXPathNavigator.ElementNode parent) : base(XPathNodeType.Text, parent)
            {
                this._nodeValue = nodeValue;
            }

            public void AddText(string text)
            {
                this._nodeValue = this._nodeValue + text;
            }

            // Properties
            public override string NodeValue
            {
                get
                {
                    return this._nodeValue;
                }
            }

            public override int Size
            {
                get
                {
                    return this._nodeValue.Length;
                }
            }
        }

        public abstract class TraceNode
        {
            // Fields
            private XPathNodeType _nodeType;
            private TraceXPathNavigator.ElementNode _parent;

            // Methods
            public TraceNode(XPathNodeType nodeType, TraceXPathNavigator.ElementNode parent)
            {
                this._nodeType = nodeType;
                this._parent = parent;
            }

            // Properties
            public XPathNodeType NodeType
            {
                get
                {
                    return this._nodeType;
                }
            }

            public abstract string NodeValue { get; }

            public TraceXPathNavigator.ElementNode Parent
            {
                get
                {
                    return this._parent;
                }
            }

            public abstract int Size { get; }
        }
    }
}
