﻿// 
// Copyright (c) 2004-2016 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;

namespace NLog.Internal
{
    /// <summary>
    /// Controls a single allocated char[]-buffer for reuse (only one active user)
    /// </summary>
    internal class ReusableBufferCreator
    {
        private char[] _buffer;

        /// <summary>Empty handle when <see cref="Targets.Target.OptimizeBufferReuse"/> is disabled</summary>
        public readonly LockBuffer None = default(LockBuffer);

        public ReusableBufferCreator(int capacity)
        {
            _buffer = new char[capacity];
        }

        /// <summary>
        /// Creates handle to the reusable char[]-buffer for active usage
        /// </summary>
        /// <returns>Handle to the reusable item, that can release it again</returns>
        public LockBuffer Allocate()
        {
            return new LockBuffer(this);
        }

        public struct LockBuffer : IDisposable
        {
            /// <summary>
            /// Access the char[]-buffer acquired
            /// </summary>
            public readonly char[] Result;
            private readonly ReusableBufferCreator _owner;

            public LockBuffer(ReusableBufferCreator owner)
            {
                Result = owner._buffer;
                owner._buffer = null;
                _owner = owner;
            }

            public void Dispose()
            {
                if (Result != null)
                {
                    _owner._buffer = Result;
                }
            }
        }
    }
}
