/* 
    The MIT License

    Copyright February 8, 2016 Shawn Gilroy. http://www.smallnstats.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using FastTalkerSkiaSharp.Interfaces;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FastTalkerSkiaSharp.Controls
{
    public class IconCollection : ICollection<Icon>, IList<Icon>
    {
        private List<Icon> _items;
        private IconsCollection _container;

        internal IconCollection(IconsCollection container)
        {
            _container = container;
            _items = new List<Icon>();
        }

        public Icon this[int index]
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

        public void Add(Icon item)
        {
            _items.Add(item);
            SetParent(item);
            Invalidate();
        }

        public void AddRange(Icon[] items)
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

        public bool Contains(Icon item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(Icon[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Icon> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(Icon item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, Icon item)
        {
            _items.Insert(index, item);
            SetParent(item);
            Invalidate();
        }

        public bool Remove(Icon item)
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

        public void BringToFront(Icon item)
        {
            if (_items.Remove(item))
            {
                _items.Add(item);
                Invalidate();
            }
        }

        public void SendToBack(Icon item)
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

        private void SetParent(Icon[] items)
        {
            foreach (var item in items)
            {
                SetParent(item);
            }
        }

        private void SetParent(Icon item)
        {
            item.Parent = _container;
        }

        private void RemoveParent(IList<Icon> items)
        {
            foreach (var item in items)
            {
                RemoveParent(item);
            }
        }

        private void RemoveParent(Icon item)
        {
            item.Parent = null;
        }

        private void Invalidate()
        {
            _container?.Invalidate();
        }

        public Icon GetIconAtPoint(SKPoint point)
        {
            return GetIconAtPoint(point, null);
        }

        public Icon GetIconAtPoint(SKPoint point, Func<Icon, bool> predicate)
        {
            List<Icon> items;
            if (predicate != null)
            {
                items = _items.Where(predicate)
                              .Where(i => !i.IsStoredInAFolder).ToList();
            }
            else
            {
                items = _items;
            }

            for (var e = items.Count - 1; e >= 0; e--)
            {
                var icon = items[e];
                if (icon.IsPointInside(point) && !icon.IsStoredInAFolder)
                {
                    var collector = icon as IconsCollection;
                    if (collector != null)
                    {
                        var subIcon = collector.Icons.GetIconAtPoint(point);
                        if (subIcon != null)
                        {
                            return subIcon;
                        }
                    }
                    else
                    {
                        return icon;
                    }
                }
            }
            return null;
        }
    }
}
