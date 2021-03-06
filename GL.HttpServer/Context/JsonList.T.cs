﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GL.HttpServer.Dto;

namespace GL.HttpServer.Context
{
    public class JsonList<T> : JsonResponse, IList<T> where T : IDto, new()
    {
        private readonly List<T> _items = new List<T>();

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                return _items[index];
            }

            set
            {
                _items[index] = value;
            }
        }

        public int IndexOf(T item)
        {
           return _items.IndexOf(item);
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
           return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
           return _items.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        protected override void WriteJson(Stream stream)
        {
            var json = JsonConvert.SerializeObject(_items);
            var bytes = Encoding.UTF8.GetBytes(json);
            Headers.Add("Content-Length", json.Length.ToString());
            stream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
