// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Internal.Cryptography;
using Internal.NativeCrypto;

namespace System.Security.Cryptography
{
    public sealed partial class AesCcm
    {
        private SafeKeyHandle _keyHandle;

        public static bool IsSupported => true;

        [MemberNotNull(nameof(_keyHandle))]
        private void ImportKey(ReadOnlySpan<byte> key)
        {
            _keyHandle = Interop.BCrypt.BCryptImportKey(BCryptAeadHandleCache.AesCcm, key);
        }

        private void EncryptCore(
            ReadOnlySpan<byte> nonce,
            ReadOnlySpan<byte> plaintext,
            Span<byte> ciphertext,
            Span<byte> tag,
            ReadOnlySpan<byte> associatedData = default)
        {
            AeadCommon.Encrypt(_keyHandle, nonce, associatedData, plaintext, ciphertext, tag);
        }

        private void DecryptCore(
            ReadOnlySpan<byte> nonce,
            ReadOnlySpan<byte> ciphertext,
            ReadOnlySpan<byte> tag,
            Span<byte> plaintext,
            ReadOnlySpan<byte> associatedData = default)
        {
            // BCrypt implementation of CCM clears plaintext for you on failure
            AeadCommon.Decrypt(_keyHandle, nonce, associatedData, ciphertext, tag, plaintext, clearPlaintextOnFailure: false);
        }

        public void Dispose()
        {
            _keyHandle.Dispose();
        }
    }
}
