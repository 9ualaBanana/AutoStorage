//MIT License

//Copyright (c) 2022 9ualaBanana

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections;
using System.Collections.Specialized;

namespace AutoStorage;

public class IndexedAutoStorage<I, T> : ICollection<KeyValuePair<I, T>
{
    readonly Dictionary<I, T> _indexedAutoStorage;
    readonly AutoStorage<T> _autoStorage;

    public int Count => _autoStorage.Count;
    public bool IsReadOnly => false;
    public void Add(KeyValuePair<I, T> item)
    {
        _indexedAutoStorage.Add(item.Key, item.Value);

    }

    public void Clear() { _autoStorage.Clear(); _indexedAutoStorage.Clear(); }

    public bool Contains(KeyValuePair<I, T> item) => _autoStorage.Contains(item.Value);

    public void CopyTo(KeyValuePair<I, T>[] array, int arrayIndex) => _autoStorage.CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<I, T> item)
    { _autoStorage.Remove(item.Value); return _indexedAutoStorage.Remove(item.Key); }

    IEnumerator IEnumerable.GetEnumerator() => _indexedAutoStorage.GetEnumerator();
    public IEnumerator<KeyValuePair<I, T>> GetEnumerator() => _indexedAutoStorage.GetEnumerator();
}
