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

internal class StorageTimerFactory
{
    internal readonly StorageTime DefaultStorageTime;
    internal static readonly StorageTimer UnlimitedStorageTimer = StorageTimer.Unlimited;
    internal StorageTimer DefaultStorageTimer => CreateWith(DefaultStorageTime);

    readonly ElapsedEventHandler _onElapsed;


    internal StorageTimerFactory(StorageTime? defaultStorageTime, ElapsedEventHandler onElapsed)
    {
        DefaultStorageTime = defaultStorageTime ?? StorageTime.Unlimited;
        _onElapsed = onElapsed;
    }


    internal StorageTimer CreateWith(StorageTime storageTime) =>
        (storageTime.IsUnlimited ? StorageTimer.Unlimited : storageTime.IsDefault ?
        DefaultStorageTimer : new StorageTimer(storageTime)
        ).InitializeWith(_onElapsed);
}
