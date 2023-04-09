using System;
using System.Collections.Generic;
using System.Linq;

namespace Webinex.Activity
{
    internal class JsonPath
    {
        private readonly LinkedList<Item> _path = new LinkedList<Item>();

        public string Value { get; }

        public Item[] Path => _path.ToArray();

        public JsonPath(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));

            var startIndex = 0;

            for (int i = 0; i < value.Length; i++)
            {
                var letter = value[i];

                if (i == value.Length - 1)
                {
                    if (startIndex == i)
                    {
                        break;
                    }

                    var path = Value.Substring(startIndex);
                    _path.AddLast(new Item(path));
                    break;
                }

                switch (letter)
                {
                    case '.':
                    {
                        if (startIndex == i)
                        {
                            startIndex = i + 1;
                            break;
                        }

                        var path = Value.Substring(startIndex, i - startIndex);
                        _path.AddLast(new Item(path));
                        startIndex = i + 1;
                        break;
                    }

                    case '[':
                    {
                        if (startIndex != i)
                        {
                            var path = Value.Substring(startIndex, i - startIndex);
                            _path.AddLast(new Item(path));
                            startIndex = i;
                        }

                        var end = Value.IndexOf(']', i);
                        var indexNumber = Value.Substring(i + 1, end - i - 1);
                        _path.AddLast(new Item(indexNumber, true));
                        startIndex = end + 1;
                        i = end;
                        break;
                    }
                }
            }
        }

        public class Item
        {
            public Item(string value, bool isIndex = false)
            {
                Value = value;
                IsIndex = isIndex;
            }

            public string Value { get; }

            public bool IsIndex { get; }

            public int Index => int.Parse(Value);
        }
    }
}