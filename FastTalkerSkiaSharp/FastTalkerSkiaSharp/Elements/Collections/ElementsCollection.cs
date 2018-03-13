﻿/*
   Copyright February 8, 2016 Shawn Gilroy

   This file is part of Fast Talker
  
   Fast Talker is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, version 3.

   Fast Talker is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with Fast Talker.  If not, see http://www.gnu.org/licenses/. 
   </copyright>

   The Fast Talker is a tool to assist clinicans and researchers in the treatment of communication disorders.

   Email: shawn(dot)gilroy(at)temple.edu

   =========================================================================================================
   
   Based on SkiaSharp.Elements
   Felipe Nicoletto
   https://github.com/FelipeNicoletto/SkiaSharp.Elements

*/

using SkiaSharp.Elements.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SkiaSharp.Elements.Collections
{
    public class ElementsCollection : ICollection<Element>, IList<Element>
    {
        private List<Element> _items;
        private IElementContainer _container;

        internal ElementsCollection(IElementContainer container)
        {
            _container = container;
            _items = new List<Element>();
        }

        public Element this[int index]
        {
            get { return _items[index]; }
            set
            {
                _items[index] = value;
                SetParent(value);
                Invalidate();
            }
        }
        
        public int Count => _items.Count;

        public bool IsReadOnly => false;
        
        public void Add(Element item)
        {
            _items.Add(item);
            SetParent(item);
            Invalidate();
        }
        
        public void AddRange(Element[] items)
        {
            _items.AddRange(items);
            SetParent(items);
            Invalidate();
        }

        public void Clear()
        {
            RemoveParent(_items);
            _items.Clear();
            Invalidate();
        }

        public bool Contains(Element item)
        {
            return _items.Contains(item);
        }
        
        public void CopyTo(Element[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }
        
        public IEnumerator<Element> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(Element item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, Element item)
        {
            _items.Insert(index, item);
            SetParent(item);
            Invalidate();
        }
        
        public bool Remove(Element item)
        {
            if (_items.Remove(item))
            {
                RemoveParent(item);
                Invalidate();
                return true;
            }
            return false;
        }
        
        public void RemoveAt(int index)
        {
            RemoveParent(_items[index]);
            _items.RemoveAt(index);
            Invalidate();
        }

        public void BringToFront(Element item)
        {
            if (_items.Remove(item))
            {
                _items.Add(item);
                Invalidate();
            }
        }

        public void SendToBack(Element item)
        {
            if (_items.Remove(item))
            {
                _items.Insert(0, item);
                Invalidate();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        private void SetParent(Element[] items)
        {
            foreach(var item in items)
            {
                SetParent(item);
            }
        }

        private void SetParent(Element item)
        {
            item.Parent = _container;
        }

        private void RemoveParent(IList<Element> items)
        {
            foreach (var item in items)
            {
                RemoveParent(item);
            }
        }

        private void RemoveParent(Element item)
        {
            item.Parent = null;
        }

        private void Invalidate()
        {
            _container?.Invalidate();
        }

        public Element GetElementAtPoint(SKPoint point)
        {
            return GetElementAtPoint(point, null);
        }

        public Element GetElementAtPoint(SKPoint point, Func<Element, bool> predicate)
        {
            List<Element> items;
            if (predicate != null)
            {
                //items = _items.Where(predicate).ToList();
                items = _items.Where(predicate)
                              .Where(i => !i.IsStoredInAFolder).ToList();
            }
            else
            {
                items = _items;
            }

            for(var e = items.Count - 1; e >= 0; e--)
            {
                var element = items[e];
                if (element.IsPointInside(point) && !element.IsStoredInAFolder)
                {
                    var collector = element as IElementsCollector;
                    if (collector != null)
                    {
                        var subElement = collector.Elements.GetElementAtPoint(point);
                        if (subElement != null)
                        {
                            return subElement;
                        }
                    }
                    else
                    {
                        return element;
                    }
                }
            }
            return null;
        }
    }
}
