using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace Common
{
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
