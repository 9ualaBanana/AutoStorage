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

using System.Timers;

namespace System.Collections.Specialized;

public readonly struct StorageTime
{
    readonly Interval? _value;

    public static readonly Interval? Unlimited = null;
    public static readonly Interval Default = default;

    public bool IsUnlimited => _value == Unlimited;
    public bool IsDefault => _value == Default;


    StorageTime(Interval? storageTime) => _value = storageTime;


    #region Conversions
    public static implicit operator StorageTime(Interval? storageTime) => new(storageTime);
    public static implicit operator Interval?(StorageTime storageTime) => storageTime._value;

    public static implicit operator StorageTime(TimeSpan? storageTime) => new(storageTime);
    public static implicit operator TimeSpan?(StorageTime storageTime) => storageTime._value;

    public static implicit operator StorageTime(double? storageTime) => new(storageTime);
    public static implicit operator double?(StorageTime storageTime) => storageTime._value;
    #endregion
}
